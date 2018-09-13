using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    protected BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    protected Vector2 index;

	// Use this for initialization
	protected virtual void Start () {
        index = HexToIndex(transform.position);
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1.0f / moveTime;
	}

    protected virtual bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = IndexToHex((int)index.x + xDir, (int)index.y + yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(IndexToHex((int)index.x + xDir, (int)index.y + yDir)));
            index.x += xDir; index.y += yDir;
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement (Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;

        if (xDir == 1 || (xDir == 0 && yDir == -1))
            transform.localScale = Vector3.one;
        else
            transform.localScale = new Vector3(-1, 1, 1);

        if (Move(xDir, yDir, out hit))
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        OnCantMove(hitComponent);
    }

    protected Vector2 IndexToHex(int xDir, int yDir)
    {
        return new Vector2(xDir * 1.2f - yDir * 0.6f, yDir * Mathf.Sqrt(1.2f * 1.2f - 0.6f * 0.6f));
    }

    protected Vector2 IndexToHex(Vector2 index)
    {
        return new Vector2(index.x * 1.2f - index.y * 0.6f, index.y * Mathf.Sqrt(1.2f * 1.2f - 0.6f * 0.6f));
    }

    protected Vector2 HexToIndex(Vector2 pos)
    {
        float y = pos.y / Mathf.Sqrt(1.2f * 1.2f - .6f * .6f);
        return new Vector2((int)(pos.x / 1.2f + y / 2.0f), (int)y);     
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;

}

using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    [SerializeField] int playerDamage = 1;

    private Animator animator;
    private Player targetScript;
    private bool skipMove;
    private Vector2 exitIndex;

    [SerializeField] AudioClip enemyAttack1;
    [SerializeField] AudioClip enemyAttack2;

    private LinkedList<Vector2> options = new LinkedList<Vector2>();
    private LinkedListNode<Vector2> node;

    // Use this for initialization
    protected override void Start() { 
        GameManager.instance.AddEnemyToList(this);
        exitIndex = -GameManager.instance.startIndex;
        animator = GetComponentInChildren<Animator>();
        targetScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        base.Start();
	}

    private void ChooseDir()
    {
        options.Clear();

        Vector2 target = targetScript.GetIndex();
        int targetX = (int)target.x;
        int targetY = (int)target.y;
        int thisX = (int)index.x;
        int thisY = (int)index.y;

        if (targetX == thisX) // same X
        {
            if (targetY > thisY) // X higher
                options.AddFirst(Vector2.up);
            else // X lower
                options.AddFirst(Vector2.down);
        }
        else if (targetY == thisY) // same Y
        {
            if (targetX > thisX) // Y higher
                options.AddFirst(Vector2.right);
            else // Y lower
                options.AddFirst(Vector2.left);
        }
        else
        {
            int difX = targetX - thisX;
            int difY = targetY - thisY;

            if (difX == difY)
            {
                if (targetX > thisX) // Z higher
                    options.AddFirst(Vector2.one);
                else // Z lower
                    options.AddFirst(-Vector2.one);
            }
            else if (targetX > thisX)
            {
                if (targetY < thisY)
                    RankTwoOptions(Vector2.right, Vector2.down);
                else if (difX > difY) // && targetY > thisY
                    RankTwoOptions(Vector2.one, Vector2.right);
                else // && targetY > thisY && difX < difY
                    RankTwoOptions(Vector2.up, Vector2.one);
            }
            else // targetX < thisX
            {
                if (targetY > thisY)
                    RankTwoOptions(Vector2.left, Vector2.up);                    
                else if (difX > difY) // && targetY < thisY
                    RankTwoOptions(-Vector2.one, Vector2.down);
                else // && targetY < thisY && difX < difY
                    RankTwoOptions(-Vector2.one, Vector2.left);
            }
        }

        node = options.First;
        AttemptMove<Player>((int)node.Value.x, (int)node.Value.y);

    }

    private void RankTwoOptions(Vector2 optionOne, Vector2 optionTwo)
    {
        options.AddFirst(optionOne);

        Vector2 target = targetScript.GetIndex();

        if (Mathf.Abs(SqrDistance(target, index + optionOne) - SqrDistance(target, index + optionTwo)) < 0.1f)
        {
            if (SqrDistance(exitIndex, index + optionOne) < SqrDistance(exitIndex, index + optionTwo))
                options.AddLast(optionTwo);
            else
                options.AddFirst(optionTwo);     
        }
        else if (SqrDistance(target, index + optionOne) < SqrDistance(target, index + optionTwo))
            options.AddLast(optionTwo);        
        else
            options.AddFirst(optionTwo);          
    }

    private float SqrDistance(Vector2 target, Vector2 option)
    {
        return (IndexToHex(target) - IndexToHex(option)).sqrMagnitude;
    }

    public void MoveEnemy()
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        ChooseDir();

        skipMove = true;   
    }

    protected override void OnCantMove <T> (T component)
    {
        Player hitPlayer = component as Player;

        if (hitPlayer != null)
        {
            hitPlayer.TakeDamage(playerDamage);

            animator.SetTrigger("EnemyKick");

            SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        }
        else if (node.Next != null)
        {
            node = node.Next;
            AttemptMove<Player>((int)node.Value.x, (int)node.Value.y);
        }

    }


}

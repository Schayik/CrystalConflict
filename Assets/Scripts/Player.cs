using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject {

    [SerializeField] float restartLevelDelay = 1.0f;
    [SerializeField] int waterPerCan = 10;
    public int wallDamage = 2;
    [SerializeField] Text waterText;
    [SerializeField] Text scoreText;

    [SerializeField] private int water = 50;
    private int score = 0;

    private Transform child;
    private Animator animator;
    private bool isDead = false;

    [SerializeField] private AudioClip openDoorSound;
    [SerializeField] private AudioClip doNothingSound;
    [SerializeField] private AudioClip moveSound1;
    [SerializeField] private AudioClip moveSound2;
    [SerializeField] private AudioClip moveSound3;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip moneySound;
    [SerializeField] private AudioClip drinkSound;

    // Use this for initialization
    protected override void Start() {
        transform.position = Vector2.zero;

        child = transform.GetChild(0);
        animator = child.GetComponent<Animator>();

        waterText.text = "Water: " + water;
        CheckColor();
        scoreText.text = "Score: " + score;

        base.Start();
    }

    // Update is called once per frame
    void Update() {
        FromButton(0);
    }

    public Vector2 GetIndex()
    {
        return index;
    }

    public void FromButton(int nr)
    {
        if (!GameManager.instance.playersTurn || GameManager.instance.doingSetup || isDead)
            return;

        Vector2 dir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.E) || nr == 1)
            dir = Vector2.one;
        else if (Input.GetKeyDown(KeyCode.D) || nr == 2)
            dir = Vector2.right;
        else if (Input.GetKeyDown(KeyCode.X) || nr == 3)
            dir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.Z) || nr == 4)
            dir = -Vector2.one;
        else if (Input.GetKeyDown(KeyCode.A) || nr == 5)
            dir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.W) || nr == 6)
            dir = Vector2.up;

        if (dir != Vector2.zero)
            AttemptMove<Wall>((int)dir.x, (int)dir.y);
    }

    protected override bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        bool didMove = base.Move(xDir, yDir, out hit);

        if (didMove)
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2, moveSound3);
        else
            SoundManager.instance.RandomizeSfx(doNothingSound);

        return didMove;
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        waterText.text = "Water: " + --water;
        CheckColor();
        scoreText.text = "Score: " + score;

        base.AttemptMove<T>(xDir, yDir);

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            GameManager.instance.doingSetup = true;
            SoundManager.instance.RandomizeSfx(openDoorSound);
            Invoke("NextLevel", restartLevelDelay);
        }
        else if (other.tag == "Can")
        {
            ChangeWater(waterPerCan);
            SoundManager.instance.RandomizeSfx(drinkSound);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Crystals")
        {
            IncreaseScore(other.GetComponent<Crystals>().GetValue());
            SoundManager.instance.RandomizeSfx(moneySound);
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
            InteractableObject interactable = component.GetComponent<InteractableObject>();
            interactable.Interact(this);
    }

    public void SetAnimatorTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    private void NextLevel()
    {
        index = -index;
        transform.position = IndexToHex(index);
        GameManager.instance.NextLevel();
    }

    public void IncreaseScore(int gain)
    {
        score += gain;
        scoreText.text = "+" + gain + ", Score: " + score;
    }

    private void ChangeWater(int change)
    {
        water += change;

        if (change < 0)
            waterText.text = change + ", Water: " + water;
        else
            waterText.text = "+" + change + ", Water: " + water;

        CheckColor();
    }

    private void CheckColor()
    {
        if (water <= 20)
            waterText.color = Color.red;
        else if (water <= 50)
            waterText.color = Color.white;
        else
            waterText.color = Color.green;
    }

    public void TakeDamage(int loss)
    {
        animator.SetTrigger("PlayerHit");
        ChangeWater(-loss);
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (water <= 0)
        {
            isDead = true;
            SoundManager.instance.RandomizeSfx(deathSound);
            GameManager.instance.GameOver(score);
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField] float levelStartDelay = 2.0f;
    [SerializeField] float turnDelay = 0.1f;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject deathImage;

    [HideInInspector] public Vector2 startIndex = Vector2.zero;

    public static GameManager instance = null;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private BoardManager boardScript;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    [HideInInspector] public bool doingSetup = true;

    // Use this for initialization
    void Awake () {

        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        enemies = new List<Enemy>();

        boardScript = GetComponent<BoardManager>();

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        InitGame();
    }

    public void NextLevel()
    {
        level++;

        InitGame();
    }

    private void InitGame()
    {
        doingSetup = true;

        enemies.Clear();

        boardScript.SetupScene(level);

        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
    }

    void HideLevelImage()
    {
        levelImage = GameObject.Find("LevelImage");
        levelImage.SetActive(false);
        doingSetup = false;
    }

    // Update is called once per frame
    void Update () {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
	}

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void GameOver(int score)
    {
        mainMenu.SetActive(true);
        deathImage.SetActive(true);
        levelText.text = "You died after " + level + " days."
            + System.Environment.NewLine + System.Environment.NewLine + "Your score: " + score + ".";
        levelImage.SetActive(true);

        enabled = false;
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

}

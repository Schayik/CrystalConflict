using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [SerializeField]
    private class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    [SerializeField] private int radius = 4;
    [SerializeField] private Count waterCount = new Count(1, 2);
    [SerializeField] private Count obstacleCount = new Count(3, 5);
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject[] floorTiles;
    [SerializeField] private GameObject[] waterTiles;
    [SerializeField] private GameObject[] crystalTiles;
    [SerializeField] private GameObject[] enemyTiles;
    [SerializeField] private GameObject[] wallTiles;
    [SerializeField] private GameObject[] obstacleTiles;

    private Transform boardHolder;
    private List<Vector2> gridPositions = new List<Vector2>();
    private List<Vector2> exitPositions = new List<Vector2>();

    void InitializeList()
    {
        gridPositions.Clear();
        exitPositions.Clear();

        int maxGrid = radius - 2;

        for (int x = -maxGrid; x <= maxGrid; x++)
        {
            if (x <= 0)
            {
                for (int y = -maxGrid; y <= maxGrid + x; y++)
                    gridPositions.Add(new Vector2(x, y));
            }
            else
            {
                for (int y = -maxGrid + x; y <= maxGrid; y++)
                    gridPositions.Add(new Vector2(x, y));
            }
        }

        exitPositions.Add(new Vector2(radius / 2, radius));
        exitPositions.Add(new Vector2(radius, radius / 2));
        exitPositions.Add(new Vector2(radius / 2, -radius / 2));
        exitPositions.Add(new Vector2(-radius / 2, -radius));
        exitPositions.Add(new Vector2(-radius, -radius / 2));
        exitPositions.Add(new Vector2(-radius / 2, radius / 2));

    }

    void BoardSetup()
    {
        if (boardHolder != null)
            Destroy(boardHolder.gameObject);

        boardHolder = new GameObject("Board").transform;

        Vector2 startIndex = GameManager.instance.startIndex;
        GameObject newStart;

        if (startIndex == Vector2.zero)
        {
            newStart = Instantiate(floorTiles[Random.Range(0, floorTiles.Length)], IndexToHex(startIndex), Quaternion.identity) as GameObject;
            gridPositions.Remove(Vector2.zero);
        }
        else
        {
            newStart = Instantiate(start, IndexToHex(startIndex), Quaternion.identity) as GameObject;
            newStart.transform.up = IndexToHex(-startIndex);
            exitPositions.Remove(startIndex);
        }

        newStart.transform.SetParent(boardHolder);

        int RandomIndex = Random.Range(0, exitPositions.Count);
        Vector2 exitIndex = exitPositions[RandomIndex];
 
        GameObject newExit = Instantiate(exit, IndexToHex(exitIndex), Quaternion.identity) as GameObject;
        newExit.transform.SetParent(boardHolder);
        newExit.transform.up = IndexToHex(exitIndex);

        for (int x = -radius; x <= radius; x++)
        {
            if (x <= 0)
            {
                for (int y = -radius; y <= radius + x; y++)
                {
                    Vector2 newTilePos = new Vector2(x, y);
                    if (newTilePos != startIndex && newTilePos != exitIndex)
                        TileInstantiation(new Vector2(x, y));
                }
            }
            else
            {
                for (int y = -radius + x; y <= radius; y++)
                {
                    Vector2 newTilePos = new Vector2(x, y);
                    if (newTilePos != startIndex && newTilePos != exitIndex)
                        TileInstantiation(new Vector2(x, y));
                }
            }
        }

        GameManager.instance.startIndex = -exitIndex;

    }

    void TileInstantiation(Vector2 index)
    {
        GameObject toInstantiate;

        if (Mathf.Abs(index.x) == radius || Mathf.Abs(index.y) == radius ||
            (Mathf.Abs(index.x) + Mathf.Abs(index.y) == radius && (index.x < 0.0f ^ index.y < 0.0f)))
        {
            toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
        }
        else
        {
            toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
        }

        GameObject instance = Instantiate(toInstantiate, IndexToHex(new Vector2(index.x, index.y)), Quaternion.identity) as GameObject;
        instance.transform.SetParent(boardHolder);
    }

    Vector3 RandomPosition()
    {
        int RandomIndex = Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[RandomIndex];
        gridPositions.RemoveAt(RandomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector2 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            GameObject newTile = Instantiate(tileChoice, IndexToHex(randomPosition), Quaternion.identity);
            newTile.transform.parent = boardHolder;
        }
    }

    public void SetupScene(int level)
    {
        InitializeList();
        BoardSetup();

        LayoutObjectAtRandom(obstacleTiles, obstacleCount.minimum, obstacleCount.maximum);
        LayoutObjectAtRandom(waterTiles, waterCount.minimum, waterCount.maximum);

        int crystalCount = 1 + (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(crystalTiles, crystalCount, crystalCount);

        int enemyCount = level / 2;
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
    }

    protected Vector2 IndexToHex(Vector2 index)
    {
        return new Vector2(index.x * 1.2f - index.y * 0.6f, index.y * Mathf.Sqrt(1.2f * 1.2f - 0.6f * 0.6f));
    }

}

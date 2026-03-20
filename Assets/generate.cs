using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance;

    [Header("Maze Settings")]
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab;
    public float spacing = 4f;

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public int coinCount = 10;
    private List<Vector2Int> validCoinPositions = new List<Vector2Int>();

    private MazeCell[,] cells;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateMaze();
        CollectValidCoinPositions();
        SpawnCoins(coinCount);
    }

    //================ 迷宫生成 ==================
    void GenerateMaze()
    {
        cells = new MazeCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new MazeCell();
            }
        }

        CarvePassageFrom(0, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellOrigin = new Vector3((x - width / 2) * spacing, 0, (y - height / 2) * spacing);

                if (cells[x, y].hasTopWall)
                {
                    Vector3 pos = cellOrigin + new Vector3(0, 0, spacing / 2);
                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                }

                if (cells[x, y].hasRightWall)
                {
                    Vector3 pos = cellOrigin + new Vector3(spacing / 2, 0, 0);
                    Instantiate(wallPrefab, pos, Quaternion.Euler(0, 90, 0), transform);
                }
            }
        }
    }

    void CarvePassageFrom(int x, int y)
    {
        cells[x, y].visited = true;

        List<Vector2Int> directions = new List<Vector2Int> {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };
        Shuffle(directions);

        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (nx >= 0 && ny >= 0 && nx < width && ny < height && !cells[nx, ny].visited)
            {
                if (dir == Vector2Int.up)
                {
                    cells[x, y].hasTopWall = false;
                }
                else if (dir == Vector2Int.down)
                {
                    cells[nx, ny].hasTopWall = false;
                }
                else if (dir == Vector2Int.left)
                {
                    cells[nx, ny].hasRightWall = false;
                }
                else if (dir == Vector2Int.right)
                {
                    cells[x, y].hasRightWall = false;
                }

                CarvePassageFrom(nx, ny);
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    class MazeCell
    {
        public bool visited = false;
        public bool hasTopWall = true;
        public bool hasRightWall = true;
    }

    public void GenerateNewMaze()
    {
        // 1. 清除所有子物体（包括墙和金币）
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 2. 重新生成迷宫
        GenerateMaze();

        // 3. 收集并生成金币
        CollectValidCoinPositions();
        SpawnCoins(coinCount);
    }

    //================ 金币生成 ==================

    void CollectValidCoinPositions()
    {
        validCoinPositions.Clear();
        int cx = width / 2;
        int cy = height / 2;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 排除中心3x3区域
                if (cells[x, y].visited && !(x >= cx - 1 && x <= cx + 1 && y >= cy - 1 && y <= cy + 1))
                {
                    validCoinPositions.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    void SpawnCoins(int count)
    {
        Shuffle(validCoinPositions);
        for (int i = 0; i < Mathf.Min(count, validCoinPositions.Count); i++)
        {
            SpawnCoinAt(validCoinPositions[i]);
        }
    }

    public void SpawnOneCoin()
    {
        if (validCoinPositions.Count == 0) return;
        Vector2Int pos = validCoinPositions[Random.Range(0, validCoinPositions.Count)];
        SpawnCoinAt(pos);
    }

    void SpawnCoinAt(Vector2Int cellPos)
    {
        Vector3 worldPos = new Vector3(
            (cellPos.x - width / 2) * spacing,
            0.5f,
            (cellPos.y - height / 2) * spacing
        );

        Instantiate(coinPrefab, worldPos, Quaternion.identity, transform);
    }
}
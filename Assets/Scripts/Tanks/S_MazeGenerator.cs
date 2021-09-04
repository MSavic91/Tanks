using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MazeGenerator : MonoBehaviour
{
    private static S_MazeGenerator instance;

#pragma warning disable 0649
    [SerializeField] private GameObject clearBoardTilePrefab;
    [SerializeField] private GameObject baseBoardTilePrefab;
    [SerializeField] private GameObject wallBoardTilePrefab;
    [SerializeField] private GameObject destructibleBoardTilePrefab;
    [SerializeField] private GameObject sidesParent;
    [SerializeField] private GameObject cornerTile;
    [SerializeField] private GameObject sideTile;
    [SerializeField] private GameObject sidesPlane;
    [SerializeField] private GameObject upDownPlane;
    [SerializeField] private Transform tiledPlane;

    [Header("Debug")]
#if UNITY_EDITOR
    [SerializeField] private bool UseSettingsData = false;
#endif
    [Range(4, 20)]
    [SerializeField] private int startRows = 1;
    [Range(1, 11)]
    [SerializeField] private int startColumns = 1;
    [Range(1, 4)]
    [SerializeField] private int enemiesToSpawn = 1;
    [Range(1, 60)]
    [SerializeField] private int totalObstaclesToSpawn = 10;
#pragma warning restore 0649

    private S_SearchAlgorythm checkAlgorythm;

    private TileDescription[][] generatedTiles;

    private TileDescription playerBaseTile;
    private TileDescription playerStartTile;
    private TileDescription[] enemyStartTiles;

    private int totalObstaclesSpawned = 0;

    public TileDescription[][] GeneratedTiles { get => generatedTiles; }
    public TileDescription PlayerBaseTile { get => playerBaseTile; }

    public int Rows { get => startRows; }
    public int Columns { get => startColumns; }

    List<TileDescription> availableTilesForObstacles = new List<TileDescription>();
    List<TileDescription> restrictedTilesForObstacles = new List<TileDescription>();

    public void Init()
    {
        instance = this;
#if UNITY_EDITOR
        if (UseSettingsData)
        {
#endif
            if (PlayerPrefs.HasKey(S_MM_Settings.TERRAIN_HEIGHT))
            {
                startRows = PlayerPrefs.GetInt(S_MM_Settings.TERRAIN_HEIGHT);
            }
            if (PlayerPrefs.HasKey(S_MM_Settings.TERRAIN_WIDTH))
            {
                startColumns = PlayerPrefs.GetInt(S_MM_Settings.TERRAIN_WIDTH);
            }
            if (PlayerPrefs.HasKey(S_MM_Settings.ENEMIES_COUNT))
            {
                enemiesToSpawn = PlayerPrefs.GetInt(S_MM_Settings.ENEMIES_COUNT);
            }
            if (PlayerPrefs.HasKey(S_MM_Settings.OBSTACLES_COUNT))
            {
                totalObstaclesToSpawn = PlayerPrefs.GetInt(S_MM_Settings.OBSTACLES_COUNT);
            }
#if UNITY_EDITOR
        }
#endif
    }

    public void ForceGenerate()
    {
        Generate(startRows, startColumns, enemiesToSpawn);
    }

    private void Generate(int rows, int columns, int numberOfEnemies)
    {
        //checkAlgorythm = new S_TankSearchAlgorythm();
        checkAlgorythm = new S_A_StarSearchAlgorythm();

        generatedTiles = new TileDescription[rows][];
        for(int i = 0; i < rows; i++)
        {
            generatedTiles[i] = new TileDescription[columns];
        }

        bool playerBaseSpawned = false;
        bool playerStartSpawned = false;
        numberOfEnemies = Mathf.Min(columns, numberOfEnemies);
        bool[] enemySpawnsSpawned = new bool[numberOfEnemies];
        int requiredEnemies = numberOfEnemies;
        if (numberOfEnemies < startColumns)
        {
            enemyStartTiles = new TileDescription[startColumns];
        }
        else
        {
            enemyStartTiles = new TileDescription[numberOfEnemies];
        }
        int spawnedEnemies = 0;

        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                int x = j;
                int y = i;
                bool shouldSpawnEnemy = spawnedEnemies < requiredEnemies && y == rows - 1 && x >= ((columns + 1) / requiredEnemies) * spawnedEnemies;
                if(y % 2 != 0)
                {
                    x = columns - 1 - x;
                }
                Vector3 position = Vector3.left * ((float)(columns - 1) / 2f) + Vector3.right * x
                                 + Vector3.back * ((float)(rows - 1) / 2f) + Vector3.forward * y;

                TileDescription newTile = null;
                if(!playerBaseSpawned && y == 0 && x == columns / 2)
                {
                    playerBaseTile = new TileDescription(x, y, TileType.Base, position);
                    newTile = playerBaseTile;
                    playerBaseSpawned = true;
                }
                else if(shouldSpawnEnemy)
                {
                    enemyStartTiles[spawnedEnemies] = new TileDescription(x, y, TileType.EnemyStart, position);
                    newTile = enemyStartTiles[spawnedEnemies];
                    spawnedEnemies++;
                }
                else
                {
                    newTile = new TileDescription(x, y, TileType.Clear, position);
                    availableTilesForObstacles.Add(newTile);
                }
                generatedTiles[y][x] = newTile;
            }
        }

        List<TileDescription> playerAvailableTiles = new List<TileDescription>(5);
        if (columns > 2)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        playerAvailableTiles.Add(generatedTiles[playerBaseTile.Y + j][playerBaseTile.X + i]);
                    }
                }
            }

            playerStartTile = playerAvailableTiles[Random.Range(0, playerAvailableTiles.Count)];
            generatedTiles[playerStartTile.Y][playerStartTile.X].Type = TileType.PlayerStart;
            playerStartTile.Type = TileType.PlayerStart;
        }
        else
        {
            playerStartTile = new TileDescription(1, 0, TileType.PlayerStart, generatedTiles[1][0].Position);
            generatedTiles[1][playerBaseTile.X].Type = TileType.PlayerStart;
        }
        
        ClearAroundBase(1);
        AddObstacles();
        InstantiateTiles();
        CreateWorld();

        S_OverlayCanvas.Hide();
    }


    void ClearAroundBase(int n)
    {
        for (int i = -n; i <= n; i++)
        {
            for (int j = -n; j <= n; j++)
            {
                for (int k = 0; k < availableTilesForObstacles.Count; k++)
                {
                    if (availableTilesForObstacles[k].X == playerBaseTile.X + i && availableTilesForObstacles[k].Y == playerBaseTile.Y + j)
                    {
                        availableTilesForObstacles.RemoveAt(k);
                    }
                }
            }
        }
    }

    void AddObstacles()
    {
        while (totalObstaclesSpawned < totalObstaclesToSpawn && availableTilesForObstacles.Count > 0)
        {
            int randomTileIndex = (int)Random.Range(0, availableTilesForObstacles.Count);
            var randomTile = availableTilesForObstacles[randomTileIndex];
            if (Random.value > 0.5f)
            {
                generatedTiles[randomTile.Y][randomTile.X].Type = TileType.Wall;
                for (int i = 0; i < enemyStartTiles.Length; i++)
                {
                    if (enemyStartTiles[i] == null)
                    {
                        break;
                    }

                    var path = checkAlgorythm.FindPath(GeneratedTiles, enemyStartTiles[i], playerBaseTile);
                    if (path == null)
                    {
                        generatedTiles[randomTile.Y][randomTile.X].Type = TileType.Destructible;
                        break;
                    }
                }
            }
            else
            {
                generatedTiles[randomTile.Y][randomTile.X].Type = TileType.Destructible;
            }
            availableTilesForObstacles.RemoveAt(randomTileIndex);
            totalObstaclesSpawned++;
        }
    }

    void InstantiateTiles()
    {
        S_BoardTile[][] boardTiles = new S_BoardTile[GeneratedTiles.Length][];
        for (int i = 0; i < GeneratedTiles.Length; i++)
        {
            boardTiles[i] = new S_BoardTile[GeneratedTiles[0].Length];
        }
        GameObject newTile = null;
        for (int i = 0; i < GeneratedTiles.Length; i++)
        {
            for (int j = 0; j < GeneratedTiles[i].Length; j++)
            {
                switch (GeneratedTiles[i][j].Type)
                {
                    case TileType.Clear:
                        newTile = Instantiate(clearBoardTilePrefab, GeneratedTiles[i][j].Position, Quaternion.identity, transform);
                        break;
                    case TileType.Base:
                        newTile = Instantiate(baseBoardTilePrefab, GeneratedTiles[i][j].Position, Quaternion.identity, transform);
                        break;
                    case TileType.PlayerStart:
                        newTile = Instantiate(clearBoardTilePrefab, GeneratedTiles[i][j].Position, Quaternion.identity, transform);
                        break;
                    case TileType.EnemyStart:
                        newTile = Instantiate(clearBoardTilePrefab, GeneratedTiles[i][j].Position, Quaternion.identity, transform);
                        break;
                    case TileType.Wall:
                        newTile = Instantiate(wallBoardTilePrefab, GeneratedTiles[i][j].Position, Quaternion.identity, transform);
                        break;
                    case TileType.Destructible:
                        newTile = Instantiate(destructibleBoardTilePrefab, GeneratedTiles[i][j].Position, Quaternion.identity, transform);
                        break;
                    default:
                        break;
                }
                boardTiles[i][j] = newTile.GetComponent<S_BoardTile>();
                boardTiles[i][j].Init(generatedTiles[i][j]);
            }
        }
    }

    //void AddToAvailableListCheck(TileDescription tileDescription)
    //{
    //    int lenght = restrictedTilesForObstacles.Count;
    //    for (int i = 0; i < lenght; i++)
    //    {
    //        if (tileDescription.Position == restrictedTilesForObstacles[i].Position)
    //        {
    //            return;
    //        }
    //    }
    //    availableTilesForObstacles.Add(tileDescription);
    //}

    void CreateWorld() 
    {
        float addForOddNumbersX = 0;
        float addForOddNumbersZ = 0;
        if (startColumns % 2 != 0)
        {
            addForOddNumbersX = 0.5f;
        }
        if (startRows % 2 != 0)
        {
            addForOddNumbersZ = 0.5f;
        }

        tiledPlane.transform.position += Vector3.right * (0.5f + addForOddNumbersX) + Vector3.forward * (0.5f + addForOddNumbersZ);

        float sidePosition = startColumns / 2;
        for (int i = 0; i < generatedTiles.Length; i++)
        {
            Instantiate(sideTile, new Vector3(sidePosition + addForOddNumbersX, 0f, generatedTiles[i][0].Position.z - 0.5f), Quaternion.identity, sidesParent.transform);
            Instantiate(sideTile, new Vector3(-sidePosition - addForOddNumbersX, 0f, generatedTiles[i][0].Position.z + 0.5f), Quaternion.Euler(0, 180, 0), sidesParent.transform);
        }

        float upDownPosition = startRows / 2;
        for (int i = 0; i < generatedTiles[0].Length; i++)
        {
            Instantiate(sideTile, new Vector3(generatedTiles[0][i].Position.x + 0.5f, 0f, upDownPosition + addForOddNumbersZ), Quaternion.Euler(0, 270, 0), sidesParent.transform);
            Instantiate(sideTile, new Vector3(generatedTiles[0][i].Position.x - 0.5f, 0f, -upDownPosition - addForOddNumbersZ), Quaternion.Euler(0, 90, 0), sidesParent.transform);
        }

        Instantiate(cornerTile, new Vector3(sidePosition + addForOddNumbersX, 0f, upDownPosition + addForOddNumbersZ), Quaternion.Euler(0, 0, 0), sidesParent.transform);
        Instantiate(cornerTile, new Vector3(-sidePosition - addForOddNumbersX, 0f, upDownPosition + addForOddNumbersZ), Quaternion.Euler(0, 270, 0), sidesParent.transform);
        Instantiate(cornerTile, new Vector3(sidePosition + addForOddNumbersX, 0f, -upDownPosition - addForOddNumbersZ), Quaternion.Euler(0, 90, 0), sidesParent.transform);
        Instantiate(cornerTile, new Vector3(-sidePosition - addForOddNumbersX, 0f, -upDownPosition - addForOddNumbersZ), Quaternion.Euler(0, 180, 0), sidesParent.transform);

        float positionX = sidePosition + addForOddNumbersX + sidesPlane.transform.localScale.x / 2 + 1;
        Instantiate(sidesPlane, new Vector3(positionX, sidesPlane.transform.position.y, 0), Quaternion.Euler(90, 0, 0), sidesParent.transform);
        Instantiate(sidesPlane, new Vector3(-positionX, sidesPlane.transform.position.y, 0), Quaternion.Euler(90, 0, 0), sidesParent.transform);

        float positionZ = upDownPosition + addForOddNumbersZ + upDownPlane.transform.localScale.z / 2 + 1;
        var upPlane = Instantiate(upDownPlane, new Vector3(0, upDownPlane.transform.position.y, positionZ), Quaternion.Euler(90, 0, 0), sidesParent.transform);
        var downPlane = Instantiate(upDownPlane, new Vector3(0, upDownPlane.transform.position.y, -positionZ), Quaternion.Euler(90, 0, 0), sidesParent.transform);
        
        upPlane.transform.localScale = new Vector3(startColumns + 2, upPlane.transform.localScale.y, upPlane.transform.localScale.z);
        downPlane.transform.localScale = new Vector3(startColumns + 2, downPlane.transform.localScale.y, downPlane.transform.localScale.z);
    }

    public static TileDescription GetNeighbourTile(TileDescription currentTile, Direction direction)
    {
        switch(direction)
        {
            case Direction.Up:
                {
                    if(currentTile.Y < instance.generatedTiles.Length - 1)
                    {
                        return instance.generatedTiles[currentTile.Y + 1][currentTile.X];
                    }
                }
                break;
            case Direction.Down:
                {
                    if (currentTile.Y > 0)
                    {
                        return instance.generatedTiles[currentTile.Y - 1][currentTile.X];
                    }
                }
                break;
            case Direction.Right:
                {
                    if (currentTile.X < instance.generatedTiles[0].Length - 1)
                    {
                        return instance.generatedTiles[currentTile.Y][currentTile.X + 1];
                    }
                }
                break;
            case Direction.Left:
                {
                    if (currentTile.X > 0)
                    {
                        return instance.generatedTiles[currentTile.Y][currentTile.X - 1];
                    }
                }
                break;
        }

        return null;
    }


    public static void DestructibleDestroyed(int x, int y) 
    {
        instance.generatedTiles[x][y].Type = TileType.Clear;
    }
}

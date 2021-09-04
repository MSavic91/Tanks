using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_WorldManager : MonoBehaviour
{
    private static S_WorldManager instance;

#pragma warning disable 0649
    [SerializeField] private Camera mainCamera;
    [SerializeField] private S_MazeGenerator generator;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
#pragma warning restore 0649

    private S_Tank playerTank;
    private List<S_Tank> enemyTanks;
    public static bool BaseDestroyed;

    public static bool IsPlayerAlive() 
    {
        return instance.playerTank.Alive;
    }

    public UnityEvent MazeChange;

    public void Init()
    {
        if (instance != null)
        {
            if (instance == this)
            {
                return;
            }
            Destroy(gameObject);
            return;
        }
        instance = this;
        BaseDestroyed = false;
        generator.Init();
        SetCameraSize(generator.Rows, generator.Columns);
        enemyTanks = new List<S_Tank>();
        MazeChange = new UnityEvent();

        generator.ForceGenerate();

        for(int i = 0; i < generator.GeneratedTiles.Length; i++)
        {
            for(int j = 0; j < generator.GeneratedTiles[i].Length; j++)
            {
                var tile = generator.GeneratedTiles[i][j];
                if(tile.Type == TileType.PlayerStart)
                {
                    playerTank = Instantiate(playerPrefab, tile.Position, Quaternion.identity).GetComponent<S_PlayerTank>();
                    playerTank.Init(generator.GeneratedTiles, tile, generator.PlayerBaseTile, Direction.Up, this);
                    //tile.IsTaken = true;
                }
                else if (tile.Type == TileType.EnemyStart)
                {
                    var tank = Instantiate(enemyPrefab, tile.Position, Quaternion.identity).GetComponent<S_EnemyTank>();
                    tank.Init(generator.GeneratedTiles, tile, generator.PlayerBaseTile, Direction.Down, this);
                    tile.IsTaken = true;
                    enemyTanks.Add(tank);
                }
            }
        }
    }

    public static void MazeChangeInvokeEvent() 
    {
        instance.MazeChange.Invoke();
    }

    public TileDescription[][] GetGeneratedTiles()
    {
        return generator.GeneratedTiles;
    }

    public void SetCameraSize(float rows, float columns)
    {
        float size = 0;
        if ((columns - 1) / 10 < (rows - 4) / 16)
        {
            //4 - 20 rows
            //0 - 16 (+4)

            //6 - 15 size
            //0 - 9 (+6)
            //(rows - 4) / 16) * 9 + 6
            size = ((rows - 4) / 16) * 9 + 6;
        }
        else
        {
            //1 - 11 col
            //6 - 15 size
            size = ((columns - 1) / 10) * 9 + 6;
        }
        mainCamera.orthographicSize = size;
    }

    public static void CheckIfGameIsOverStatic() 
    {
        instance.CheckIfGameIsOver();
    }

    public void CheckIfGameIsOver() 
    {
        if (!IsPlayerAlive())
        {
            S_GameManager.MissionOver.Invoke();
            return;
        }
        else
        {
            foreach (var enemy in enemyTanks)
            {
                if (enemy.Alive)
                {
                    return;
                }
            }
            S_GameManager.MissionOver.Invoke();
        }
    }

}

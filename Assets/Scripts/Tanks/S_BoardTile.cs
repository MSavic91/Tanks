using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDescription
{
    public int X;
    public int Y;
    public TileType Type;
    public Vector3 Position;
    public S_BoardTile Holder;
    public bool IsTaken = false;

    public TileDescription(int x, int y, TileType type, Vector3 position)
    {
        this.X = x;
        this.Y = y;
        this.Type = type;
        this.Position = position;

        //if (type == TileType.Destructible)
        //{
        //    this.Cost = 1;
        //}
        //else
        //{
        //    this.Cost = 10;
        //}
        //SetDistance(X, Y);
    }

    public bool Passable
    {
        get => (Type & (TileType.Clear | TileType.EnemyStart | TileType.PlayerStart)) > 0;
    }

    public bool Destructible
    {
        get => Type == TileType.Destructible;
    }

    public bool CanBePassedThrough
    {
        get => Passable || Destructible;
    }

    // A Star

    public int Cost { get; set; }
    public int Distance { get; set; }
    public int CostDistance => Cost + Distance;
    public TileDescription Parent { get; set; }

    public TileDescription(int x, int y, TileDescription currentTile, TileType tileType)
    {
        this.X = x;
        this.Y = y;
        this.Parent = currentTile;
        this.Cost = currentTile.Cost + 1;
        this.Type = tileType;
        if (tileType == TileType.Destructible)
        {
            this.Cost += 1;
        }
        SetDistance(X, Y);
    }

    public void SetDistance(int targetX, int targetY)
    {
        this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
    }

    public Direction DirectionToTile(TileDescription other)
    {
        if(other.X == this.X)
        {
            if(other.Y < this.Y)
            {
                return Direction.Up;
            }
            else
            {
                return Direction.Down;
            }
        }
        else if(other.X < this.X)
        {
            return Direction.Right;
        }
        else
        {
            return Direction.Left;
        }
    }
}

public class S_BoardTile : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private TileType type;
    [Header("Destructible")]
    [SerializeField] private GameObject destructible;
    [SerializeField] private GameObject destructibleBroken;
    [SerializeField] private BoxCollider boxCollider;
    [Range(1, 4)]
    [SerializeField] private int hardness;
#pragma warning restore 0649

    private int row;
    private int column;
    public TileDescription TileDescription;

    public void Init(TileDescription desc)
    {
        this.TileDescription = desc;
        desc.Holder = this;
        this.row = desc.X;
        this.column = desc.Y;
    }

    public void MorphType(TileType newType)
    {
        this.type = newType;
    }

    public TileType Type { get => type; }
    public int Row { get => row; }
    public int Column { get => column; }


    private int hits = 0;

    public void HitDestructibleTile()
    {
        hits += 1;
        if (hardness <= hits)
        {
            destructible.SetActive(false);
            destructibleBroken.SetActive(false);
            boxCollider.enabled = false;
            S_AudioManager.PlaySound(Sounds.DestroyableDestroy);
            MorphType(TileType.Clear);
            S_MazeGenerator.DestructibleDestroyed(Column, Row);
            S_WorldManager.MazeChangeInvokeEvent();
        }
        else if (hardness / 2 <= hits)
        {
            if (destructible.activeSelf)
            {
                S_AudioManager.PlaySound(Sounds.DestroyableDestroy);
                destructible.SetActive(false);
                destructibleBroken.SetActive(true);
            }
        }
    }

    public void HitBase()
    {
        hits += 1;
        if (hardness <= hits)
        {
            destructible.SetActive(false);
            destructibleBroken.SetActive(true);
            S_AudioManager.PlaySound(Sounds.BaseDestroy);
            StartCoroutine(BaseDestroyIE());
        }
    }

    IEnumerator BaseDestroyIE() 
    {
        yield return new WaitForSeconds(1f);
        S_WorldManager.BaseDestroyed = true;
        S_GameManager.MissionOver.Invoke();
    }
}

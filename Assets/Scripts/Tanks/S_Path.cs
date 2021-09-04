using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Path
{
    private enum Direction
    {
        Vertical,
        Horizontal
    }

    //public List<S_BoardTile> Tiles;
    public List<TileDescription> Tiles;
    public int Length;
    public int Turns;

    //public S_Path(List<S_BoardTile> tiles)
    public S_Path(List<TileDescription> tiles)
    {
        this.Tiles = tiles;
        this.Length = tiles.Count;

        // Length 1 paths have no turns
        if(Length > 1)
        {
            var dir = Direction.Horizontal;
            if(tiles[1].X == tiles[0].X)
            {
                dir = Direction.Vertical;
            }

            for(int i = 2; i < Length; i++)
            {
                if(dir == Direction.Vertical)
                {
                    if(tiles[i].X != tiles[i - 1].X)
                    {
                        dir = Direction.Horizontal;
                        Turns++;
                    }
                }
                else
                {
                    if (tiles[i].Y != tiles[i - 1].Y)
                    {
                        dir = Direction.Vertical;
                        Turns++;
                    }
                }
            }
        }
    }
}

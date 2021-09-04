using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TankSearchAlgorythm : S_SearchAlgorythm
{
    private List<TileDescription> allVisitedTiles;
    private int shortestPath = 0;
    private int attempts = 0;

    Dictionary<TileDescription, int> minStepsToReach;

    public List<TileDescription> AllCheckedTiles { get => allVisitedTiles; }

    public override S_Path FindPath(TileDescription[][] tiles, TileDescription start, TileDescription end)
    {
        List<S_BoardTile> visitedTiles = new List<S_BoardTile>();

        minStepsToReach = new Dictionary<TileDescription, int>(tiles.Length * tiles[0].Length);
        allVisitedTiles = new List<TileDescription>(tiles.Length * tiles[0].Length);
        shortestPath = 0;
        attempts = 0;
        return FindPathImpl(new List<TileDescription>(), tiles, start, end);
    }

    private S_Path FindPathImpl(List<TileDescription> visitedTiles, TileDescription[][] allTiles, TileDescription currentTile, TileDescription destination)
    {
        // Fail safe just in case, you never know.
        attempts++;
        if (attempts > 10000)
        {
            return null;
        }

        //if (currentTile == null)
        //{
        //    return null;
        //}

        var localVisitedTiles = new List<TileDescription>(visitedTiles);
        localVisitedTiles.Add(currentTile);

        if (minStepsToReach.ContainsKey(currentTile))
        {
            if (localVisitedTiles.Count >= minStepsToReach[currentTile])
            {
                return null;
            }
            else
            {
                minStepsToReach[currentTile] = localVisitedTiles.Count;
            }
        }
        else
        {
            minStepsToReach.Add(currentTile, localVisitedTiles.Count);
        }

        allVisitedTiles.Add(currentTile);

        //TODO: optimizing for cutting of longer paths
        if (localVisitedTiles.Count > shortestPath && shortestPath != 0)
        {
            return null;
        }
        List<TileDescription> neighbours = new List<TileDescription>(4);
        // Adding left neighbour
        if (currentTile.X > 0)
        {
            var neighbour = allTiles[currentTile.Y][currentTile.X - 1];
            if(neighbour.CanBePassedThrough || neighbour.Type == TileType.Base)
            {
                if(!localVisitedTiles.Contains(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
        }
        // Adding right neighbour
        if (currentTile.X < allTiles[0].Length - 1)
        {
            var neighbour = allTiles[currentTile.Y][currentTile.X + 1];
            if ((neighbour.CanBePassedThrough || neighbour.Type == TileType.Base))
            {
                if (!localVisitedTiles.Contains(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
        }
        // Adding top neighbour
        if (currentTile.Y < allTiles.Length - 1)
        {
            var neighbour = allTiles[currentTile.Y + 1][currentTile.X];
            if ((neighbour.CanBePassedThrough || neighbour.Type == TileType.Base))
            {
                if (!localVisitedTiles.Contains(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
        }
        // Adding bottom neighbour
        if (currentTile.Y > 0)
        {
            var neighbour = allTiles[currentTile.Y - 1][currentTile.X];
            if ((neighbour.CanBePassedThrough || neighbour.Type == TileType.Base))
            {
                if (!localVisitedTiles.Contains(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
        }

        Vector2 destinationVector = new Vector2(destination.X, destination.Y);

        int neighboursCount = neighbours.Count;
        if (neighboursCount > 1)
        {
            neighbours.Sort((x, y) => (Vector2.Distance(new Vector2(x.X, x.Y), destinationVector).CompareTo(Vector2.Distance(new Vector2(y.X, y.Y), destinationVector))));
        }

        // If there are no free neighbours to visit then we're stuck
        if (neighboursCount == 0)
        {
            return null;
        }

        S_Path result = null;

        for (int i = 0; i < neighboursCount; i++)
        {
            if (neighbours[i].X == destination.X && neighbours[i].Y == destination.Y)
            {
                localVisitedTiles.Add(neighbours[i]);
                result = new S_Path(localVisitedTiles);
                break;
            }
           
            var potentialPath = FindPathImpl(localVisitedTiles, allTiles, neighbours[i], destination);
            if (potentialPath != null)
            {
                if (result == null || potentialPath.Length < result.Length)
                {
                    result = potentialPath;
                }
                else if(potentialPath.Turns < result.Turns && potentialPath.Length == result.Length)
                {
                    result = potentialPath;
                }

                if (result != null && (result.Length < shortestPath || shortestPath == 0))
                {
                    shortestPath = result.Length;
                }
            }
        }

        if (result != null && (result.Length < shortestPath || shortestPath == 0))
        {
            shortestPath = result.Length;
        }

        return result;
    }

}

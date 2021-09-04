using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_A_StarSearchAlgorythm : S_SearchAlgorythm
{
    private List<TileDescription> allVisitedTiles;
    public List<TileDescription> AllCheckedTiles { get => allVisitedTiles; }

    public override S_Path FindPath(TileDescription[][] tiles, TileDescription start, TileDescription end)
    {
        return FindPathImpl(tiles, start, end);
    }

    private S_Path FindPathImpl(TileDescription[][] allTiles, TileDescription currentTile, TileDescription destination)
    {
        var visitedTiles = new List<TileDescription>();
        allVisitedTiles = new List<TileDescription>();
        var activeTiles = new List<TileDescription>();

        currentTile.SetDistance(destination.X, destination.Y);
        activeTiles.Add(currentTile);
        
        S_Path result = null;
        //This is where we created the map from our previous step etc. 
        while (allTiles.Any())
        {
            if (activeTiles.Count == 0)
            {
                //Debug.Log("null!!!  No Path Found! ");
                return null;
            }
            var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();
            if (checkTile.X == destination.X && checkTile.Y == destination.Y)
            {
                //We can actually loop through the parents of each tile to find our exact path which we will show shortly. 
                List<TileDescription> path = new List<TileDescription>();
                //We found the destination and we can be sure (Because the the OrderBy above)
                //That it's the most low cost option. 
                var tile = checkTile;
                while (true)
                {
                    if (allTiles[tile.Y][tile.X].CanBePassedThrough || allTiles[tile.Y][tile.X].Type == TileType.Base)
                    {
                        path.Add(allTiles[tile.Y][tile.X]);
                    }
                    tile = tile.Parent;
                    if (tile == null)
                    {
                        var lenght = path.Count - 1;
                        var newPath = new List<TileDescription>();
                        for (int i = lenght; i >= 0; i--)
                        {
                            newPath.Add(path[i]);
                        }
                        
                        result = new S_Path(newPath);
                        return result;
                    }
                }
            }
            visitedTiles.Add(checkTile);
            allVisitedTiles.Add(allTiles[checkTile.Y][checkTile.X]);
            activeTiles.Remove(checkTile);

            var walkableTiles = GetWalkableTiles(allTiles, checkTile, destination);

            foreach (var walkableTile in walkableTiles)
            {
                //We have already visited this tile so we don't need to do so again!
                if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    continue;

                //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                {
                    var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                    if (existingTile.CostDistance > checkTile.CostDistance)
                    {
                        activeTiles.Remove(existingTile);
                        activeTiles.Add(walkableTile);
                    }
                }
                else
                {
                    //We've never seen this tile before so add it to the list. 
                    activeTiles.Add(walkableTile);
                }
            }
        }

        //Debug.Log("No Path Found!");
        return null;
    }

    private static List<TileDescription> GetWalkableTiles(TileDescription[][] allTiles, TileDescription currentTileWeAreOn, TileDescription targetTile)
    {
        var maxX = allTiles[0].Length - 1;
        var maxY = allTiles.Length - 1;

        var possibleTiles = new List<TileDescription>();
        if (currentTileWeAreOn.X - 1 >= 0)
        {
            possibleTiles.Add(new TileDescription(currentTileWeAreOn.X - 1, currentTileWeAreOn.Y, currentTileWeAreOn, allTiles[currentTileWeAreOn.Y][currentTileWeAreOn.X - 1].Type));
        }
        if (currentTileWeAreOn.X + 1 <= maxX)
        {
            possibleTiles.Add(new TileDescription(currentTileWeAreOn.X + 1, currentTileWeAreOn.Y, currentTileWeAreOn, allTiles[currentTileWeAreOn.Y][currentTileWeAreOn.X + 1].Type));
        }
        if (currentTileWeAreOn.Y - 1 >= 0)
        {
            possibleTiles.Add(new TileDescription(currentTileWeAreOn.X, currentTileWeAreOn.Y - 1, currentTileWeAreOn, allTiles[currentTileWeAreOn.Y - 1][currentTileWeAreOn.X].Type));
        }
        if (currentTileWeAreOn.Y + 1 <= maxY)
        {
            possibleTiles.Add(new TileDescription(currentTileWeAreOn.X, currentTileWeAreOn.Y + 1, currentTileWeAreOn, allTiles[currentTileWeAreOn.Y + 1][currentTileWeAreOn.X].Type));
        }

        return possibleTiles.Where(tile => allTiles[tile.Y][tile.X].Type != TileType.Wall).ToList();
    }
}

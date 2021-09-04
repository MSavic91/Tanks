using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_EnemyController : MonoBehaviour
{
    S_EnemyTank controlledTank;
    TileDescription[][] generatedTiles;
    S_SearchAlgorythm search;
    TileDescription playerBaseTile;

    public void Init(S_EnemyTank tank, TileDescription[][] generatedTiles, TileDescription playerBaseTile)
    {
        controlledTank = tank;
        this.playerBaseTile = playerBaseTile;

        //search = new S_TankSearchAlgorythm();
        search = new S_A_StarSearchAlgorythm();
        CalculateNewPath(generatedTiles);
        //var newPath = search.FindPath(generatedTiles, controlledTank.CurrentTile , playerBaseTile);

        //controlledTank.SetAllVisited(((S_A_StarSearchAlgorythm)search).AllCheckedTiles);
        //controlledTank.SetPath(newPath);
    }


    public void CalculateNewPath(TileDescription[][] generatedTiles) 
    {
        this.generatedTiles = generatedTiles;
        var newPath = search.FindPath(generatedTiles, controlledTank.CurrentTile, playerBaseTile);
        controlledTank.SetAllVisited(((S_A_StarSearchAlgorythm)search).AllCheckedTiles);
        controlledTank.SetPath(newPath);
    }


}

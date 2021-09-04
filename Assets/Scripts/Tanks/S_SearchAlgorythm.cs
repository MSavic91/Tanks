using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Search Algorythm classes should implement method to return path from start location to end location based on generated tiles,
/// or null in case no path was found.
/// </summary>
public abstract class S_SearchAlgorythm
{
    public abstract S_Path FindPath(TileDescription[][] tiles, TileDescription start, TileDescription end);
}

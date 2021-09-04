using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_EnemyTank : S_Tank
{
#pragma warning disable 0649
    [SerializeField] private ParticleSystem searchSystem;
    [SerializeField] private ParticleSystem solutionSystem;
    [SerializeField] private GameObject isTaken;
    [SerializeField] private GameObject isNotTaken;
#pragma warning restore 0649

    private bool pathRecalcualte = false;
    private bool deactivateOld = false;
    private S_EnemyController controller;

    private S_Path currentPath;
    private List<TileDescription> allCheckedTiles;

    public TileDescription CurrentTile { get => currentTile; }

    private static List<S_EnemyTank> currentlyShowingPath;

    public override void Init(TileDescription[][] generatedTiles, TileDescription startTile, TileDescription playerBaseTile, Direction facing, S_WorldManager worldManager)
    {
        base.Init(generatedTiles, startTile, playerBaseTile, facing, worldManager);

        controller = gameObject.AddComponent<S_EnemyController>();
        controller.Init(this, generatedTiles, playerBaseTile);

        rotationTime = 0.1f;
        moveTime = 0.5f;

        MovedToNewTile.AddListener(DeactivateTileOld);
        worldManager.MazeChange.AddListener(RecalculatePath);

        StartCoroutine(EnemyFireIE());
    }

    public void SetAllVisited(List<TileDescription> allTiles)
    {
        this.allCheckedTiles = allTiles;
    }

    public void SetPath(S_Path path)
    {
        this.currentPath = path;

        if (currentlyShowingPath == null)
        {
            currentlyShowingPath = new List<S_EnemyTank>();
        }
        currentlyShowingPath.Add(this);

        ShowActivePath();

        StartCoroutine(DecisionTreeIE());
    }

    public void ShowActivePath()
    {
        bool showPaths = true;
        if (PlayerPrefs.HasKey(S_MM_Settings.SHOW_PATHS))
        {
            showPaths = PlayerPrefs.GetInt(S_MM_Settings.SHOW_PATHS) == 1;
        }
        if (showPaths)
        {
            StartCoroutine(ShowPathIE());
        }
    }

    IEnumerator ShowPathIE()
    {
        while (currentlyShowingPath[0] != this)
        {
            yield return new WaitForSecondsRealtime(0.2f);
        }
        Time.timeScale = 0f;
        if (currentPath != null)
        {
            int c = allCheckedTiles.Count;
            for (int i = 0; i < c; i++)
            {
                searchSystem.transform.position = allCheckedTiles[i].Holder.transform.position;
                searchSystem.Play();
                yield return new WaitForSecondsRealtime(0.1f);
            }

            yield return new WaitForSecondsRealtime(1f);

            for (int i = 0; i < currentPath.Length; i++)
            {
                solutionSystem.transform.position = currentPath.Tiles[i].Holder.transform.position;
                solutionSystem.Play();
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        Time.timeScale = 1f;
        currentlyShowingPath.RemoveAt(0);
    }

    IEnumerator DecisionTreeIE()
    {
        while (alive)
        {
            yield return new WaitForSeconds(moveTime + 0.01f);
            if (currentPath != null && !IsMoving())
            {
                int indInPath = currentPath.Tiles.IndexOf(currentTile);
                indInPath++;
                if (indInPath < currentPath.Length)
                {
                    var nextTile = currentPath.Tiles[indInPath];
                    if (nextTile.DirectionToTile(currentTile) != Facing)
                    {
                        FaceDirection(nextTile.DirectionToTile(currentTile));
                        yield return new WaitForSeconds(rotationTime + 0.01f);
                    }
                    if (indInPath < currentPath.Length - 1)
                    {
                        if (currentPath.Tiles[indInPath].IsTaken == false && currentPath.Tiles[indInPath].Type != TileType.Destructible)
                        {
                            currentPath.Tiles[indInPath].IsTaken = true;
                            MoveToTile(currentPath.Tiles[indInPath]);
                            deactivateOld = true;
                        }
                        if (pathRecalcualte)
                        {
                            yield return new WaitUntil(() => !IsMoving());
                            yield return new WaitUntil(() => !deactivateOld);
                            StartCoroutine(RecalculatePathIE());
                            break;
                        }
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        if (S_WorldManager.BaseDestroyed || S_GameManager.goToMenu)
        {
            return;
        }
        int indInPath = currentPath.Tiles.IndexOf(currentTile);
        currentPath.Tiles[indInPath].IsTaken = false;
        if (IsMoving())
        {
            if (indInPath + 1 < currentPath.Tiles.Count)
            {
                currentPath.Tiles[indInPath + 1].IsTaken = false;
            }
        }
        else
        {
            if (indInPath - 1 < currentPath.Tiles.Count)
            {
                currentPath.Tiles[indInPath - 1].IsTaken = false;
            }
        }
    }

    private void DeactivateTileOld() 
    {
        int indInPath = currentPath.Tiles.IndexOf(currentTile);
        if (indInPath - 1 < currentPath.Tiles.Count)
        {
            currentPath.Tiles[indInPath - 1].IsTaken = false;
        }
        deactivateOld = false;
    }

    
    private void RecalculatePath() 
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        pathRecalcualte = true;
    }

    IEnumerator RecalculatePathIE() 
    {
        yield return new WaitForSeconds(0f);
        //Debug.Log("Recalcualte path");
        controller.CalculateNewPath(worldManager.GetGeneratedTiles());
        pathRecalcualte = false;
    }

    IEnumerator EnemyFireIE() 
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        if (alive)
        {
            Fire();
            StartCoroutine(EnemyFireIE());
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(PathFinder))]
[RequireComponent(typeof(SpawningEnemy))]
public class GameManager : MonoBehaviour
{
    //variable
    [Header("Map info")]
    //pour le future faire un read only avec editor pour les variavle suivante
    int row;
    int col;
    static internal List<GameTiles> spawnTiles;
    internal GameTiles endTile;
    internal GameTiles[,] currentGameTiles;
    internal GameTiles focusTile;
    internal bool deleteTower = false;

    //script ref
    SpawningEnemy spawningEnemy;
    PathFinder pathFinder;
    NewPathFinder newPathFinder;
    FlowFieldPathfinding flowFlieldPathfinding;

    //singletone
    public static GameManager Instance;

    public GameTiles TargetTile { get; internal set; }

    private void Awake()
    {
        //Creation of singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //get les script
        spawningEnemy = GetComponent<SpawningEnemy>();
        pathFinder = GetComponent<PathFinder>();
        newPathFinder = GetComponent<NewPathFinder>();
        flowFlieldPathfinding = GetComponent<FlowFieldPathfinding>();
        MapLoading mapLoading = GetComponentInChildren<MapLoading>();

        //set les valeurs
        currentGameTiles = MapLoading.currentGameTiles;
        row = mapLoading.RowCount;
        col = mapLoading.ColCount;
        spawnTiles = MapLoading.spawnTile;
        endTile = MapLoading.endTile;

        //set les valeur pour le path
        //plus tard mettre lorsque le joueur commence partie
        SetPath();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            deleteTower = !deleteTower;
        }
    }

    public void SetPath()
    {
        //pathFinder.SetValue(currentGameTiles, spawnTiles, endTile, col, row);
        //pathFinder.SetPath();

        //newPathFinder.SetValue(currentGameTiles, spawnTiles, endTile, col, row);
        //newPathFinder.FindPathAStar();

        flowFlieldPathfinding.SetFlowField(currentGameTiles);
    }

    public List<GameTiles> EnemySetPath(GameTiles pos, GameTiles end)
    {
        return pathFinder.EnemySetPath(pos, end);
    }

    public List<int> GetPathLeght()
    {
        //return pathFinder.SetTempPath();
        SetAStarPath();
        return newPathFinder.GetLength();
    }

    private void SetAStarPath()
    {
        newPathFinder.SetValue(currentGameTiles, spawnTiles, endTile, col, row);
        newPathFinder.FindPathAStar();
    }

    internal void UpdateFlowFieldAround(Vector2Int position)
    {
        flowFlieldPathfinding.UpdateFlowFieldAround(position, currentGameTiles);
    }
}

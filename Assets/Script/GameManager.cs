using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlowFieldPathfinding))]
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
        flowFlieldPathfinding = GetComponent<FlowFieldPathfinding>();
        MapLoading mapLoading = GetComponentInChildren<MapLoading>();

        //set les valeurs
        currentGameTiles = MapLoading.currentGameTiles;
        row = mapLoading.RowCount;
        col = mapLoading.ColCount;
        spawnTiles = MapLoading.spawnTile;
        endTile = MapLoading.endTile;

        SetPath();

    }

    public void SetGameTiles(GameTiles[,] gameTile)
    {
        currentGameTiles = gameTile;
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
        flowFlieldPathfinding.SetFlowField(currentGameTiles);
    }

    internal void UpdateFlowFieldAround(Vector2Int position)
    {
        flowFlieldPathfinding.UpdateFlowFieldAround(position, currentGameTiles);
    }
}

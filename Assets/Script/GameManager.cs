using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    GameTiles focusTile;
    internal bool deleteTower = false;

    //script ref
    SpawningEnemy spawningEnemy;
    PathFinder pathFinder;

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

        //set les valeurs
        currentGameTiles = MapLoading.currentGameTiles;
        row = MapLoading.RowCount;
        col = MapLoading.ColCount;
        spawnTiles = MapLoading.spawnTile;
        endTile = MapLoading.endTile;

        //set les valeur pour le path
        //plus tard mettre lorsque le joueur commence partie
        pathFinder.SetValue(currentGameTiles, spawnTiles, endTile, col, row);
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
    { pathFinder.SetPath(); }

    public List<GameTiles> EnemySetPath(GameTiles pos, GameTiles end)
    {
        return pathFinder.EnemySetPath(pos, end);
    }

    public List<int> GetTempPathLeght()
    {
        return pathFinder.SetTempPath();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathFinder))]
[RequireComponent (typeof(SpawningEnemy))]
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

    //script ref
    SpawningEnemy spawningEnemy;
    PathFinder pathFinder;

    private void Start()
    {
        //get les script
        spawningEnemy= GetComponent<SpawningEnemy>();
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
        pathFinder.SetPath();
       
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

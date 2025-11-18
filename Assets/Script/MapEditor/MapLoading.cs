#if UNITY_EDITOR
using UnityEditor;

using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Rendering;
using static UnityEngine.Rendering.DebugUI.Table;

public class MapLoading : MonoBehaviour
{
    public enum TileSets
    {
        Grass,
        StonePath,
        StoneBluiding,
    }

    [Header("Map Creator")]
    [SerializeField] internal int RowCount = 20;
    [SerializeField] internal int ColCount = 35;

    [SerializeField] internal GameObject gameTilePrefab;
    static internal GameTiles[,] currentGameTiles;
    internal Vector2 spawnPoint;
    internal Vector2 endPoint;
    static internal Vector2 target;
    static internal List<GameTiles> spawnTile = new List<GameTiles>();
    static internal GameTiles endTile;

    [SerializeField] List<GameObject> mapList = new List<GameObject>();
    static public GameObject MapObject;
    public GameObject mapPrefab;
    Map currentMap;

    [Header("Map loading")]
    [SerializeField] internal int mapIndex = 0;
    internal string mapName = "n/a";
    //string tagBackground = "Background";
    string tagMap = "Map";

    //variable pour le tilling des niveau
    [Header("Tile set")]
    internal int natureLayerIndex = 0;
    internal int terrainLayerIndex = 0;
    internal int object3DLayerIndex = 0;
    internal int decorationlayerIndex = 0;
    internal int moduleLayerIndex = 0;

    internal TileSets tilesets;

    private void Awake()
    {
        CreateMap();
        target = new Vector2(RowCount / 2, ColCount / 2);
    }

    public void CreateMap()
    {
        RemoveMap();

        if (mapList == null || mapList.Count == 0)
        {
            Debug.LogError("mapList vide.");
            return;
        }

        // 1) Instancier le prefab de map
        mapPrefab = mapList[mapIndex];
        var mapPrefabAsset = mapList[mapIndex];
        GameObject mapInstance = Instantiate(mapPrefabAsset);
        mapInstance.tag = tagMap;         // <- IMPORTANT
        MapObject = mapInstance;

        // 2) Utiliser le composant Map de l'INSTANCE (pas du prefab)
        currentMap = MapObject.GetComponent<Map>();

        // 3) Dimensions déclarées par la Map
        RowCount = currentMap.row;
        ColCount = currentMap.col;



        // 4) Charger la grille depuis JSON (remplit currentMap.map[,])
        //currentMap.LoadJson();
        mapName = currentMap.mapName;


        //currentMap.LoadMap();

        // S'assurer que currentMap.map existe et a la bonne taille
        Debug.Log($"la map et est-elle vide? : {currentMap.mapTiles == null}");
        if (currentMap.map == null
            || currentMap.map.GetLength(0) != RowCount
            || currentMap.map.GetLength(1) != ColCount)
        {
            Debug.LogWarning("Map char[,] manquante ou taille incorrecte. Création d'une grille vide ' ' par défaut.");
            currentMap.map = new char[RowCount, ColCount];
            for (int x = 0; x < RowCount; x++)
                for (int y = 0; y < ColCount; y++)
                    currentMap.map[x, y] = ' ';

            //load la map Char[,] en GameTile[,]
            currentMap.LoadMap();
            currentGameTiles = currentMap.mapTiles;
            Debug.Log($"la map et est-elle vide apres le load? : {currentMap.mapTiles == null}");

        }
        else
        {
            currentGameTiles = currentMap.mapTiles;
            Debug.Log(currentGameTiles);
        }


        Debug.LogWarning($"Map Tiles : {currentGameTiles}");
        if (currentMap)

            //// S'assurer que la liste des spawns est initialisée
            //if (spawnTile == null) spawnTile = new List<GameTiles>();

            if (currentMap.mapTiles != null)
            {
                for (int x = 0; x < RowCount; x++)
                {
                    for (int y = 0; y < ColCount; y++)
                    {
                        if (currentGameTiles[x, y].IsSpawn)
                        {
                            spawnTile.Add(currentGameTiles[x, y]);
                        }
                    }
                }
            }

        // 5) Créer/peupler les tiles enfants sous mapInstance
        if (currentMap.mapTiles == null)
        {
            currentGameTiles = new GameTiles[currentMap.row, currentMap.col];

            for (int x = 0; x < RowCount; x++)
            {
                for (int y = 0; y < ColCount; y++)
                {
                    GameTiles gt = currentGameTiles[x, y];

                    //if(currentMap.mapTiles != null)
                    //{
                    //    gt = currentGameTiles[x, y];
                    //}
                    //else
                    //{
                    //    gt = null;
                    //}

                    // ---> Le point clé : instancier si gt est null (et pas selon currentMap.map)
                    if (gt == null)
                    {
                        var spawnPosition = new Vector3(x, 0, y);
                        var tileGO = Instantiate(gameTilePrefab, spawnPosition, Quaternion.Euler(90, 0, 0), mapInstance.transform);

                        gt = tileGO.GetComponent<GameTiles>();
                        if (gt == null)
                        {
                            Debug.LogError("gameTilePrefab n'a pas de composant GameTiles.");
                            continue;
                        }

                        currentGameTiles[x, y] = gt;
                        gt.SetComponent(x, y);
                        gt.X = x; gt.Y = y;
                        gt.name = $"tile {x}/{y}";
                    }

                    // Ici gt est garanti non-null
                    char c = currentMap.map[x, y];
                    switch (c)
                    {
                        case 'W': gt.TurnBloced(); break;
                        case 'S': gt.TurnSlow(); break;
                        case 'D': gt.TurnDamaging(); break;
                        case 'E':
                            spawnPoint = new Vector2(gt.transform.position.x, gt.transform.position.z);
                            spawnTile.Add(gt);
                            gt.TurnSpawn();
                            break;
                        case 'F':
                            endPoint = new Vector2(gt.transform.position.x, gt.transform.position.z);
                            endTile = gt;
                            gt.TurnEnd();
                            break;
                        default:
                            gt.TurnBlank(); // 'B' par défaut
                            break;
                    }
                }
            }
        }

        // (optionnel) garder un pointeur côté Map
        //currentMap.mapTiles = currentGameTiles;
    }

    void SafeDestroy(GameObject go)
    {
#if UNITY_EDITOR
        if (Application.isPlaying) Destroy(go);
        else DestroyImmediate(go);
#else
    Destroy(go);
#endif
    }

    internal void RemoveMap()
    {
        mapName = "n/a";
        spawnPoint = Vector2.zero;
        endPoint = Vector2.zero;

        foreach (var tile in GameObject.FindGameObjectsWithTag("Tile"))
            SafeDestroy(tile);

        foreach (var map in GameObject.FindGameObjectsWithTag(tagMap))
            SafeDestroy(map);

        PathFinder.pathToGoal.Clear();
        spawnTile.Clear();
    }

    internal void SaveMap()
    {
        currentMap.mapTiles = currentGameTiles;
        //mapList[mapIndex].GetComponent<Map>().SaveMap(currentGameTiles);
        mapPrefab.GetComponent<Map>().SaveMap(currentGameTiles);

        //section pour save en prefab permetent de mettre plus d'object
        const string defaultDir = "Assets/Map";
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Prefab",
            mapPrefab.name,
            "prefab",
            "Choisis l'emplacement",
            defaultDir
        );

        PrefabUtility.SaveAsPrefabAssetAndConnect(MapObject, path, InteractionMode.AutomatedAction);

        RefreshMap();
    }

    internal void RefreshMap()
    {
        RemoveMap();
        CreateMap();
    }

    internal void ResizeMap()
    {
        currentMap.ResizeMap(RowCount, ColCount);
        RefreshMap();
    }

    internal void SetPath()
    {
        //PathFinder pathFinder = FindAnyObjectByType<PathFinder>().GetComponent<PathFinder>();
        //pathFinder.SetValue(currentGameTiles, spawnTile, endTile, ColCount, RowCount);
        //pathFinder.SetPath();

        //NewPathFinder newPathFinder = FindAnyObjectByType<NewPathFinder>().GetComponent<NewPathFinder>();
        //newPathFinder.SetValue(currentGameTiles, spawnTile, endTile, ColCount, RowCount);
        //newPathFinder.FindPathAStar();

        FlowFieldPathfinding flowFieldPathfinding = FindAnyObjectByType<FlowFieldPathfinding>().GetComponent<FlowFieldPathfinding>();
        flowFieldPathfinding.SetFlowField(currentGameTiles);
    }

    internal void EditMap()
    {

    }

    internal int getRow()
    {
        return RowCount;
    }

    internal int getCol()
    {
        return ColCount;
    }

    internal IEnumerable<object> GetSpawnPoint()
    {
        return spawnTile;
    }

    internal GameTiles[,] GetCurrentMap()
    {
        return currentGameTiles;
    }
}
#endif
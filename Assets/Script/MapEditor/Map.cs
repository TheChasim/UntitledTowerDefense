using System;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

[System.Serializable]
public class MapData
{
    public string mapName;
    public CharArray2DContainer mapComtainer;
    public int col;
    public int row;
    public char[,] map;
}


[Serializable]
public class CharArray2D
{
    public char[] row;
}

[Serializable]
public class CharArray2DContainer
{
    public CharArray2D[] map2D;
}

public class Map : MonoBehaviour
{
    //variable
    [SerializeField] internal GameObject prefab;
    [SerializeField] internal string mapName;
    //internal GameTiles[,] mapTile;
    internal char[,] map;

    [SerializeField] internal int col;
    [SerializeField] internal int row;
    //{
    //    {' ', ' ', ' ', ' ', 'X', 'X', 'X', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'X'}, //1
    //    {' ', ' ', ' ', ' ', ' ', 'X', 'X', ' ', ' ', ' ', ' ', 'X', ' ', ' ', ' ', 'X'}, //2
    //    {' ', 'X', ' ', ' ', ' ', 'X', 'X', ' ', 'X', ' ', ' ', 'X', ' ', ' ', ' ', 'X'}, //3
    //    {' ', ' ', 'X', ' ', ' ', ' ', 'X', ' ', 'X', ' ', ' ', ' ', ' ', ' ', ' ', ' '}, //4
    //    {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'X', ' ', ' ', 'F'}, //5
    //    {' ', 'X', ' ', ' ', ' ', ' ', 'X', ' ', ' ', 'X', ' ', ' ', 'X', ' ', ' ', ' '}, //6
    //    {' ', ' ', ' ', ' ', ' ', ' ', 'X', ' ', ' ', 'X', ' ', ' ', ' ', ' ', ' ', 'X'}, //7
    //    {'S', ' ', ' ', ' ', ' ', 'X', 'X', ' ', ' ', 'X', ' ', ' ', ' ', ' ', ' ', 'X'}, //8
    //    {' ', 'X', ' ', ' ', ' ', 'X', 'X', ' ', ' ', 'X', ' ', ' ', ' ', ' ', ' ', 'X'}, //9
    //    {' ', ' ', ' ', ' ', ' ', 'X', 'X', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'X'}  //10
    //    //  1    2    3    4    5    6    7   8     9    10   11  12    13   14   15   16
    //};
    public void SaveMap(GameTiles[,] CurrentMapTile)
    {
        col = map.GetLength(1);
        row = map.GetLength(0);


        map = new char[row, col];

        for (int x = 0; x < map.GetLength(1); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                Debug.Log($"{x} | {y}");
                var tile = CurrentMapTile[y, x].GetComponent<GameTiles>();

                //Debug.Log($"Processing tile at ({y}, {x})");
                if (tile.IsBloced)
                {
                    map[y, x] = 'W';
                    Debug.Log("Wall");
                }
                else if (tile.IsDamaging)
                {
                    map[y, x] = 'D';
                    Debug.Log("Fire");
                }
                else if (tile.IsSlowing)
                {
                    map[y, x] = 'S';
                    Debug.Log("Water");
                }
                else if (tile.IsSpawn)
                {
                    map[y, x] = 'E';
                    Debug.Log("Spawing");
                }
                else if (tile.IsEnd)
                {
                    map[y, x] = 'F';
                    Debug.Log("End");
                }
                else
                {
                    map[y, x] = ' ';
                }
            }
        }

        SaveInJson();
    }



    private void SaveInJson()
    {
        //transforme la map pour le json
        CharArray2DContainer container = new CharArray2DContainer();
        container.map2D = new CharArray2D[map.GetLength(0)];

        //passe dans le tableu a deux dimension et le met dans une list de list
        for (int i = 0; i < map.GetLength(0); i++)
        {
            container.map2D[i] = new CharArray2D();
            container.map2D[i].row = new char[map.GetLength(1)];

            for (int j = 0; j < map.GetLength(1); j++)
            {
                container.map2D[i].row[j] = map[i, j];
            }
        }

        //assigne les valeur a la classe pour le Json
        MapData mapData = new MapData
        {
            mapName = this.mapName,
            col = this.col,
            row = this.row,
            mapComtainer = container,
          
        };

        //transforme les info en string pour le Json et le true permet de mettre pour faciliter la lecture des donnees
        string json = JsonUtility.ToJson(mapData, true);


        // Sauvegarder le JSON dans un fichier
        //chemein creer avec le nom de la map comme nom du  fichier JSON
        string path = Path.Combine(Application.dataPath, "Saves", mapName);
        //transforme la string d'info en document JSON
        File.WriteAllText(path, json);

        //afiche le chemin de la save dans la console
        Debug.Log("Data saved to " + path);
    }


    internal void LoadJson()
    {
        //vas chercher le chemin
        string path = Path.Combine(Application.dataPath, "Saves", mapName);

        if (File.Exists(path))
        {
            //lie les infos dans le fichier JSON
            string json = File.ReadAllText(path);

            //cree une class temp pour lire les info du JSON
            MapData mapData = JsonUtility.FromJson<MapData>(json);
            
            //set les valeur du JSON dans les variable
            this.mapName = mapData.mapName;
            this.row = mapData.row;
            this.col = mapData.col;
            map = new char[row, col];

            //assosi les info de la list de list dans le tableau a deux dimension
            for(int i =0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
                {
                    Debug.Log($"x : {i} y : {j}");
                    map[i,j] = mapData.mapComtainer.map2D[i].row[j];
                }

            }

            //affiche la reussite du load et le chemin de la load
            Debug.Log("Data load from " + path);
        }
        else
        {
            Debug.LogError("Save file not found at " + path);
        }
    }

    internal void ResizeMap(int newRow, int newCol)
    {

        this.row = newRow;
        this.col = newCol;

        map = new char[row, col];
        //MapLoading.currentGameTiles = new GameTiles[newRow, newCol];

        GameTiles[,] newGameTiles = new GameTiles[newRow, newCol];

        for (int x = 0; x < map.GetLength(1); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                if(newGameTiles[y, x] == null)
                {
                    break;
                }

                Debug.Log($"{x} | {y}");
                var tile = newGameTiles[y, x].GetComponent<GameTiles>();


                //Debug.Log($"Processing tile at ({y}, {x})");
                if (tile.IsBloced)
                {
                    map[y, x] = 'W';
                    Debug.Log("Wall");
                }
                else if (tile.IsDamaging)
                {
                    map[y, x] = 'D';
                    Debug.Log("Fire");
                }
                else if (tile.IsSlowing)
                {
                    map[y, x] = 'S';
                    Debug.Log("Water");
                }
                else if (tile.IsSpawn)
                {
                    map[y, x] = 'E';
                    Debug.Log("Spawing");
                }
                else if (tile.IsEnd)
                {
                    map[y, x] = 'F';
                    Debug.Log("End");
                }
                else
                {
                    map[y, x] = ' ';
                }
            }
        }

        SaveInJson();
    }



    //ne fonctionne pas mais garder au cas ou. can be useful later

    //a utiliser avec SaveInPrefab()
    //#if UNITY_EDITOR
    //using UnityEditor;
    //#endif 

    //private void SaveInPrefab()
    //{
    //    var changedMap = MapLoading.MapObject.GetComponent<Map>();

    //    changedMap.map = this.map;
    //    changedMap.col = this.col;
    //    changedMap.row = this.row;

    //    EditorUtility.SetDirty(MapLoading.MapObject);
    //    PrefabUtility.SavePrefabAsset(MapLoading.MapObject);
    //    AssetDatabase.SaveAssets();
    //}

    //List<List<char>> ConvertToSerializable(char[,] array)
    //{
    //    List<List<char>> list = new List<List<char>>();
    //    for (int i = 0; i < array.GetLength(0); i++)
    //    {
    //        List<char> row = new List<char>();
    //        for (int j = 0; j < array.GetLength(1); j++)
    //        {
    //            row.Add(array[i, j]);
    //        }
    //        list.Add(row);
    //    }
    //    return list;
    //}

    //char[,] ConvertFromSerializable(List<List<char>> list)
    //{
    //    Debug.Log(list.Count);  
    //    int rows = list.Count;
    //    int cols = list[0].Count;
    //    char[,] array = new char[rows, cols];

    //    for (int i = 0; i < rows; i++)
    //    {
    //        for (int j = 0; j < cols; j++)
    //        {
    //            array[i, j] = list[i][j];
    //        }
    //    }
    //    return array;
    //}
}

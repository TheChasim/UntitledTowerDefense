using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] internal string mapName;
    //internal GameTiles[,] mapTile;
    internal char[,] map;
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
        for (int x = 0; x < map.GetLength(1); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                var tile = CurrentMapTile[y, x].GetComponent<GameTiles>();

                Debug.Log($"Processing tile at ({y}, {x})");
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
                else
                {
                    map[y, x] = ' ';
                }
            }
        }

        foreach (char c in map)
        {
            Debug.Log(c);
        }
    }
}

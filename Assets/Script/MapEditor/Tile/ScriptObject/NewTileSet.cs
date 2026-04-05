using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileSet", menuName = "Level Editor/Tile Set/New Tile Set")]
public class NewTileSet : ScriptableObject
{
    public TileSet3x3[] tiles;
}


[Serializable]
public class TileSet3x3
{
    public Sprite sprite;

    public int upLeft = 0;
    public int upMid = 0;
    public int upRight = 0;

    public int midLeft = 0;
    public int midMid = 0;
    public int midRight = 0;

    public int downLeft = 0;
    public int downMid = 0;
    public int downRight = 0;
}
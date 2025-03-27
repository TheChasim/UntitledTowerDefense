using UnityEngine;

[CreateAssetMenu(fileName = "NewTileSet", menuName = "Level Editor/Tile Set")]
public class TileSet : ScriptableObject
{
    public Sprite[] tiles;
}

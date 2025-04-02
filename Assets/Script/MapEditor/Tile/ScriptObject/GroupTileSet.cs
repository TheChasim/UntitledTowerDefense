using UnityEngine;

[CreateAssetMenu(fileName = "NewTileSet", menuName = "Level Editor/Goup Tile Set")]
public class GroupTileSet : ScriptableObject
{
    public TileSet[] groupSet;
}

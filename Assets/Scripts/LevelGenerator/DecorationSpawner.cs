using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Stores the condition used when spawning a decoration
/// </summary>
public class DecorationSpawner : ScriptableObject
{ 
    /// <summary>
    /// Spawns a decoration when the spawn chance is met
    /// </summary>
    /// <param name="decoPrefab">The prefab of the decoration</param>
    /// <param name="tiles">A list of all the tiles in the scene</param>
    /// <param name="row">The current row that the decorations are being spawned on</param>
    /// <param name="rowIndex">The index of the current row that the decorations are being spawed on for indexing into tiles.
    /// The index points to the left most tile in the row. Positive increments will read to the right of that tile</param>
    /// <param name="rowStep">The step between each row. Equal to the number of lanes</param>
    /// <param name="tileSize">The size in world units of each tile</param>
    /// <param name="generateOffset">The offset from 0,0, 0 all the tiles were spawned from</param>
    public virtual void Spawn(GameObject decoPrefab, in List<TileInfo> tiles, int row, uint rowIndex, uint rowStep, TileSize tileSize, Vector3 generateOffset)
    {
#if DEBUG
        Debug.LogError("This is empty and does nothing! Replace me with something that will actually spawn decoration: " + decoPrefab.name);
#endif
    }
}
/// <summary>
/// Stores tile size information for DecorationCondition
/// </summary>
public struct TileSize
{
    /// <summary>
    /// The length of the tile along the z axis
    /// </summary>
    public float m_length;
    /// <summary>
    /// The height of the tile along the y axis
    /// </summary>
    public float m_height;
    /// <summary>
    /// The width of the tile along the x axis
    /// </summary>
    public float m_width;
    /// <summary>
    /// Constructor for TileSize
    /// </summary>
    /// <param name="length">The length of the tile along the z axis</param>
    /// <param name="height">The height of the tile along the y axis</param>
    /// <param name="width">The width of the tile along the x axis</param>
    public TileSize(float length, float height, float width)
    {
        m_length = length;
        m_width = width;
        m_height = height;
    }
}


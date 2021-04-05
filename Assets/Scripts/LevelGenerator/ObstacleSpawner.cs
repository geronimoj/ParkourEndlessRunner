using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Used for spawning obstacles
/// </summary>
public class ObstacleSpawner : ScriptableObject
{
    /// <summary>
    /// Spawns the obstacle
    /// </summary>
    /// <param name="prefab">The prefab for the obstacle</param>
    /// <param name="tiles">All the tiles for checking neighbouring tiles</param>
    /// <param name="curTile">A reference to the current tile for ease of access</param>
    /// <param name="curTileIndex">The index of curTile in tiles</param>
    /// <param name="tileCenterOnSurface">The point, in the center of curTile, on the top of curTile. Ease of access</param>
    /// <param name="tileWidth">The width of the tile</param>
    /// <param name="tileLength">The length of the tile</param>
    /// <param name="tileHeight">The height of the tile</param>
    /// <param name="numberOfLanes">The number of lanes in the level</param>
    public virtual void Spawn(GameObject prefab, in List<TileInfo> tiles, uint curTileIndex, Vector3 tileCenterOnSurface, float tileWidth, float tileLength, float tileHeight, uint numberOfLanes)
    {
        Debug.LogError("Using empty spawner. Create a spawner for the decoration: " + prefab.name);
    }
}

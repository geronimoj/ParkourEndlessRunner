using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The default spawning behaviour of an obstacle.
/// Spawns the obstacle in the center of the tile
/// </summary>
[CreateAssetMenu(fileName ="Default", menuName = "Obstacle Spawner/Default", order = 0)]
public class DefaultObstacleSpawner : ObstacleSpawner
{
    /// <summary>
    /// Spawns the obstacle
    /// </summary>
    /// <param name="prefab">The prefab for the obstacle</param>
    /// <param name="tiles">All the tiles for checking neighbouring tiles</param>
    /// <param name="curTileIndex">The index of the current tile in tiles</param>
    /// <param name="tileCenterOnSurface">The point, in the center of the current tile, on the top of the current tile. Ease of access</param>
    /// <param name="tileWidth">The width of the tile</param>
    /// <param name="tileLength">The length of the tile</param>
    /// <param name="tileHeight">The height of the tile</param>
    /// <param name="numberOfLanes">The number of lanes in the level</param>
    public override void Spawn(GameObject prefab, in List<TileInfo> tiles, uint curTileIndex, Vector3 tileCenterOnSurface, float tileWidth, float tileLength, float tileHeight, uint numberOfLanes)
    {
        GameObject obj = Instantiate(prefab, tileCenterOnSurface, Quaternion.identity);

        tiles[(int)curTileIndex].AddObstacle(ref obj);
    }
}

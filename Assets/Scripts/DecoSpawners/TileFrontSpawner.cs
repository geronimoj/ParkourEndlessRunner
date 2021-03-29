using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The spawner for the curb decoration
/// </summary>
[CreateAssetMenu(fileName = "TileFrontSpawner", menuName = "DecoSpawners/TileFront", order = 0)]
public class TileFrontSpawner : DecorationSpawner
{
    /// <summary>
    /// Spawns the decoration at the end of the tile facing outwards
    /// </summary>
    /// <param name="decoPrefab">The prefab of the decoration</param>
    /// <param name="tiles">A list of all the tiles in the scene</param>
    /// <param name="row">The current row that the decorations are being spawned on</param>
    /// <param name="rowIndex">The index of the current row that the decorations are being spawed on for indexing into tiles.
    /// The index points to the left most tile in the row. Positive increments will read to the right of that tile</param>
    /// <param name="rowStep">The step between each row. Equal to the number of lanes</param>
    /// <param name="tileSize">The size in world units of each tile</param>
    /// <param name="generateOffset">The offset from 0,0, 0 all the tiles were spawned from</param>
    public override void Spawn(GameObject decoPrefab, in List<TileInfo> tiles, int row, uint rowIndex, uint rowStep, TileSize tileSize, Vector3 generateOffset)
    {   
        for (int lane = 0; lane < rowStep; lane++)
        {
            TileInfo prev = tiles[(int)rowIndex - (int)rowStep + lane];
            TileInfo cur = tiles[(int)rowIndex + lane];
            //Compare the height between the previous row
            if (prev.Height > cur.Height)
            {   //Calculate the height
                Vector3 pos = generateOffset;
                //The y is the height of the tile multiplied by the height of the lane. This gives us the center of that tile
                //We then add an additional half layerHeight to get the point on top of the layer
                pos.y += prev.Height * tileSize.m_height + (tileSize.m_height / 2);
                //Z is a similar story but we are doing this based on the current tile instead of the previous tile
                pos.z += row * tileSize.m_length - (tileSize.m_length / 2);
                //Calculate the x position starting from the front left corner of the tile
                pos.x = tileSize.m_width * lane + generateOffset.x;
                //Spawn the object
                GameObject decoration = Instantiate(decoPrefab, pos, Quaternion.LookRotation(Vector3.forward, Vector3.up));
                //Store the object so it gets deleted
                tiles[(int)rowIndex + lane].AddObstacle(ref decoration);
            }
        }
    }
}

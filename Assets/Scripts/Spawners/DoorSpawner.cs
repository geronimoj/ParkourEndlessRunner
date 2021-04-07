using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Spawns a door at the edge of a tile that enters indoor
/// </summary>
[CreateAssetMenu(fileName = "DoorSpawner", menuName = "DecoSpawners/Door", order = 1)]
public class DoorSpawner : DecorationSpawner
{
    public override void Spawn(GameObject decoPrefab, in List<TileInfo> tiles, int row, uint rowIndex, uint rowStep, TileSize tileSize, Vector3 generateOffset)
    {   //Loop over the row
        for (int i = 0; i < rowStep; i++)
        {   //Get the current and previous tiles for readonly purposes to save indexing
            TileInfo cur = tiles[(int)rowIndex + i];
            TileInfo prev = tiles[(int)rowIndex - (int)rowStep + i];
            //If the tiles are not both indoors or outdoors that means there is a transition between them
            if (cur.HasIndoors != prev.HasIndoors)
            {   //Calculate the position of the door
                Vector3 pos = generateOffset;
                //Determine if we should use the indoor height of this tile or the previous tile
                int height = prev.HasIndoors ? (int)prev.IndoorHeight : (int)cur.IndoorHeight;
                //The y is the height of the tile multiplied by the height of the lane. This gives us the center of that tile
                //We then add an additional half layerHeight to get the point on top of the layer
                pos.y += height * tileSize.m_height + (tileSize.m_height / 2);
                //Z is a similar story but we are doing this based on the current tile instead of the previous tile
                pos.z += row * tileSize.m_length - (tileSize.m_length / 2);
                //Calculate the x position starting from the front left corner of the tile
                pos.x = tileSize.m_width * i + generateOffset.x;
                //Spawn the door at the edge of the tile but looking back towards the player
                GameObject door = Instantiate(decoPrefab, pos, Quaternion.LookRotation(-Vector3.forward, Vector3.up));
                //Store the door on the tile
                tiles[(int)rowIndex + i].AddObstacle(ref door);
                //We the door is an exit, then we need to set the drop distance to 0 so the wall appears
                if (prev.HasIndoors)
                    for (int obj = 0; obj < prev.m_objectsOnTile.Count; obj++)
                        if (prev.m_objectsOnTile[obj].CompareTag("Indoors"))
                        {   //Undo the drop so the back wall spawns
                            Renderer r = tiles[(int)rowIndex - (int)rowStep + i].m_objectsOnTile[obj].GetComponentInChildren<Renderer>();
                            r.material.SetFloat("_Back", 0);
                            break;
                        }
            }
        }
    }
}

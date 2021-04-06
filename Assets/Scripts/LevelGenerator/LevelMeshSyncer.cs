using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the mesh of the level
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LevelMeshSyncer : MonoBehaviour
{
    Mesh mesh;

    List<Vector3> _verticies;
    List<Vector2> _uv;
    List<int> _triangles;


    Vector3 genOffset;

    private void Awake()
    {
        mesh = new Mesh();
        _verticies = new List<Vector3>();
        _triangles = new List<int>();
        _uv = new List<Vector2>();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = mesh;
        //Get the offset since we are assuming that a vertex 0,0 is at the position of the object
        genOffset = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelGenerator>().GenerateOffset;

        ResetMeshPosition();
    }
    /// <summary>
    /// Removes the front row of verticies.
    /// Also moves the mesh forwards to keep in sync with level geometry
    /// </summary>
    public void RemoveFrontRow()
    {
        int removed = 0;
        float step = 0;
        bool gotStep = false;
        Vector3 vPos;
        //Loop over the verticies and any below this value we want to remove
        //The verticies are expected to be ordered
        for (int i = 0; i < _verticies.Count; i++)
        {   //If the verticie is pretty much at 0, then get it removed. This is simply done for floating point error
            if (_verticies[i].z < 0.01f)
            {
                removed++;
                _verticies.RemoveAt(i);
                _uv.RemoveAt(i);
                //Move i backwards to make sure we don't miss any
                i--;
            }
            //If there are no more verticies to remove, then move the rest of the verticies towards 0 by step
            //So that the front row remains at z = 0 for subsequent calls
            else
            {   //If we don't yet have the step between tiles, get it as the z value of this verticie.
                //Its expected that the front most row of verticies are at z = 0.
                if (!gotStep)
                {
                    step = _verticies[i].z;
                    gotStep = true;
                }
                //Since we can't directly modify _verticies z value, we have to do this rediculous work around
                vPos = _verticies[i];
                vPos.z -= step;
                //Update the value
                _verticies[i] = vPos;
            }
        }

        //Update and remove the triangles
        for (int i = 0; i < _triangles.Count / 3; i++)
        {   //Reduce the triangles index by the removed factor to keep them synced up to the verticies array
            _triangles[i * 3] -= removed;
            _triangles[i * 3 + 1] -= removed;
            _triangles[i * 3 + 2] -= removed;

            //If any of the triangles are pointing at a negative index, delete all 3 triangles and decrement i by 1 to account for this index no-longer existing
            if (_triangles[i * 3] < 0 || _triangles[i * 3 + 1] < 0 || _triangles[i * 3 + 2] < 0)
            {   //Delete them
                _triangles.RemoveRange(i * 3, 3);
                i--;
            }
        }
        //Teleport us forwards by the same distance that we moved the verticies backwards
        //So they appear to remain in the exact same position
        vPos = transform.position;
        vPos.z += step;
        transform.position = vPos;
        //Sync our changes to the mesh
        SyncMesh();
    }
    /// <summary>
    /// Extends the mesh to include the tiles at the end of the mesh along the z axis.
    /// </summary>
    /// <param name="tiles">The tiles that should be added. This is expected to be of consistent length between calls.</param>
    /// <param name="prevTileHeights">The heights of the previous row. This is expected to be of equal length as tiles</param>
    /// <param name="tileWidth">The width of a tile</param>
    /// <param name="tileHeight">The height of a layer</param>
    /// <param name="tileLength">The length of the tile</param>
    /// <param name="maxHeight">The maximum height of the level</param>
    public void ExtendLevelMesh(TileInfo[] tiles, int[] prevTileHeights, float tileWidth, float layerHeight, float tileLength, int maxHeight)
    {
        bool firstCall = false;
        //If we don't have any verticies yet, we need to add some
        if (_verticies.Count == 0)
        {   //Add the left most verticie
            Vector3 pos = new Vector3(-0.5f * tileWidth, (tiles[0].Height + 0.5f) * layerHeight, -0.5f * tileLength);
            _verticies.Add(pos);
            //Now loop over and add one to the right of that for each lane
            for(int i = 0; i < tiles.Length; i++)
            {
                pos += Vector3.right * tileWidth;
                _verticies.Add(pos);
            }
            firstCall = true;
        }
        
        //This is done in multiple separate loops in order to keep the order the verticies exist in the list the same. This should be optimised later into 1 loop
        Vector3[] vertPoses = new Vector3[6];
        //Store the z value before we start adding more verticies
        float z = _verticies[_verticies.Count - 1].z;
        //Loop over the tiles and create new verticies, triangles and uv cords
        //If there has been a height change.
        for (int i = 0; i < tiles.Length; i++)
        {
            //Calculate the position of the tile and thus vertexs                                      //We use the last verticie to calculate the z position and is cheaper for calculating the furthest point on the tile
            Vector3 pos = new Vector3(tiles[i].Lane * tileWidth, (tiles[i].Height + 0.5f) * layerHeight, z);
            //If there has been a height change
            if (!firstCall && prevTileHeights[i] != tiles[i].Height)
                pos.y = (prevTileHeights[i] + 0.5f) * layerHeight;

            bool needLeft = false;
            //Now we want to find the index of the verticie that represents the start of this tile. This would have already been created.
            //We store it in backTileIndex
            vertPoses[0] = pos + Vector3.right * (tileWidth / 2);
            int backTileIndex = GetIndexOfEquivalentVertex(vertPoses[0]);

            if (backTileIndex == 0)
                Debug.LogError("Could not find previous tile");

            //If there is a height change but its not a ramp, we need to create two new verticies
            if (!firstCall && tiles[i].Height != prevTileHeights[i] && !tiles[i].IsRamp)
            {
                //Now we need to re-calculate pos! Woo hoo
                pos.y = (tiles[i].Height + 0.5f) * layerHeight;
                vertPoses[0] = pos + Vector3.right * (tileWidth / 2);
                //Calculate the position of the back left and right corner
                //Only need the left tile if the height of the tile to our left is not the same as ours. If it is, we can use their vertex
                if (i - 1 < 0 || tiles[i - 1].Height != tiles[i].Height)
                {
                    vertPoses[1] = pos - Vector3.right * (tileWidth / 2);
                    needLeft = true;
                }
                //Add the verticies
                //We know their indexes are Count - 1 and Count - 2 reguardless as to whether we needed the left tile or not
                if (needLeft)
                    //Add Left
                    _verticies.Add(vertPoses[1]);
                //Add Right
                _verticies.Add(vertPoses[0]);
                //DO LATER THE UV'S because they are annoying
                Debug.LogWarning("UV's NOT DONE. They have been dissabled");

                //Calculate the uv cords of the vertex
                //This is easy, the x flips between 0 and 1 for each row (otherwise we have to deal with duplicate verticies which isn't a BIG issue but its annoying)
                //The y is simply a value between 0 and 1. 0 being the bottom of the level, 1 being the top most layer of the level
                //Vector2 uv = new Vector2(0, tiles[i].Height / maxHeight - 1);
                //Since this is on the same row as the last uv's, we copy those
                //uv.x = _uv[_uv.Count - 1].x;

                //Because the verticies are no-longer ordered to save on time we need to make sure we have the correct index for the left vertex
                int leftIdex = GetIndexOfEquivalentVertex(vertPoses[0] - Vector3.right * tileWidth);

                //Create the triangles
                //Remember the verticies have to go around in a clockwise direction from the direction you are looking otherwise the normal calculations will not work
                //Add the left
                _triangles.Add(leftIdex);
                //Add the right
                _triangles.Add(_verticies.Count - 1);
                //Add the index of that verticie
                //Add the top right
                _triangles.Add(backTileIndex);
                //Now we need to find the third vertice
                //Create the second triangle.
                //Add the top left
                _triangles.Add(backTileIndex - 1);
                //Add the left
                _triangles.Add(leftIdex);
                //Add the top right
                _triangles.Add(backTileIndex);
                //We need to override backTileIndex to point to the new back right tile
                backTileIndex = _verticies.Count - 1;
            }
            needLeft = false;
            //Now we need to create the second set of verticies for the front of the tile
            //Because we can't be sure we didn't hit the previous if statement, its easier to just recalculate it again
            vertPoses[0] = pos + Vector3.right * (tileWidth / 2) + Vector3.forward * tileLength;
            //We only want to bother with the left vertex if the tile to our right hasn't already created one
            if (i - 1 < 0 || tiles[i - 1].Height != tiles[i].Height)
            {
                vertPoses[1] = pos - Vector3.right * (tileWidth / 2) + Vector3.forward * tileLength;
                needLeft = true;
            }
            //If its a ramp, we also need to move them up
            if (tiles[i].IsRamp)
            {
                vertPoses[0] += Vector3.up * layerHeight;
                if (needLeft)
                    vertPoses[1] += Vector3.up * layerHeight;
            }

            //Now we pop them in
            //Left first
            if (needLeft)
                _verticies.Add(vertPoses[1]);
            //Then right
            _verticies.Add(vertPoses[0]);
            //Now to create the triangles
            //Right
            _triangles.Add(_verticies.Count - 1);
            //Back Right
            _triangles.Add(backTileIndex);
            //Left
            _triangles.Add(_verticies.Count - 2);

            //Repeat for second triangle
            //Left
            _triangles.Add(_verticies.Count - 2);
            //Back Right
            _triangles.Add(backTileIndex);
            //Back Left
            _triangles.Add(backTileIndex - 1);
            /////////////////////////////////////////////////////
            //WE HAVE NO CREATED THE FLOOR & FRONT FACING WALLS// & all the verticies
            /////////////////////////////////////////////////////
        }

        //Sync the mesh over
        SyncMesh();
    }
    /// <summary>
    /// Returns the mesh back to 0,0,0 for level sync
    /// </summary>
    public void ResetMeshPosition()
    {
        transform.position = genOffset;
    }

    public void ResetMesh()
    {
        _verticies.Clear();
        _triangles.Clear();
        _uv.Clear();
    }
    /// <summary>
    /// Syncs the verticies and triangles to the mesh
    /// </summary>
    private void SyncMesh()
    {
        mesh.vertices = _verticies.ToArray();
        mesh.triangles = _triangles.ToArray();
        //mesh.uv = _uv.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private int GetIndexOfEquivalentVertex(Vector3 vertPos)
    {
        //We loop over the verticies starting from the back, going to the front
        for (int vert = _verticies.Count - 1; vert >= 0; vert--)
        {
            //And check if the x and z values are basically identical. If they are, we have found our vertex
            if (Conditions.InTolerance(_verticies[vert].x, vertPos.x, 0.01f) && Conditions.InTolerance(_verticies[vert].z, vertPos.z, 0.01f) && Conditions.InTolerance(_verticies[vert].y, vertPos.y, 0.01f))
            {
                //Store the index of the top right, 1 - that is the top left
                return vert;
            }
        }

        return 0;
    }
}

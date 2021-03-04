using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains all the information for the player. Contains code that should be used to Move and rotate the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    //Player Size Information
    public ColliderInfo colInfo;

    #region Movement
    public MovementDirection direction;
    /// <summary>
    /// Get or set the horizontal speed
    /// </summary>
    public float HozSpeed
    {
        get
        {
            return direction.HozSpeed;
        }
        set
        {
            direction.HozSpeed = value;
        }
    }
    /// <summary>
    /// Get or Set the vertical speed
    /// </summary>
    public float VertSpeed
    {
        get
        {
            return direction.VertSpeed;
        }
        set
        {
            direction.VertSpeed = value;
        }
    }
    /// <summary>
    /// Get or Set the total speed of the player
    /// </summary>
    public float TotalSpeed
    {
        get
        {
            return direction.TotalSpeed;
        }
        set
        {
            direction.TotalSpeed = value;
        }
    }
    /// <summary>
    /// Get or Set the direction of movement
    /// </summary>
    public Vector3 Direction
    {
        get
        {
            return direction.Direction;
        }
        set
        {
            direction.Direction = value;
        }
    }
    /// <summary>
    /// Get the direction and speed combined
    /// </summary>
    public Vector3 TotalVector
    {
        get
        {
            return direction.TotalVector;
        }
    }
    /// <summary>
    /// A call function for direction.
    /// See MovementDirection.SmoothDirection
    /// </summary>
    /// <param name="final"></param>
    /// <param name="time"></param>
    /// <param name="timer"></param>
    public void SmoothDirection(Vector3 final, float time, ref float timer)
    {
        direction.SmoothDirection(final, time, ref timer);
    }
    /// <summary>
    /// A call function for direction.
    /// See MovementDirection.SmoothDirection
    /// </summary>
    /// <param name="final"></param>
    /// <param name="velocity"></param>
    public void SmoothDirection(Vector3 final, float velocity)
    {
        direction.SmoothDirection(final, velocity);
    }
    #endregion

#if UNITY_EDITOR
    /// <summary>
    /// Used for debugging information
    /// </summary>
    private Vector3 test;
#endif
    /// <summary>
    /// The colliders that should be ignored when moving. This adds child colliders by default
    /// </summary>
    public List<Collider> collidersToIgnore = new List<Collider>();
    /// <summary>
    /// Is true if the controller is on the ground
    /// </summary>
    public bool OnGround
    {
        get { return colInfo.OnGround; }
    }
    /// <summary>
    /// Sets the transform and adds child colliders to colliders to ignore
    /// </summary>
    private void Start()
    {
        colInfo.SetTransform(transform);
        collidersToIgnore = new List<Collider>(GetComponentsInChildren<Collider>());
    }
    /// <summary>
    /// Moves the player. Contains all collision detection required
    /// </summary>
    /// <param name="dir">The direction with a magnitude of distance the player should be moved</param>
    /// <param name="cancelOnFail">If true, the player will not be moved if dir is changed</param>
    public void MoveTo(Vector3 dir, bool cancelOnFail = false)
    {   
#if UNITY_EDITOR
        test = dir;
#endif
        RaycastHit[] hits;
        //Make sure we have a direction to move in//
        if (dir != Vector3.zero)
        {
            //Get the collisions
            hits = MoveToRaycasts(dir, out int offsetIndex);

            bool updateHitInfo = false;
            int attempts = 0;
            //Loop through the collisions and adjust the current movement direction so that it is not pointing into any normals
            for (int i = 0; (i < hits.Length || updateHitInfo); i++)
            {   //Ensure we have hits as updateHitInfo can enter this loop even with a length of 0
                if (hits.Length == 0)
                    break;
                //Reset the index and don't update hit info. this is done so that we loop through all hits again
                if (updateHitInfo)
                {
                    updateHitInfo = false;
                    i = 0;
                }

                attempts++;
                if (attempts > 99)
                {
                    Debug.LogError("Could not solve for collision response. Not moving character");
                    return;
                }
                //Make sure aren't colliding with ourself
                if (hits[i].distance != 0 && hits[i].point != Vector3.zero)
                {
                    float dot = Vector3.Dot(hits[i].normal, dir);
                    //Make sure we are heading into the normal
                    if (dot < 0)
                    {   //If we can't move there, exit if this bool is true
                        if (cancelOnFail)
                            return;
                        //Get a vector from the point, to our raycast origin
                        Vector3 curNew = colInfo.GetCenteralPoint(hits[i].point, hits[i].normal) - hits[i].point;
                        //Get the dot product against the normal (its literally dot that we calculated earlier but positive)
                        dot = Vector3.Project(dir, hits[i].normal).magnitude;
                        //Subtract the dot product from are calculated vector to only get the overshooting amount.
                        dot -= Vector3.Project(curNew, hits[i].normal).magnitude;
                        float dist = colInfo.Radius + colInfo.CollisionOffset;
                        if (dot > -dist)
                        {
                            //We then apply this as a vector along the normal of the hit surface with a bit of extra stuff to adjust the movement vector away from the wall
                            dir += hits[i].normal * (dist + dot);
                            //Tell ourself to update the movement information
                            updateHitInfo = true;
                        }
                    }
                    //This is just to make sure that the last hit check actually gets updated
                    if (updateHitInfo)
                        hits = MoveToRaycasts(dir, out offsetIndex);
                }
                else
                {
                    if (i < offsetIndex)
                        //If this debug message is being called, we have a big problem
                        Debug.LogWarning("In collider: " + hits[i].transform.gameObject.name);
                    else
                        //This Debug message is ok to see
                        Debug.LogWarning("Too close to surface: " + hits[i].transform.gameObject.name);
                }
            }
        }
#if UNITY_EDITOR
        //Draw the movement and then move us along it
        Debug.DrawLine(colInfo.GetLowestPoint(), colInfo.GetLowestPoint() + dir, Color.magenta, 20f);
#endif
        transform.Translate(dir, Space.World);
        //Do a raycast down to check if we are on the ground
        hits = MoveToRaycasts(colInfo.GravityDirection * 1e-3f, out int _);
        //Loop through the downwards raycast results and check if any of them meet the on ground conditions
        for (int i = 0; i < hits.Length; i++)
            if (colInfo.ValidSlope(hits[i].normal))
            {   //If we just entered onGround, then change our movement vector to be that direction
                //This exists to help with exiting from slopes we can't stand on. However, in turn, its causes other issues with going up small un-ramped bumps, 
                //causing the player to drift on them. So we also check that the point we hit is below us, so that small ledges at our height aren't affected
                if (!colInfo.OnGround && (hits[i].point - colInfo.GetLowestPoint()).y < 0)
                    direction.HozDirection = dir.normalized;
                //We found one of them so set us to be on the ground, and set previous ground
                colInfo.OnGround = true;
                return;
            }
        //Set us to not be on the ground. We can only hit this if the previous checks failed
        colInfo.OnGround = false;
    }
    /// <summary>
    /// Performs the raycasts to detect collisions for MoveTo
    /// </summary>
    /// <param name="dir">The direction & magnitude of the raycasts</param>
    /// <param name="offsetIndex">The index that the output hitInfo is from the second raycast instead of the first</param>
    /// <returns>An array containing hitInfo from two raycasts. One with radius, the other with radius + offset</returns>
    private RaycastHit[] MoveToRaycasts(Vector3 dir, out int offsetIndex)
    {   //Raycast for the players regular collider
        RaycastHit[] regular = ColliderInfo.CastAll(colInfo, dir);
        //Raycast for the players collider with the offset
        RaycastHit[] withOffset = ColliderInfo.CastAllWithOffset(colInfo, dir);
        //Sort them by distance. Closest ones should be checked first
        System.Array.Sort(regular, Conditions.CompareDist);
        System.Array.Sort(withOffset, Conditions.CompareDist);
        //Remove the colliders we are supposed to ignore
        //This is inefficient and should be solved
        //Combine the raycast results
        List<RaycastHit> total = new List<RaycastHit>();
        offsetIndex = 0;
        for (int i = 0; i < regular.Length + withOffset.Length; i++)
        {
            if (i >= regular.Length && !collidersToIgnore.Contains(withOffset[i - regular.Length].collider))
                total.Add(withOffset[i - regular.Length]);
            else if (i < regular.Length && !collidersToIgnore.Contains(regular[i].collider))
            {
                total.Add(regular[i]);
                offsetIndex += 1;
            }
        }
        return total.ToArray();
    }

    #region UNITYEDITOR
#if UNITY_EDITOR
    /// <summary>
    /// Draw debugging information
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        colInfo.SetTransform(transform);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + test);
        Gizmos.DrawWireSphere(colInfo.GetUpperPoint() + test, colInfo.Radius);
        Gizmos.DrawWireSphere(colInfo.GetLowerPoint() + test, colInfo.Radius);
    }
#endif
    #endregion
}
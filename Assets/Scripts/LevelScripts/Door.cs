using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls how a door should react to a player entering the door
/// </summary>
[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    /// <summary>
    /// The scale of the AABB used to cut a hole in the wall for the door
    /// </summary>
    [Tooltip("The scale of the AABB used to cut a hole in the wall for the door. Defaults to 0,0,0")]
    [SerializeField]
    private Vector3 _AabbScale = new Vector3();
    /// <summary>
    /// Makes sure the trigger is set
    /// </summary>
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    /// <summary>
    /// Starts the door opening and sets the AABB for cutting a hole in the wall for the door.
    /// And toggles the players LayerMasks for what is considered ground
    /// </summary>
    /// <param name="other">The collider of the object entering the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController pc = Player.player.GetComponent<PlayerController>();

        bool indoors = pc.colInfo.ground == LayerMask.GetMask("Default", "Indoors");
        //If the player is indoors, set them to be outdoors
        if (indoors)
            pc.colInfo.ground = LayerMask.GetMask("Default", "Indoors", "OutDoors");
        //Otherwise set them to in indoors
        else
            pc.colInfo.ground = LayerMask.GetMask("Default", "Indoors");
        //Set the AABB to cut a hole in the wall
        Shader.SetGlobalVector("_AABBP1", transform.position + _AabbScale);
        Shader.SetGlobalVector("_AABBP2", transform.position - _AabbScale);
        //Open the door
        transform.parent.GetComponentInChildren<DoorOpen>().Open();
    }
    /// <summary>
    /// Undos any changes to the AABB to avoid is existing between multiple loops
    /// </summary>
    /// <param name="other">The collider of the target</param>
    private void OnTriggerExit(Collider other)
    {   //Undo our AABB so that it doesn't persist between enters and exits and looks weird
        Shader.SetGlobalVector("_AABBP1", Vector3.zero);
        Shader.SetGlobalVector("_AABBP2", Vector3.zero);
    }
}

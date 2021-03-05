using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Gives the player bonus points upon passing through the trigger
/// </summary>
[RequireComponent(typeof(Collider))]
public class GiveBonusPoints : MonoBehaviour
{
    /// <summary>
    /// The bonus points to give the player upon entering
    /// </summary>
    public float bonusPoints = 100;
    /// <summary>
    /// Sets the collider to be a trigger
    /// </summary>
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Player.player.Score += bonusPoints;
    }
}

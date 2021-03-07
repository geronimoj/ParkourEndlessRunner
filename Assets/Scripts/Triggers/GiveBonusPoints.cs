using UnityEngine;
/// <summary>
/// Gives the player bonus points upon passing through the trigger
/// </summary>
[RequireComponent(typeof(Collider))]
public class GiveBonusPoints : MonoBehaviour
{
    /// <summary>
    /// The bonus points to give to the players score upon entering the trigger
    /// </summary>
    [Tooltip("The points gained from performing a parkour move on an obstacle.")]
    [SerializeField]
    private float _bonusPoints = 100;
    /// <summary>
    /// The bonus points to give to the players score upon entering the trigger
    /// </summary>
    public float BonusPoints => _bonusPoints;
    /// <summary>
    /// Sets the collider to be a trigger just in case its not done by a developer
    /// </summary>
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    /// <summary>
    /// Adds bonusPoints to the players score if the player enters the trigger
    /// </summary>
    /// <param name="other">The collider of the object that entered the trigger</param>
    private void OnTriggerEnter(Collider other)
    {   //If its not a player, return
        if (!other.CompareTag("Player"))
            return;
        //Increment the players store
        Player.player.Score += BonusPoints;
    }
}

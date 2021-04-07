using UnityEngine;
/// <summary>
/// Opens a door. Is expected to be placed on the hinge object of the door
/// </summary>
public class DoorOpen : MonoBehaviour
{
    /// <summary>
    /// Is the door currently opening
    /// </summary>
    private bool open = false;
    /// <summary>
    /// How quickly doors open
    /// </summary>
    [SerializeField]
    private float _openTime = 0.5f;
    /// <summary>
    /// A timer for the duration it takes to open doors
    /// </summary>
    private float t_openTime = 0;
    /// <summary>
    /// The previous/current angle the door is open
    /// </summary>
    private float _prevAngle;
    /// <summary>
    /// Opens the door
    /// </summary>
    void Update()
    {
        if (open)
        {   //Move the timer forward
            t_openTime += Time.deltaTime;
            t_openTime = Mathf.Clamp(t_openTime, 0, _openTime);
            //Stop opening the door once the timer completes
            if (t_openTime == _openTime)
                open = false;
            //Get the angle the door should be at at this time interval
            float expected = -90 * (t_openTime / _openTime);
            //Subtract our current angle from the expected angle to get the change in angle we need to move this frame
            expected -= _prevAngle;
            //Rotate around y by the given value
            transform.Rotate(0, expected, 0, Space.Self);
            //Increment the previous angle so that its the current angle
            _prevAngle += expected;
        }
    }
    /// <summary>
    /// Starts the door open
    /// </summary>
    [ContextMenu("Open")]
    public void Open()
    {
        open = true;
        _prevAngle = 0;
        t_openTime = 0;
    }
}

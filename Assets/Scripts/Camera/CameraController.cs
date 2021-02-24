using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the rotation of the camera and player
/// </summary>
public class CameraController : MonoBehaviour
{   
    /// <summary>
    /// The sensitivity of rotation
    /// </summary>
    [Tooltip("The sensitivity of rotation")]
    public float speed = 1000;
    /// <summary>
    /// The transform of the players origin or the equivalent location of the character mover
    /// </summary>
    [Tooltip("The transform of the players origin or the equivalent location of the character mover")]
    public Transform parentTrans = null;
    /// <summary>
    /// The transform of the head of the model
    /// </summary>
    [Tooltip("The transform of the head of the model")]
    public Transform trueParent = null;
    /// <summary>
    /// The vertical offset from the center of the head of the model
    /// </summary>
    [Tooltip("The vertical offset from the center of the head of the model")]
    public float trueParentOffset = 0.1f;
    /// <summary>
    /// The maximum angle the player can look up and down
    /// </summary>
    [Range(0, 90)]
    public float maxAngle = 70;
    /// <summary>
    /// Disables the players mouse cursor
    /// </summary>
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    /// <summary>
    /// Updates the players rotation and cameras rotation
    /// </summary>
    void Update()
    {
        if (trueParent != null)
            transform.position = trueParent.position + transform.up * trueParentOffset;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector3 angles = transform.eulerAngles;
            float dx = -Input.GetAxis("Mouse Y");
            float dy = Input.GetAxis("Mouse X");

            dx = angles.x + dx * speed * Time.deltaTime;

            //Clamp the angle within 70 degrees
            if (dx < 360 && dx < 360 - maxAngle && dx > 180)
                dx = 360 - maxAngle;
            else if (dx >= 0 && dx > maxAngle && dx < 180)
                dx = maxAngle;

            angles.x = dx;

            Vector3 parentAngle = parentTrans.eulerAngles;
            parentAngle.y += dy * speed * Time.deltaTime;
            parentTrans.eulerAngles = parentAngle;

            angles.y = parentAngle.y;

            transform.eulerAngles = angles;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

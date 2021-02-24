using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 1000;

    [Range(0, 90)]
    public float maxAngle = 70;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector3 angles = transform.eulerAngles;
            float dx = -Input.GetAxis("Mouse Y");
            float dy = Input.GetAxis("Mouse X");
            float dxDir = dx < 0 ? -1 : 1;
            // Debug.Log(dxDir);

            dx = angles.x + dx * speed * Time.deltaTime;

            //Clamp the angle within 70 degrees
            if (dx < 360 && dx < 360 - maxAngle && dx > 180)
                dx = 360 - maxAngle;
            else if (dx >= 0 && dx > maxAngle && dx < 180)
                dx = maxAngle;

            angles.x = dx;

            angles.y += dy * speed * Time.deltaTime;
            transform.eulerAngles = angles;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

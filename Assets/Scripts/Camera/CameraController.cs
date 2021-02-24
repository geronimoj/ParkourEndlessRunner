using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 1000;
    public Transform parentTrans = null;
    public Transform trueParent = null;
    public Vector3 trueParentOffset = new Vector3();

    [Range(0, 90)]
    public float maxAngle = 70;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        if (trueParent != null)
            transform.position = trueParent.position + trueParentOffset;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneRotationFixer : MonoBehaviour
{
    public Vector3 localRotationOffset = new Vector3();

    private void LateUpdate()
    {
        transform.localEulerAngles -= localRotationOffset;
    }
}

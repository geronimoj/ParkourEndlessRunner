using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorTile : MonoBehaviour
{
    private void Awake()
    {
        Renderer r = GetComponent<Renderer>();
        //Simply set the cull height of the back face to be something big so it is pretty much always culled
        r.material.SetFloat("_Back", 10000);
    }
}

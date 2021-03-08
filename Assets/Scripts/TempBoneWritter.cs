using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBoneWritter : MonoBehaviour
{
    [ContextMenu("Print Bone Heirachy")]
    void PrintBoneHeirachy()
    {
        SkinnedMeshRenderer rend = GetComponent<SkinnedMeshRenderer>();

        if (rend == null)
            return;
        int i = 0;
        foreach (Transform t in rend.bones)
        {
            Debug.Log("Index: " + i + " Bone Name: " + t.name);
            i++;
        }
    }
    //0. Delete our old model

    //1. Load the model we want to change too

    //2. Get all the SkinnedMeshRenderer components off of them

    //3. Create empty gameObjects on the player and put skinned mesh renderers on them.

    //4. Copy the necessary variables from the SkinnedMeshRenderers from the model onto our new skinned mesh renderers.

    //5. Update the bones of the skinned mesh renderers to be our bones but in the same order.
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    protected bool _rebindingInput = false;

    public void RebindingInput()
    {
        _rebindingInput = true;
    }
}

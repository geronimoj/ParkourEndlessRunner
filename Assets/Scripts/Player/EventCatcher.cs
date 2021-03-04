using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCatcher : MonoBehaviour
{
    private Player m_p;
    private void Awake()
    {
        m_p = GetComponentInParent<Player>();
    }

    void GetUp()
    {
        m_p.GetUp();
        Debug.Log("Get Up");
    }
}

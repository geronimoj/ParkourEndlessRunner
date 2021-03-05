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

    void EnterSlide()
    {
        m_p.EnterSlide();
    }

    void ReleaseCamera()
    {
        m_p.ReleaseCamera();
    }

    public void ExitVault()
    {
        m_p.ExitVault();
    }
}

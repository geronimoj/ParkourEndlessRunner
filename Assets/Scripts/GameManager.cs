using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelGenerator))]
public class GameManager : MonoBehaviour
{
    LevelGenerator m_lg;

    Player m_p;

    Ragdoll m_playerRagdoll;

    private void Awake()
    {
        m_lg = GetComponent<LevelGenerator>();
        m_p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_playerRagdoll = m_p.GetComponentInChildren<Ragdoll>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_lg.CreateLevel();
            m_playerRagdoll.RagdollOn = false;
            m_p.ResetPosition();
        }
    }
}

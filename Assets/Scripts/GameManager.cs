using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LevelGenerator))]
public class GameManager : MonoBehaviour
{
    LevelGenerator m_lg;

    Player m_p;

    Ragdoll m_playerRagdoll;

    Text m_score;

    private void Awake()
    {
        m_lg = GetComponent<LevelGenerator>();
        m_p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_playerRagdoll = m_p.GetComponentInChildren<Ragdoll>();
        m_score = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
    }

    void Update()
    {   //Cast score to an int to cut off any decimal places. We don't want them in the score
        m_score.text = "Score: " + (int)m_p.Score;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_lg.CreateLevel();
            m_playerRagdoll.RagdollOn = false;
            m_p.ResetPosition();
        }
    }
}

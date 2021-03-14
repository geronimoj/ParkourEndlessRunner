using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LevelGenerator))]
public class GameManager : Manager
{
    LevelGenerator m_lg = null;

    Player m_p = null;

    Text m_score = null;

    public GameObject m_pauseMenu = null;

    public GameObject m_deadText = null;

    private void Awake()
    {
        m_lg = GetComponent<LevelGenerator>();
        m_p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_score = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();

        m_pauseMenu.SetActive(false);
    }

    void Update()
    {   //Cast score to an int to cut off any decimal places. We don't want them in the score
        m_score.text = "Score: " + (int)m_p.Score;
        //If we are rebinding a key don't toggle the menu
        if (_rebindingInput)
        {   //If the player pressed any key, allow the menu to be used again
            if (Input.anyKeyDown)
                _rebindingInput = false;
            return;
        }

        if (!m_pauseMenu.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            m_lg.CreateLevel();
            m_p.ResetPlayer();
        }

        m_deadText.SetActive(m_p.IsDead);

        if (Input.GetKeyDown(KeyCode.Escape))
        {   //Toggle time scale
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;

            m_pauseMenu.SetActive(!m_pauseMenu.activeSelf);
        }
    }
}

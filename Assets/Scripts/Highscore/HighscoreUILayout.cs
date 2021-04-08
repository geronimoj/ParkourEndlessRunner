using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Controls how the UI is ordered and displayed
/// </summary>
public class HighscoreUILayout : MonoBehaviour
{
    /// <summary>
    /// The highscoreUI prefab
    /// </summary>
    [SerializeField]
    private GameObject _highscoreUIPrefab = null;
    /// <summary>
    /// The layoutGroup transform the highscore UI is on
    /// </summary>
    [SerializeField]
    private Transform _layoutGroup = null;

    [SerializeField]
    private Transform _canvas = null;

    private List<GameObject> _cameras = new List<GameObject>();
    /// <summary>
    /// Generates the highscoreUI
    /// </summary>
    public void Display()
    {
        //Make sure we have the prefabs
        if (!_highscoreUIPrefab)
        {
            Debug.LogError("Highscore UI prefab not assigned!");
            return;
        }
        //make sure we have a layout group
        if (!_layoutGroup)
        {
            Debug.LogError("Layout Group not assigned!");
            return;
        }
        //If there are children still on it for some reason, get rid of them before continuing
        if (_layoutGroup.childCount != 0)
            Close();
        //We only display the first 10 highscores.
        //This shouldn't be a const but it'll do for now
        //We generate all the UI we will need to make it easier later
        for (int i = 0; i < 10; i++)
        {   //Spawn the ui and dissable them all so they don't appear.
            //We'll leave SortByScore to enable them and assign the highscore UI
            GameObject ui = Instantiate(_highscoreUIPrefab, _layoutGroup);
            ui.SetActive(false);
        }
        //Assign the highscores
        SortByScore();
    }
    /// <summary>
    /// Sets the highscore UI to display by run score
    /// </summary>
    public void SortByScore()
    {   //Loop over all the high scores
        for (int i = 0; i < 10; i++)
        {   //Get the HighscoreUI component
            HighscoreUI hUi = _layoutGroup.GetChild(i).GetComponent<HighscoreUI>();
            //If we couldn't find it, report and error and give up.
            if (!hUi)
            {
                Debug.LogError("Highscore UI does not contain UI!");
                return;
            }
            //Get the highscore for that value
            Highscore h = Highscore.GetScore(i, false);
            //Check if its valid
            if (h)
            {   //If its valid, set it to be used and enable that ui component
                hUi.m_hScore = h;
                hUi.SetValues();
                hUi.gameObject.SetActive(true);

                if (hUi.ModelCamera)
                    _cameras.Add(hUi.ModelCamera);
            }
            //Otherwise make sure that ui component is dissabled
            else
                hUi.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Sets the highscores to display by run duration
    /// </summary>
    public void SortByDuration()
    {   //Loop over the layout groups children
        for (int i = 0; i < 10; i++)
        {
            HighscoreUI hUi = _layoutGroup.GetChild(i).GetComponent<HighscoreUI>();
            //Make sure we get the highscoreUI component
            if (!hUi)
            {   //Otherwise give up
                Debug.LogError("Highscore UI does not contain UI!");
                return;
            }
            //Get the highscore
            Highscore h = Highscore.GetScore(i, true);
            //Make sure it valid
            if (h)
            {   //Set the UI to use this score and enable it
                hUi.m_hScore = h;
                hUi.SetValues();
                hUi.gameObject.SetActive(true);
            }
            //If its not, make sure the gameObject isn't active.
            //This is for swapping between SortByDuration and SortByScore
            //If for whatever reason they become out of synce
            else
                hUi.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Cleans up the HighscoreUI and Cameras
    /// </summary>
    public void Close()
    {
        int length = _layoutGroup.childCount;
        for (int i = 0; i < length; i++)
            DestroyImmediate(_layoutGroup.GetChild(0).gameObject);

        foreach (GameObject cam in _cameras)
            Destroy(cam);
        _cameras.Clear();
    }

    private void Update()
    {   //If the canvas is disabled but the layout group still has children or we have cameras, call close again.
        if ((_layoutGroup.childCount != 0 || _cameras.Count != 0) && !_canvas.gameObject.activeSelf)
            Close();
    }
}

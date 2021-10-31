using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Adds a Lambda function that calls Highscore.SaveScores to the OnClick Event
/// </summary>
[RequireComponent(typeof(Button))]
public class SaveOnClick : MonoBehaviour
{
    /// <summary>
    /// Adds a Lambda function that calls Highscore.SaveScores to the OnClick Event
    /// </summary>
    void Start()
    {
        Button b = GetComponent<Button>();

        if (b)
            b.onClick.AddListener(() => { Highscore.SaveScores(); });
    }
}

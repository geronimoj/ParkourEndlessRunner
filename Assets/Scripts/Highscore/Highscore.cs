using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Stores the highscores
/// </summary>
public class Highscore
{
    /// <summary>
    /// Stores all the highscores. The first 10 are highscore in order of score.
    /// The second 10 (11 - 20) are highscores in order of duration
    /// </summary>
    private static Highscore[] highscores = new Highscore[20];
    /// <summary>
    /// The duration of the run
    /// </summary>
    private readonly float _duration;
    /// <summary>
    /// The duration of the run
    /// </summary>
    public float Duration => _duration;
     /// <summary>
     /// The score the player got from the run
     /// </summary>
    private readonly int _score;
    /// <summary>
    /// The score the player got
    /// </summary>
    public int Score => _score;
    /// <summary>
    /// The index of the model used for this run. Index is in order of Player Models from the Main Menu Manager
    /// </summary>
    private readonly uint _model;
    /// <summary>
    /// The index of the model used for this run. Index is in order of Player Models from the Main Menu Manager
    /// </summary>
    public uint Model => _model;
    /// <summary>
    /// Constructor for Highscore
    /// </summary>
    /// <param name="model">The index of the model used for this run. Index is in order of Player Models from the Main Menu Manager</param>
    /// <param name="score">The score the player got during the run</param>
    /// <param name="duration">The duration of the run</param>
    public Highscore(uint model, int score, float duration)
    {
        _model = model;
        _score = score;
        _duration = duration;
    }
    /// <summary>
    /// Adds a scores to the highscores if it would be in the top 10
    /// </summary>
    /// <param name="score">The highscore to add</param>
    public static void AddScore(Highscore score)
    {   //Precalculate the length to save on repeat calculations
        int length = highscores.Length / 2;
        bool assignedScore = false;
        bool assignedDuration = false;
        for (int i = 0; i < length; i++)
        {   //Check if we have found a slot to put the score
                                    //We will either need to move a score or put it in a null
                                    //We check that highscores[i] is not null first to make sure the second check doesn't pop out a null reference exception
            if (!assignedScore && (!highscores[i] || highscores[i].Score < score.Score))
            {   //If we need to move scores down, move them down
                if (highscores[i])
                    for (int sc = length - 2; sc >= i; sc--)
                        //Move the highscores down to make space
                        highscores[sc + 1] = highscores[sc];
                //Store the highScore
                highscores[i] = score;
                assignedScore = true;
            }
            int index = length + i;
            //Check if we have found a slot to put duration
            if (!assignedDuration && (!highscores[index] || highscores[index].Duration < score.Duration))
            {   //If we need to move scores down, move them down
                if (highscores[index])
                    for (int sc = highscores.Length - 2; sc >= index; sc--)
                        //Move the highscores down to make space
                        highscores[sc + 1] = highscores[sc];
                //Store the highScore
                highscores[index] = score;
                assignedDuration = true;
            }
            //If we find the score and duration early, break out
            if (assignedScore && assignedDuration)
                break;
        }
    }
    /// <summary>
    /// Saves the scores. This does significant amounts of string concatinations and should not be called regularly
    /// </summary>
    public static void SaveScores()
    {   //Precalculate the length to save on repeat calculations
        int length = highscores.Length / 2;
        int index;
        for (int i = 0; i < length; i++)
        {   //Make sure this index is not null before continuing
            if (highscores[i])
            {
                //Store the scores for the highscore by score tracker
                PlayerPrefs.SetFloat(i + "duS", highscores[i].Duration);
                PlayerPrefs.SetInt(i + "scS", highscores[i].Score);
                PlayerPrefs.SetInt(i + "moS", (int)highscores[i].Model);
            }
            //If its null, then save the score as -1
            else
                PlayerPrefs.SetInt(i + "scS", -1);
            //Calculate the index of the score
            index = length + i;
            //Make sure this index is not null before continuing
            if (highscores[index])
            {
                //Store the duration for the highsocre by duration tracker
                PlayerPrefs.SetFloat(i + "duD", highscores[index].Duration);
                PlayerPrefs.SetInt(i + "scD", highscores[index].Score);
                PlayerPrefs.SetInt(i + "moD", (int)highscores[index].Model);
            }
            //If its null, then save the score as -1
            else
                PlayerPrefs.SetInt(i + "scD", -1);
        }
        //Save the PlayerPrefs
        PlayerPrefs.Save();
    }
    /// <summary>
    /// Loads the scores from PlayerPrefs. This uses lots of string concat so don't call it regularly
    /// </summary>
    public static void LoadScores()
    {   //Initialise the highscores if its not already initialised
        if (highscores == null)
            highscores = new Highscore[20];
        //Prepare storage locations for the highscore values
        int score;
        float duration;
        uint model;
        int length = highscores.Length / 2;
        //Loop over the scores and load them
        for (int i = 0; i < length; i++)
        {   //Get the score
            score = PlayerPrefs.GetInt(i + "scS");
            //Score is stored as -1 if there is no score there, so make sure its positive before continuing
            //We also assume that you can't have a score of 0
            if (score > 0)
            {   //Get the duration and model
                duration = PlayerPrefs.GetFloat(i + "duS");
                model = (uint)PlayerPrefs.GetInt(i + "moS");
                //And store the score
                highscores[i] = new Highscore(model, score, duration);
            }
            //Store a null if there is not score
            else
                highscores[i] = null;
            //Get the score
            score = PlayerPrefs.GetInt(i + "scD");
            //Score is stored as -1 if there is no score there, so make sure its positive before continuing
            if (score > 0)
            {   //Get the duration and model
                duration = PlayerPrefs.GetFloat(i + "duD");
                model = (uint)PlayerPrefs.GetInt(i + "moD");
                //And store the score
                highscores[length + i] = new Highscore(model, score, duration);
            }
            else
                //Store a null if there is no score
                highscores[length + i] = null;
        }
    }
    /// <summary>
    /// Removes a score from the highscores
    /// </summary>
    /// <param name="scoreToRemove">An identical copy of the score</param>
    public static void RemoveScore(Highscore scoreToRemove)
    {   //Loop over the highscores and check for identical scores.
        for (int i = 0; i < highscores.Length; i++)
            if (highscores[i] == scoreToRemove)
                //If one is found, delete it
                highscores[i] = null;
    }
    /// <summary>
    /// Sets all highscores to nothing
    /// </summary>
    public static void ResetScores()
    {   //Loop over the scores and set them to null
        for (int i = 0; i < highscores.Length; i++)
            highscores[i] = null;
    }
    /// <summary>
    /// Returns the highscore at the index
    /// </summary>
    /// <param name="index">The index of the highscore. Should be between 0 and 9 (There are 10 highscores stored)</param>
    /// <param name="scoreByDuration">Do you want the highscore by duration or by score</param>
    /// <returns>Returns null if there is no score at the index</returns>
    public static Highscore GetScore(int index, bool scoreByDuration)
    {   //If the index is invalid, return null
        if (index < 0 || index > 9)
            return null;
        //If they want the highscore at index index for the duration, return that
        if (scoreByDuration)
            return highscores[highscores.Length / 2 + index];
        //Otherwise return score
        return highscores[index];
    }
    /// <summary>
    /// Overloaded operator for comparing Highscores
    /// </summary>
    /// <param name="l">Left</param>
    /// <param name="r">Right</param>
    /// <returns>True if highscores are identical</returns>
    public static bool operator == (Highscore l, Highscore r)
    {   //If either are null, we need to do different checks
        if (Equals(null, l) || Equals(null, r))
        {   //If they are both null, return true
            if (Equals(null, l) && Equals(null, r))
                return true;
            //Otherwise return true
            return false;
        }
        if (Conditions.InTolerance(l.Duration, r.Duration, 0.001f) && l.Model == r.Model && l.Score == r.Score)
            return true;
        return false;
    }
    /// <summary>
    /// Overriden to avoid warnings. Identical to base.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(object other)
    {
        return base.Equals(other);
    }
    /// <summary>
    /// Overriden to avoid warnings. Identical to base.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    /// <summary>
    /// Overloaded operator for comparing Highscores
    /// </summary>
    /// <param name="l">Left</param>
    /// <param name="r">Right</param>
    /// <returns>False if highscores are identical</returns>
    public static bool operator !=(Highscore l, Highscore r)
    {   //If either are null, we need to do different checks
        if (Equals(null, l) || Equals(null, r))
        {   //If they are both null, return true
            if (Equals(null, l) && Equals(null, r))
                return false;
            //Otherwise return true
            return true;
        }
        if (l.Duration == r.Duration && l.Model == r.Model && l.Score == r.Score)
            return false;
        return true;
    }
    /// <summary>
    /// Overload conversion operator to bool because not deriving from Monobehaviour.
    /// Converts to True if the Highscore is not null, otherwise converts to False
    /// </summary>
    /// <param name="s">The score to convert</param>
    public static implicit operator bool(Highscore s)
    {
        return s != null;
    }
}
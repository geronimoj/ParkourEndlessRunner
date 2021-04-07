using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highscore : MonoBehaviour
{
    static Highscore[] highscores = new Highscore[10];

    private float _duration = 0;

    public float Duration => _duration;
     
    private int _score = 0;

    public int Score => _score;
    
    private uint _model = 0;

    public uint Model => _model;

    public Highscore(uint model, int score, float duration)
    {
        _model = model;
        _score = score;
        _duration = duration;
    }

    public Highscore()
    {
        _duration = 0;
        _model = 0;
        _score = 0;
    }

    public static void AddScore(Highscore score)
    { 
    }

    public static void SaveScores()
    {

    }

    public static void LoadScores()
    {

    }

    public static void RemoveScore(Highscore scoreToRemove)
    {
        for (int i = 0; i < highscores.Length; i++)
            if (highscores[i] == scoreToRemove)
            {
                highscores[i] = new Highscore();
                break;
            }
    }

    public static void ResetScores()
    {

    }

    public static bool operator == (Highscore l, Highscore r)
    {
        if (l.Duration == r.Duration && l.Model == r.Model && l.Score == r.Score)
            return true;
        return false;
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }


    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator !=(Highscore l, Highscore r)
    {
        if (l.Duration == r.Duration && l.Model == r.Model && l.Score == r.Score)
            return false;
        return true;
    }
}
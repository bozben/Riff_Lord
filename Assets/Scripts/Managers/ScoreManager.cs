using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int currentScore = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NoteHit(int scoreValue)
    {
        currentScore += scoreValue;
    }

    public void NoteMissed(int scorePenalty)
    {
        currentScore -= scorePenalty;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
        Debug.Log("Points Reset.");
    }
}
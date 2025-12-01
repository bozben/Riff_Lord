using System;
using UnityEngine;

public class RhythmEvents : MonoBehaviour
{
    private static RhythmEvents _instance;
    public static RhythmEvents instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<RhythmEvents>();

                if (_instance == null)
                {
                    Debug.LogWarning("No rhythmevent found!");
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public event Action OnNoteHit;

    public void NoteHit()
    {
        OnNoteHit?.Invoke();
    }
}

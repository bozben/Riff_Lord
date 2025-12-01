using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Data")]
    [Tooltip("WaveList")]
    public List<WaveData_SO> waves;
    public List<CharacterData_SO> allChampions;

    [Header("Player Progression")]
    [Tooltip("Wave index.")]
    public int currentWaveIndex = 0;

    [Header("Player Selection")]
    public List<CharacterData_SO> selectedTeam = new List<CharacterData_SO>();
    public SongChart_SO selectedSpell;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (selectedTeam.Count == 0 && allChampions.Count > 0)
        {
            selectedTeam.Add(allChampions[0]);
            Debug.Log("GameManager: Team was empty " + allChampions[0].characterName + " added.");
        }
        else if (allChampions.Count == 0)
        {
            Debug.LogError("List is empty!!!");
        }
    }


    public WaveData_SO GetCurrentWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            return waves[waves.Count - 1];
        }
        return waves[currentWaveIndex];
    }

    public void PlayerWonWave()
    {
        currentWaveIndex++;
        SaveLoadManager.instance.SaveGame();
        Debug.Log("Next wave: " + currentWaveIndex);
    }
}
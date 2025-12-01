using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveReward
{
    [Tooltip("Max gold reward amount.")]
    public int maxGold = 100;
    [Tooltip("Max soul reward amount")]
    public int soulsOnVictory = 5;
}
[System.Serializable]
public struct WaveEnemyData
{
    public CharacterData_SO enemyData;
    public int level;
}

[CreateAssetMenu(fileName = "New Wave Data", menuName = "Riff Lord/Wave Data")]
public class WaveData_SO : ScriptableObject
{
    [Header("Wave Info")]
    public string waveName;

    [Tooltip("Wave Song")]
    public AudioClip waveMusic;

    [Header("Enemies")]
    [Tooltip("Max 3 enemies.")]
    public List<WaveEnemyData> enemiesInWave;

    [Header("Rewards")]
    public WaveReward rewards;

    private void OnValidate()
    {
        if (enemiesInWave.Count > 3)
        {
            Debug.LogWarning("Enemy count cant be more than 3!!! '" + this.name + "'s excess heroes removed.");
            enemiesInWave.RemoveRange(3, enemiesInWave.Count - 3);
        }
    }
}
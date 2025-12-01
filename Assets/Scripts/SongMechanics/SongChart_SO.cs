using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TierData
{
    public string tierName; 
    public int scoreThreshold; 
    public int effectAmount; 
}

[CreateAssetMenu(fileName = "New Spell Chart", menuName = "Riff Lord/Spell Chart")]
public class SongChart_SO : ScriptableObject
{
    [Header("Spell Info")]
    public string spellName; 
    [TextArea(4, 8)]
    public string spellLore;

    [Header("Rhythm Settings")]
    public float baseBPM = 120f;
    [TextArea(15, 20)]
    public string rhythmPattern;

    [Header("Effect Data")]
    public RhythmEffect_SO mainEffectType;
    [TextArea(2, 4)]
    public string effectDescription; 

    [Header("Tier System")]
    public List<TierData> tiers; 

    [Header("Progression")]
    public int level = 1;
    public int baseUpgradeCost = 10;
    public int costIncreasePerLevel = 50;

    [Header("Growth Settings")]
    public float bpmIncreasePerLevel = 5f; 
    public int scoreThresholdIncrease = 50; 
    public int effectGrowth = 5;
    [Header("Unlock Info")]
    public bool isUnlocked = false;
    public int unlockCost = 50;

    public float GetCurrentBPM()
    {
        return baseBPM + ((level - 1) * bpmIncreasePerLevel);
    }

    public TierData GetCurrentTierData(int index)
    {
        if (index >= tiers.Count) return new TierData();

        int levelsGained = level - 1;
        TierData original = tiers[index];
        TierData current = new TierData();

        current.tierName = original.tierName;

        current.scoreThreshold = original.scoreThreshold + (levelsGained * scoreThresholdIncrease);

        if (original.effectAmount < 0)
        {
            current.effectAmount = original.effectAmount - (levelsGained * effectGrowth);
        }
        else if (original.effectAmount > 0)
        {
            current.effectAmount = original.effectAmount + (levelsGained * effectGrowth);
        }
        else
        {
            current.effectAmount = 0;
        }

        return current;
    }

}
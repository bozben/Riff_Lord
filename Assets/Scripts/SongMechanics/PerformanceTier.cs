
using UnityEngine;

[System.Serializable]
public class PerformanceTier
{
    public string tierName = "New Tier";
    [Tooltip("Min amount")]
    public int scoreThreshold;
    [Tooltip("Sound effect buff/debuff.")]
    public RhythmEffect_SO effectToApply;
}
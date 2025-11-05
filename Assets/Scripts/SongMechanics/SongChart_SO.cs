using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song Chart", menuName = "Riff Lord/Song Chart")]
public class SongChart_SO : ScriptableObject
{
    [Header("Song Info")]
    public AudioClip musicClip;
    public string songName;
    public string artistName;
    [Tooltip("Beats Per Minute: Note speed.")]
    public float bpm = 120f;

    [Header("Note Chart")]
    [Tooltip("1-5=Lane, -=Space, {13}=Dual Note, [1--]=Long note")]
    [TextArea(15, 20)] 
    public string noteChart;

    [Header("Performance System")]
    [Tooltip("Checks for buff points")]
    public List<float> performanceCheckpoints;

    [Tooltip("Determines buff amount")]
    public List<PerformanceTier> performanceTiers;
}
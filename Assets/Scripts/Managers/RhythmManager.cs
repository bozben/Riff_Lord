using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class RhythmManager : MonoBehaviour
{
    [Header("Spell Data")]
    public SongChart_SO currentSpell;

    [Header("Setup")]
    public GameObject notePrefab;
    public Transform[] noteSpawnPoints;
    public Transform notesContainer;
    public Transform hitZone;
    public float checkInterval = 10f;
    [SerializeField] private ComboManager comboManager;

    [Header("Visuals")]
    public float noteSpeed = 700f;
    public float spaceMultiplier = 1.0f;
    public Color[] laneColors;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI remainingBuffsText;
    public TextMeshProUGUI targetScoreText;


    private BattleSystem battleSystem;
    private float performanceTimer = 10f;

    private float currentBPM;
    private int nextCheckpointIndex = 0;
    private int maxTargetScore = 0;


    private bool isGameStarted = false;


    void Start()
    {
        battleSystem = FindFirstObjectByType<BattleSystem>();
        if (comboManager == null) comboManager = FindFirstObjectByType<ComboManager>();
    }
    private void UpdateRhythmUI(float currentTime)
    {
        if (comboManager != null && comboManager.comboText != null)
        {
            comboManager.comboText.text = ScoreManager.instance.GetCurrentScore().ToString();
        }
        if (timerText != null)
        {
            timerText.text = "Next Cast: " + performanceTimer.ToString("F0") + "s";
        }

        int currentScore = ScoreManager.instance.GetCurrentScore();

        if (remainingBuffsText != null) 
        {
        }

        if (targetScoreText != null)
        {
            string targetText = "Max Tier!";
            int nextThreshold = 0;
            string nextTierName = "";

            foreach (var tier in currentSpell.tiers.OrderBy(t => t.scoreThreshold))
            {
                if (currentScore < tier.scoreThreshold)
                {
                    nextThreshold = tier.scoreThreshold;
                    nextTierName = tier.tierName;
                    targetText = nextTierName + ": " + nextThreshold;
                    break;
                }
            }

            targetScoreText.text = targetText;
        }
    }
    public void BeginRhythmGame()
    {
        isGameStarted = true;
        StartCoroutine(StartSpellRoutine());
    }

    IEnumerator StartSpellRoutine()
    {
        if (GameManager.instance != null && GameManager.instance.selectedSpell != null)
        {
            currentSpell = GameManager.instance.selectedSpell;
        }
        if (currentSpell == null) { Debug.LogError("No Spell Assigned!"); yield break; }

        foreach (Transform child in notesContainer)
        {
            if (child.CompareTag("Note")) Destroy(child.gameObject);
        }

        int maxTargetScore = 0;
        foreach (var tier in currentSpell.tiers)
        {
            if (tier.scoreThreshold > maxTargetScore) maxTargetScore = tier.scoreThreshold;
        }
        if (targetScoreText != null) targetScoreText.text = "Target: " + maxTargetScore;

        currentBPM = currentSpell.GetCurrentBPM();
        string chartText = currentSpell.rhythmPattern;

        float beatDuration = 60f / currentBPM;
        float sixteenthNoteDuration = beatDuration / 4f;
        float spaceDistance = noteSpeed * sixteenthNoteDuration * spaceMultiplier;

        float currentXOffset = 0f;
        int charIndex = 0;

        while (charIndex < chartText.Length)
        {
            char c = chartText[charIndex];

            if (c != '\n' && c != '\r' && c != ' ')
            {
                currentXOffset += spaceDistance;
            }

            if (c == '{') 
            {
                charIndex++;
                while (charIndex < chartText.Length && chartText[charIndex] != '}')
                {
                    if (char.IsDigit(chartText[charIndex]))
                    {
                        int lane = int.Parse(chartText[charIndex].ToString()) - 1;
                        SpawnNote(lane, currentXOffset);
                    }
                    charIndex++;
                }
            }
            else if (c == '[') 
            {
                while (charIndex < chartText.Length && chartText[charIndex] != ']')
                {
                    charIndex++;
                }
            }
            else if (char.IsDigit(c)) 
            {
                int lane = int.Parse(c.ToString()) - 1;
                if (lane >= 0 && lane < 5)
                {
                    SpawnNote(lane, currentXOffset);
                }
            }

            charIndex++;
        }

        ScoreManager.instance.ResetScore();
        performanceTimer = 10f; 

        yield return null;
    }

    void Update()
    {
        if (!isGameStarted) return;
        if (!AudioManager.instance.musicSource.isPlaying) return;

        performanceTimer -= Time.deltaTime;

        UpdateRhythmUI(AudioManager.instance.musicSource.time);

        if (performanceTimer <= 0)
        {
            PerformCast();
            performanceTimer = checkInterval;
        }
    }

    void SpawnNote(int laneIndex, float xOffset)
    {
        GameObject newNoteGO = Instantiate(notePrefab, notesContainer);

        if (laneColors != null && laneIndex < laneColors.Length)
        {
            var noteImage = newNoteGO.GetComponent<UnityEngine.UI.Image>();
            if (noteImage != null)
            {
                noteImage.color = laneColors[laneIndex];
            }
        }

        Vector3 startPos = noteSpawnPoints[laneIndex].position;
        startPos.x += xOffset; 
        newNoteGO.transform.position = startPos;

        NoteObject noteScript = newNoteGO.GetComponent<NoteObject>();
        noteScript.speed = this.noteSpeed;

        noteScript.Initialize(comboManager, laneIndex);
    }

    private void PerformCast()
    {
        int score = ScoreManager.instance.GetCurrentScore();
        Debug.Log("Cast! Score: " + score);

        TierData selectedTier = new TierData();
        bool found = false;

        for (int i = currentSpell.tiers.Count - 1; i >= 0; i--)
        {
            TierData tierData = currentSpell.GetCurrentTierData(i);
            if (score >= tierData.scoreThreshold)
            {
                selectedTier = tierData;
                found = true;
                break;
            }
        }

        if (remainingBuffsText != null)
        {
            if (found)
            {
                string sign = selectedTier.effectAmount > 0 ? "+" : "";
                string statName = "Buff";

                if (currentSpell.mainEffectType is StatBuffEffect_SO statBuff)
                {
                    statName = statBuff.statToModify.ToString();
                }

                remainingBuffsText.text = "Buff: " + sign + "%" + selectedTier.effectAmount + " " + statName;

                if (selectedTier.effectAmount > 0) remainingBuffsText.color = Color.green;
                else if (selectedTier.effectAmount < 0) remainingBuffsText.color = Color.red;
                else remainingBuffsText.color = Color.white;
            }
            else
            {
                remainingBuffsText.text = "Failed!";
                remainingBuffsText.color = Color.gray;
            }
        }

        if (found)
        {
            if (currentSpell.mainEffectType is StatBuffEffect_SO statBuffRef)
            {
                battleSystem.ApplyDynamicEffect(statBuffRef, selectedTier.effectAmount);
            }
        }

        ScoreManager.instance.ResetScore();
    }
}
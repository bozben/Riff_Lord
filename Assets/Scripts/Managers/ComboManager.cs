using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI comboText;

    private int currentCombo = 0;
    private int peakCombo = 0;



    void Start()
    {
        ResetCombo();
    }

    public void NoteHit()
    {
        currentCombo++;
        if (currentCombo > peakCombo)
        {
            peakCombo = currentCombo;
        }
        UpdateComboText();
    }

    public void NoteMissed()
    {
        currentCombo = 0;
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        comboText.text = ScoreManager.instance.GetCurrentScore().ToString();
    }

    private void ResetCombo()
    {
        currentCombo = 0;
        peakCombo = 0;
        UpdateComboText();
    }

    public int GetPeakCombo()
    {
        return peakCombo;
    }
}
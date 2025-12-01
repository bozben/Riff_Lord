using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Battle_UI : MonoBehaviour
{
    [Header("Champion UI Elements")]
    public TextMeshProUGUI championNameText;
    public Slider championHPSlider;
    public TextMeshProUGUI championHPValueText;
    public Slider championSPSlider;
    public TextMeshProUGUI championSPValueText;
    public Slider championATBSlider;
    public TextMeshProUGUI championATBValueText;

    [Header("Hero UI Elements")]
    public TextMeshProUGUI heroNameText;
    public Slider heroHPSlider;
    public TextMeshProUGUI heroHPValueText;
    public Slider heroSPSlider;
    public TextMeshProUGUI heroSPValueText;
    public Slider heroATBSlider;
    public TextMeshProUGUI heroATBValueText;

    [Header("Champion Stats Text")]
    public TextMeshProUGUI championAtkText;
    public TextMeshProUGUI championDefText;
    public TextMeshProUGUI championSpdText;

    [Header("Hero Stats Text")]
    public TextMeshProUGUI heroAtkText;
    public TextMeshProUGUI heroDefText;
    public TextMeshProUGUI heroSpdText;

    [Header("Wave UI")]
    public TextMeshProUGUI waveTitleText;

    public void SetupUI(CharacterStats champion, CharacterStats hero)
    {
        championNameText.text = champion.characterData.characterName;
        heroNameText.text = hero.characterData.characterName;

        championHPSlider.maxValue = champion.maxHealth;
        championSPSlider.maxValue = champion.maxSP;
        championATBSlider.maxValue = 100f;

        heroHPSlider.maxValue = hero.maxHealth;
        heroSPSlider.maxValue = hero.maxSP;
        heroATBSlider.maxValue = 100f;
    }

    public void UpdateChampionUI(CharacterStats stats, ATB_Controller atb)
    {
        if (championNameText.text != stats.characterData.characterName)
        {
            championNameText.text = stats.characterData.characterName;

            championHPSlider.maxValue = stats.maxHealth;
            championSPSlider.maxValue = stats.maxSP;
        }
        championHPSlider.value = stats.currentHealth;
        championSPSlider.value = stats.currentSP;
        championATBSlider.value = atb.currentATB;

        championHPValueText.text = stats.currentHealth + " / " + stats.maxHealth;
        championSPValueText.text = stats.currentSP + " / " + stats.maxSP;

        if (atb.isTurnReady)
        {
            championATBValueText.text = "Full!";
        }
        else
        {
            championATBValueText.text = Mathf.FloorToInt(atb.currentATB) + " / " + (int)atb.maxATB;
        }
        if (championAtkText != null) championAtkText.text = "ATK: " + stats.attackPower;
        if (championDefText != null) championDefText.text = "DEF: %" + stats.defense;
        if (championSpdText != null) championSpdText.text = "SPD: " + stats.speed;
    }

    public void UpdateHeroUI(CharacterStats stats, ATB_Controller atb)
    {
        if (heroNameText.text != stats.characterData.characterName)
        {
            heroNameText.text = stats.characterData.characterName;

            heroHPSlider.maxValue = stats.maxHealth;
            heroSPSlider.maxValue = stats.maxSP;
        }
        heroHPSlider.value = stats.currentHealth;
        heroSPSlider.value = stats.currentSP;
        heroATBSlider.value = atb.currentATB;

        heroHPValueText.text = stats.currentHealth + " / " + stats.maxHealth;
        heroSPValueText.text = stats.currentSP + " / " + stats.maxSP;

        if (atb.isTurnReady)
        {
            heroATBValueText.text = "Full!";
        }
        else
        {
            heroATBValueText.text = Mathf.FloorToInt(atb.currentATB) + " / " + (int)atb.maxATB;
        }
        if (heroAtkText != null) heroAtkText.text = "ATK: " + stats.attackPower;
        if (heroDefText != null) heroDefText.text = "DEF: %" + stats.defense;
        if (heroSpdText != null) heroSpdText.text = "SPD: " + stats.speed;
    }
}
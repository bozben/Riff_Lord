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
    }

    public void UpdateHeroUI(CharacterStats stats, ATB_Controller atb)
    {
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
    }
}
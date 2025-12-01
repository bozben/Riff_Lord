using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop_UI : MonoBehaviour
{
    private enum ShopTab { Champions, Songs }
    private ShopTab currentTab = ShopTab.Champions;

    private CharacterData_SO selectedChampion;
    private SongChart_SO selectedSong;

    [Header("Data References")]
    public List<CharacterData_SO> allChampions;
    public List<SongChart_SO> allSongs;

    [Header("Header Elements")]
    public TextMeshProUGUI goldValueText;
    public TextMeshProUGUI soulValueText;
    public Button championsTabButton;
    public Button songsTabButton;

    [Header("Scroll View Elements")]
    public GameObject listItemPrefab;
    public Transform listContentParent;

    [Header("Detail Panel Elements")]
    public TextMeshProUGUI detailPanel_Title;
    public TextMeshProUGUI detailPanel_Description;
    public TextMeshProUGUI detailPanel_SpecialName;
    public TextMeshProUGUI detailPanel_SpecialDetails;
    public TextMeshProUGUI detailPanel_SpecialDescription;

    [Header("Stats Panel Elements")]
    public GameObject statsPanel;
    public TextMeshProUGUI currentStats_Title;
    public TextMeshProUGUI currentStats_Stat1;
    public TextMeshProUGUI currentStats_Stat2;
    public TextMeshProUGUI currentStats_Stat3;
    public TextMeshProUGUI currentStats_Stat4;
    public TextMeshProUGUI nextStats_Title;
    public TextMeshProUGUI nextStats_Stat1;
    public TextMeshProUGUI nextStats_Stat2;
    public TextMeshProUGUI nextStats_Stat3;
    public TextMeshProUGUI nextStats_Stat4;

    [Header("Action Buttons")]
    public Button levelUpButton;
    public TextMeshProUGUI levelUpButton_Text;
    public Button startBattleButton;

    private List<GameObject> currentListItems = new List<GameObject>();


    public void OpenShop()
    {
        gameObject.SetActive(true);
        SwitchTab(ShopTab.Champions);
    }

    public void UpdateUI()
    {
        goldValueText.text = PlayerCurrencyManager.instance.currentGold.ToString();
        soulValueText.text = PlayerCurrencyManager.instance.currentSoul.ToString();

        if (currentTab == ShopTab.Champions)
        {
            PopulateChampionList();
            if (selectedChampion == null && allChampions.Count > 0)
                DisplayChampionDetails(allChampions[0]);
            else if (selectedChampion != null)
                DisplayChampionDetails(selectedChampion);
        }
        else if (currentTab == ShopTab.Songs)
        {
            PopulateSongList();
            if (selectedSong == null && allSongs.Count > 0)
                DisplaySongDetails(allSongs[0]);
            else if (selectedSong != null)
                DisplaySongDetails(selectedSong);
        }
    }

    private void ClearList()
    {
        foreach (GameObject item in currentListItems) { Destroy(item); }
        currentListItems.Clear();
    }

    private void PopulateChampionList()
    {
        ClearList();

        foreach (CharacterData_SO champion in allChampions)
        {
            GameObject newItem = Instantiate(listItemPrefab, listContentParent);

            newItem.transform.localScale = Vector3.one;
            newItem.transform.localPosition = new Vector3(newItem.transform.localPosition.x, newItem.transform.localPosition.y, 0);
            newItem.transform.localRotation = Quaternion.identity;

            ShopItemSlot slotScript = newItem.GetComponent<ShopItemSlot>();
            if (slotScript != null)
            {
                slotScript.Setup(champion, this);
            }

            currentListItems.Add(newItem);
        }
    }

    private void PopulateSongList()
    {
        ClearList();
        foreach (SongChart_SO spell in allSongs)
        {
            GameObject newItem = Instantiate(listItemPrefab, listContentParent);

            newItem.transform.localScale = Vector3.one;
            newItem.transform.localPosition = new Vector3(newItem.transform.localPosition.x, newItem.transform.localPosition.y, 0);
            newItem.transform.localRotation = Quaternion.identity;

            ShopItemSlot slotScript = newItem.GetComponent<ShopItemSlot>();
            if (slotScript != null)
            {
                bool isSelected = (selectedSong == spell);
                slotScript.SetupSong(spell, this, isSelected);

                newItem.GetComponent<Button>().onClick.RemoveAllListeners();
                newItem.GetComponent<Button>().onClick.AddListener(() => {
                    DisplaySongDetails(spell);
                    PopulateSongList();
                });
            }
            currentListItems.Add(newItem);
        }
    }


    public void OnSlotButtonClicked(int slotIndex, CharacterData_SO champion)
    {
        if (!champion.isUnlocked) return;

        List<CharacterData_SO> team = GameManager.instance.selectedTeam;

        while (team.Count <= slotIndex) team.Add(null);

        if (team.Contains(champion))
        {
            int oldIndex = team.IndexOf(champion);
            team[oldIndex] = null;
        }

        team[slotIndex] = champion;

        UpdateUI();
    }


    public void DisplayChampionDetails(CharacterData_SO championData)
    {
        selectedChampion = championData;
        selectedSong = null;

        detailPanel_Title.text = championData.isUnlocked ? "Selected: " + championData.characterName : "LOCKED CHAMPION";
        detailPanel_Description.text = championData.characterDescription;
        detailPanel_SpecialName.text = championData.specialAbility.abilityName;
        detailPanel_SpecialDetails.text = "SP Cost: " + championData.specialAbility.spCost;
        detailPanel_SpecialDescription.text = championData.specialAbility.abilityDescription;

        statsPanel.SetActive(true);

        currentStats_Title.text = "Current Level (Lvl." + championData.level + ")";
        currentStats_Stat1.text = "(HP): " + championData.baseHealth;
        currentStats_Stat2.text = "(ATK): " + championData.baseAttack;
        currentStats_Stat3.text = "(DEF): " + championData.baseDefense + "%";
        currentStats_Stat4.text = "(SPD): " + championData.baseSpeed;

        nextStats_Title.text = "Next Level (Lvl." + (championData.level + 1) + ")";
        nextStats_Stat1.text = "(HP): " + (championData.baseHealth + championData.healthGrowth) + " (+" + championData.healthGrowth + ")";
        nextStats_Stat2.text = "(ATK): " + (championData.baseAttack + championData.attackGrowth) + " (+" + championData.attackGrowth + ")";
        nextStats_Stat3.text = "(DEF): " + (championData.baseDefense + championData.defenseGrowth) + "% (+" + championData.defenseGrowth + "%)";
        nextStats_Stat4.text = "(SPD): " + (championData.baseSpeed + championData.speedGrowth) + " (+" + championData.speedGrowth + ")";

        if (championData.isUnlocked)
        {
            int upgradeCost = championData.baseUpgradeCost + ((championData.level - 1) * championData.costIncreasePerLevel);
            levelUpButton_Text.text = "Level Up (" + upgradeCost + " G)";
            levelUpButton.interactable = PlayerCurrencyManager.instance.currentGold >= upgradeCost;
        }
        else
        {
            levelUpButton_Text.text = "UNLOCK (" + championData.unlockCost + " G)";
            levelUpButton.interactable = PlayerCurrencyManager.instance.currentGold >= championData.unlockCost;
        }
    }

    public void DisplaySongDetails(SongChart_SO spellData)
    {
        selectedSong = spellData;
        selectedChampion = null;
        if (spellData.isUnlocked)
        {
            GameManager.instance.selectedSpell = spellData;
            Debug.Log("Battle Spell Set: " + spellData.spellName);
        }
        Debug.Log("Picked Spell: " + spellData.spellName);

        detailPanel_Title.text = GetLevelName(spellData.spellName, spellData.level);
        detailPanel_Description.text = spellData.spellLore;

        if (spellData.mainEffectType != null)
            detailPanel_SpecialName.text = spellData.mainEffectType.effectName;
        else
            detailPanel_SpecialName.text = "Unknown";

        detailPanel_SpecialDescription.text = spellData.effectDescription;

        string tierTable = "";
        for (int i = 0; i < spellData.tiers.Count; i++)
        {
            TierData t = spellData.GetCurrentTierData(i);
            string sign = t.effectAmount > 0 ? "+" : "";
            tierTable += t.tierName + " (" + t.scoreThreshold + "): " + sign + t.effectAmount + "%  ";
            if (i == 2) tierTable += "\n";
        }
        detailPanel_SpecialDetails.text = tierTable;

        statsPanel.SetActive(true);
        currentStats_Stat3.gameObject.SetActive(true); currentStats_Stat4.gameObject.SetActive(true);
        nextStats_Stat3.gameObject.SetActive(true); nextStats_Stat4.gameObject.SetActive(true);

        currentStats_Title.text = "CURRENT: Lv." + spellData.level;

        TierData t1 = spellData.GetCurrentTierData(0);
        TierData t2 = spellData.GetCurrentTierData(1);
        string s1 = t1.effectAmount > 0 ? "+" : "";
        string s2 = t2.effectAmount > 0 ? "+" : "";
        currentStats_Stat1.text = "Tier 1: " + s1 + t1.effectAmount + "%  Tier 2: " + s2 + t2.effectAmount + "%";

        TierData t3 = spellData.GetCurrentTierData(2);
        TierData t4 = spellData.GetCurrentTierData(3);
        string s3 = t3.effectAmount > 0 ? "+" : "";
        string s4 = t4.effectAmount > 0 ? "+" : "";
        currentStats_Stat2.text = "Tier 3: " + s3 + t3.effectAmount + "%  Tier 4: " + s4 + t4.effectAmount + "%";

        TierData t5 = spellData.GetCurrentTierData(4);
        TierData t5p = spellData.GetCurrentTierData(5);
        string s5 = t5.effectAmount > 0 ? "+" : "";
        string s5p = t5p.effectAmount > 0 ? "+" : "";
        currentStats_Stat3.text = "Tier 5: " + s5 + t5.effectAmount + "%  Tier 5+: " + s5p + t5p.effectAmount + "%";

        float currentBPM = spellData.GetCurrentBPM();
        currentStats_Stat4.text = "BPM: " + currentBPM;


        if (!spellData.isUnlocked)
        {
            nextStats_Title.text = "LOCKED";
            nextStats_Stat1.text = "-";
            nextStats_Stat2.text = "-";
            nextStats_Stat3.text = "-";
            nextStats_Stat4.text = "-";

            levelUpButton_Text.text = "UNLOCK (" + spellData.unlockCost + " S)";
            levelUpButton.interactable = PlayerCurrencyManager.instance.currentSoul >= spellData.unlockCost;
        }
        else if (spellData.level >= 5)
        {
            nextStats_Title.text = "MAX LEVEL";
            nextStats_Stat1.text = "-";
            nextStats_Stat2.text = "-";
            nextStats_Stat3.text = "-";
            nextStats_Stat4.text = "-";

            levelUpButton_Text.text = "MAXED OUT";
            levelUpButton.interactable = false;
        }
        else
        {
            nextStats_Title.text = "NEXT: Lv." + (spellData.level + 1);

            int nt1 = t1.effectAmount; int nt2 = t2.effectAmount;
            if (t1.effectAmount < 0) nt1 -= spellData.effectGrowth; else if (t1.effectAmount > 0) nt1 += spellData.effectGrowth;
            if (t2.effectAmount < 0) nt2 -= spellData.effectGrowth; else if (t2.effectAmount > 0) nt2 += spellData.effectGrowth;
            string ns1 = nt1 > 0 ? "+" : ""; string ns2 = nt2 > 0 ? "+" : "";
            nextStats_Stat1.text = "Tier 1: " + ns1 + nt1 + "%  Tier 2: " + ns2 + nt2 + "%";

            int nt3 = t3.effectAmount; int nt4 = t4.effectAmount;
            if (t4.effectAmount > 0) nt4 += spellData.effectGrowth;
            string ns3 = nt3 > 0 ? "+" : ""; string ns4 = nt4 > 0 ? "+" : "";
            nextStats_Stat2.text = "Tier 3: " + ns3 + nt3 + "%  Tier 4: " + ns4 + nt4 + "%";

            int nt5 = t5.effectAmount + spellData.effectGrowth;
            int nt5p = t5p.effectAmount + spellData.effectGrowth;
            string ns5 = nt5 > 0 ? "+" : ""; string ns5p = nt5p > 0 ? "+" : "";
            nextStats_Stat3.text = "Tier 5: " + ns5 + nt5 + "%  Tier 5+: " + ns5p + nt5p + "%";

            float nextBPM = currentBPM + spellData.bpmIncreasePerLevel;
            nextStats_Stat4.text = "BPM: " + nextBPM;

            int upgradeCost = spellData.baseUpgradeCost + ((spellData.level - 1) * spellData.costIncreasePerLevel);
            levelUpButton_Text.text = "UPGRADE (" + upgradeCost + " S)";
            levelUpButton.interactable = PlayerCurrencyManager.instance.currentSoul >= upgradeCost;
        }
    }



    public void OnChampionsTabClicked() { SwitchTab(ShopTab.Champions); }
    public void OnSongsTabClicked() { SwitchTab(ShopTab.Songs); }

    private void SwitchTab(ShopTab tab)
    {
        currentTab = tab;
        UpdateUI();
    }

    public void OnLevelUpClicked()
    {
        if (currentTab == ShopTab.Champions && selectedChampion != null)
        {
            if (!selectedChampion.isUnlocked)
            {
                if (PlayerCurrencyManager.instance.currentGold >= selectedChampion.unlockCost)
                {
                    PlayerCurrencyManager.instance.RemoveGold(selectedChampion.unlockCost);
                    selectedChampion.isUnlocked = true;

                    if (GameManager.instance.selectedTeam.Count == 0)
                        OnSlotButtonClicked(0, selectedChampion);

                    Debug.Log(selectedChampion.characterName + " unlocked!");

                    SaveLoadManager.instance.SaveGame();

                    PopulateChampionList();
                    DisplayChampionDetails(selectedChampion);
                    goldValueText.text = PlayerCurrencyManager.instance.currentGold.ToString();
                }
                return;
            }

            int upgradeCost = selectedChampion.baseUpgradeCost + ((selectedChampion.level - 1) * selectedChampion.costIncreasePerLevel);

            if (PlayerCurrencyManager.instance.currentGold >= upgradeCost)
            {
                PlayerCurrencyManager.instance.RemoveGold(upgradeCost);

                selectedChampion.baseHealth += selectedChampion.healthGrowth;
                selectedChampion.baseAttack += selectedChampion.attackGrowth;
                selectedChampion.baseDefense += selectedChampion.defenseGrowth;
                selectedChampion.baseSpeed += selectedChampion.speedGrowth;

                selectedChampion.level++;

                Debug.Log(selectedChampion.characterName + " level up to " + selectedChampion.level + "!");

                SaveLoadManager.instance.SaveGame();

                PopulateChampionList();
                DisplayChampionDetails(selectedChampion);
                goldValueText.text = PlayerCurrencyManager.instance.currentGold.ToString();
            }
        }
        else if (currentTab == ShopTab.Songs && selectedSong != null)
        {
            if (!selectedSong.isUnlocked)
            {
                if (PlayerCurrencyManager.instance.currentSoul >= selectedSong.unlockCost)
                {
                    PlayerCurrencyManager.instance.RemoveSoul(selectedSong.unlockCost);
                    selectedSong.isUnlocked = true;

                    Debug.Log(selectedSong.spellName + " unlocked!");

                    SaveLoadManager.instance.SaveGame();

                    DisplaySongDetails(selectedSong);
                    soulValueText.text = PlayerCurrencyManager.instance.currentSoul.ToString();
                }
                return;
            }

            if (selectedSong.level >= 5) return; 

            int upgradeCost = selectedSong.baseUpgradeCost + ((selectedSong.level - 1) * selectedSong.costIncreasePerLevel);

            if (PlayerCurrencyManager.instance.currentSoul >= upgradeCost)
            {
                PlayerCurrencyManager.instance.RemoveSoul(upgradeCost);

                selectedSong.level++;

                Debug.Log(selectedSong.spellName + " upgraded to level " + selectedSong.level + "!");

                SaveLoadManager.instance.SaveGame();

                PopulateSongList();
                DisplaySongDetails(selectedSong);
                soulValueText.text = PlayerCurrencyManager.instance.currentSoul.ToString();
            }
        }
    }

    public void OnStartBattleClicked()
    {
        bool hasChampion = false;
        foreach (var c in GameManager.instance.selectedTeam) if (c != null) hasChampion = true;

        if (!hasChampion)
        {
            Debug.LogWarning("Lütfen en az bir þampiyon seçin!");
            return;
        }

        SceneManager.LoadScene("CombatScene");
    }

    private string GetLevelName(string baseName, int level)
    {
        switch (level)
        {
            case 1: return baseName;
            case 2: return baseName + "+";
            case 3: return "+" + baseName + "+";
            case 4: return "+" + baseName + "++";
            case 5: return "++" + baseName + "++";
            default: return baseName + " (Max)"; 
        }
    }
}
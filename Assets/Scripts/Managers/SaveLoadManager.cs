using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public List<CharacterData_SO> allChampions;
    public List<SongChart_SO> allSongs;
    public static SaveLoadManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        LoadGame();
    }

    

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Gold", PlayerCurrencyManager.instance.currentGold);
        PlayerPrefs.SetInt("Soul", PlayerCurrencyManager.instance.currentSoul);

        foreach(CharacterData_SO champion in allChampions)
        {
            PlayerPrefs.SetInt("Level_" + champion.characterName, champion.level);
            PlayerPrefs.SetInt("Unlock_" + champion.characterName, champion.isUnlocked ? 1 : 0);
           
        }

        foreach (SongChart_SO song in allSongs)
        {
            PlayerPrefs.SetInt("Level_" + song.spellName, song.level);
            PlayerPrefs.SetInt("Unlock_" + song.spellName, song.isUnlocked ? 1 : 0);
        }
        PlayerPrefs.SetInt("CurrentWaveIndex", GameManager.instance.currentWaveIndex);
        List<CharacterData_SO> team = GameManager.instance.selectedTeam;
        for (int i = 0; i < 3; i++)
        {
            if (i < team.Count && team[i] != null)
                PlayerPrefs.SetString("Team_" + i, team[i].characterName);
            else
                PlayerPrefs.SetString("Team_" + i, "EMPTY");
        }

        PlayerPrefs.Save();
        Debug.Log("Game Saved!");
    }
    public void LoadGame()
    {
        PlayerCurrencyManager.instance.currentGold = PlayerPrefs.GetInt("Gold", 0);
        PlayerCurrencyManager.instance.currentSoul = PlayerPrefs.GetInt("Soul", 0);

        foreach (CharacterData_SO champion in allChampions)
        {
            champion.level = PlayerPrefs.GetInt("Level_" + champion.characterName, 1);
            champion.RecalculateStats();
            if (PlayerPrefs.HasKey("Unlock_" + champion.characterName))
            {
                int unlocked = PlayerPrefs.GetInt("Unlock_" + champion.characterName);
                champion.isUnlocked = (unlocked == 1);
            }
        }

        foreach (SongChart_SO song in allSongs)
        {
            song.level = PlayerPrefs.GetInt("Level_" + song.spellName, 1);
            if (PlayerPrefs.HasKey("Unlock_" + song.spellName))
            {
                song.isUnlocked = (PlayerPrefs.GetInt("Unlock_" + song.spellName) == 1);
            }
        }
        GameManager.instance.currentWaveIndex = PlayerPrefs.GetInt("CurrentWaveIndex", 0);
        GameManager.instance.selectedTeam.Clear();
        for (int i = 0; i < 3; i++) GameManager.instance.selectedTeam.Add(null);

        for (int i = 0; i < 3; i++)
        {
            string savedName = PlayerPrefs.GetString("Team_" + i, "EMPTY");
            if (savedName != "EMPTY")
            {
                CharacterData_SO foundChamp = allChampions.Find(x => x.characterName == savedName);
                if (foundChamp != null)
                {
                    GameManager.instance.selectedTeam[i] = foundChamp;
                }
            }
        }
        bool hasAnyChampion = false;
        foreach (var c in GameManager.instance.selectedTeam)
        {
            if (c != null) hasAnyChampion = true;
        }

        if (!hasAnyChampion && allChampions.Count > 0)
        {
            if (allChampions[0].isUnlocked)
            {
                GameManager.instance.selectedTeam[0] = allChampions[0];
                Debug.Log("No team found.Added default: " + allChampions[0].characterName);
            }
        }
        if (!hasAnyChampion)
        {
            CharacterData_SO starterChamp = allChampions.Find(x => x.isUnlocked);

            if (starterChamp != null)
            {
                GameManager.instance.selectedTeam[0] = starterChamp;
                Debug.Log("Kayýt yok. Baþlangýç karakteri atandý: " + starterChamp.characterName);
            }
            else
            {
                if (allChampions.Count > 0)
                {
                    allChampions[0].isUnlocked = true;
                    GameManager.instance.selectedTeam[0] = allChampions[0];
                }
            }
        }

    }


#if UNITY_EDITOR 
    [UnityEditor.MenuItem("Tools/Riff Lord/Delete All Save Data")]
    public static void DeleteAllSaveData()
    {
        PlayerPrefs.DeleteAll();
    }
#endif
}

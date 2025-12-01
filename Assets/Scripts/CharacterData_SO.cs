using UnityEngine;
public enum CharacterRole { Melee, Ranged, Support }

[CreateAssetMenu(fileName = "New Character Data", menuName = "Riff Lord/Character Data")]

public class CharacterData_SO : ScriptableObject
{
    

    [Header("Character Info")]
    public string characterName;
    public GameObject characterPrefab;

    public CharacterRole role;

    [Header("Unlock Info")]
    public bool isUnlocked = false; 
    public int unlockCost = 500;

    [TextArea(4, 8)]
    public string characterDescription;

    [Header("Original Base Stats")]
    public int originalHealth = 100;
    public int originalAttack = 10;
    public int originalDefense = 5;
    public int originalSpeed = 20;

    [Header("Base Stats")]
    public int baseHealth = 100;
    public int baseAttack = 10;
    [Range(0, 90)]
    public int baseDefense = 5;
    public int baseSpeed = 20;

    [Header("Audio")]
    public AudioClip attackSFX;
    public AudioClip superSFX;
    public AudioClip hurtSFX;
    public AudioClip deathSFX;

    [Header("Visual Settings")]
    [Tooltip("Y axis.")]
    public float spawnHeightOffset = 0f;
    [Header("Visual Effects")]
    public CombatVFX_SO attackVFX;

    [Tooltip("Y Rotation.")]
    [Range(0, 360)]
    public float spawnRotationY = 90f;

    [Tooltip("Character Specific Animation Override Controller.")]
    public AnimatorOverrideController animatorOverride;

    public void RecalculateStats()
    {
        int levelsGained = level - 1;

        baseHealth = originalHealth + (levelsGained * healthGrowth);
        baseAttack = originalAttack + (levelsGained * attackGrowth);
        baseDefense = originalDefense + (levelsGained * defenseGrowth);
        baseSpeed = originalSpeed + (levelsGained * speedGrowth);
    }

    [Header("Special Ability")]
    public SpecialAbility specialAbility;

    [Header("Progression")]
    public int level = 1;
    [Header("Growth Per Level")]
    public int healthGrowth = 25;
    public int attackGrowth = 3;
    public int defenseGrowth = 1;
    public int speedGrowth = 2; 
    [Header("Cost Per Level")]
    public int baseUpgradeCost = 100;
    public int costIncreasePerLevel = 20;
}
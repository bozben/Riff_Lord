using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Riff Lord/Character Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Character Info")]
    public string characterName;
    public GameObject characterPrefab;

    [Header("Base Stats")]
    public int baseHealth = 100;
    public int baseAttack = 10;
    [Range(0, 90)]
    public int baseDefense = 5;
    public int baseSpeed = 20;

    [Header("Special Ability")]
    // Burasý en önemli deðiþiklik! Artýk bir sayý yerine,
    // bir "SpecialAbility" dosyasýný doðrudan buraya sürükleyeceðiz.
    public SpecialAbility specialAbility;
}
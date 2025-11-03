using UnityEngine;

public abstract class SpecialAbility : ScriptableObject
{
    [Header("Ability Info")]
    public string abilityName;
    [TextArea(3, 5)]
    public string abilityDescription;
    public int spCost = 100;

    public abstract void ActivateAbility(CharacterStats attacker, CharacterStats target);
}
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Ability", menuName = "Riff Lord/Abilities/Heal")]
public class HealAbility : SpecialAbility
{
    public int healAmount = 50;

    public override void ActivateAbility(CharacterStats attacker, CharacterStats target)
    {
        attacker.Heal(healAmount);

        Debug.Log(attacker.name + ", " + abilityName + " ability used. healed by " + healAmount);
    }
}
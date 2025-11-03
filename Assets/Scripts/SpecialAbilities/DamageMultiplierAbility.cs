using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Multiplier Ability", menuName = "Riff Lord/Abilities/Damage Multiplier")]
public class DamageMultiplierAbility : SpecialAbility
{
    public float multiplier = 2.5f;

    public override void ActivateAbility(CharacterStats attacker, CharacterStats target)
    {
        float rawDamage = attacker.attackPower * multiplier;

        target.TakeDamage(Mathf.RoundToInt(rawDamage));

        Debug.Log(attacker.name + ", " + abilityName + " ability is used on " + target.name);
    }
}
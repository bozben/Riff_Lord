using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Ability", menuName = "Riff Lord/Abilities/Heal")]
public class HealAbility : SpecialAbility
{
    [Header("Heal Settings")]
    [Tooltip("Heal amount = Attack Power * Multiplier. (e.g., 1.5 = 150% of Attack)")]
    public float healMultiplier = 1.5f;


    public override void ActivateAbility(CharacterStats attacker, CharacterStats target)
    {
        int healAmount = Mathf.RoundToInt(attacker.attackPower * healMultiplier);

        target.Heal(healAmount);


        Debug.Log(attacker.characterData.characterName + " healed " + target.characterData.characterName + " for " + healAmount);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Buff Ability", menuName = "Riff Lord/Abilities/Stat Buff")]
public class StatBuffAbility : SpecialAbility
{
    [Header("Buff Settings")]
    [Tooltip("Percentage to increase (e.g., 0.2 = 20%, 0.5 = 50%).")]
    public float speedMultiplier = 0.2f;

    [Tooltip("How long the buff lasts (seconds).")]
    public float duration = 5f;

    [Header("Visuals")]
    public GameObject buffVFX;

    public override void ActivateAbility(CharacterStats attacker, CharacterStats target)
    {
        int amountToAdd = Mathf.RoundToInt(attacker.speed * speedMultiplier);

        if (amountToAdd < 1) amountToAdd = 1;

        attacker.ApplyTemporarySpeedBuff(amountToAdd, duration);

        if (buffVFX != null)
        {
            Instantiate(buffVFX, attacker.transform.position, Quaternion.identity, attacker.transform);
        }
    }
}
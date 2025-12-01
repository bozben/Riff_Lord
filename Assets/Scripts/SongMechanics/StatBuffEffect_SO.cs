using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Buff Effect", menuName = "Riff Lord/Effects/Stat Buff")]
public class StatBuffEffect_SO : RhythmEffect_SO
{
    public enum Target { Player, Enemy }
    public enum StatType { Speed, Attack, Defense }

    [Header("Effect Settings")]
    public Target target;
    public StatType statToModify;


    [Tooltip("Effect duration")]
    public float duration;

    public override void ApplyEffect(CharacterStats player, CharacterStats enemy)
    {
    }
}
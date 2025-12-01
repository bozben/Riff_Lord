using UnityEngine;

public abstract class RhythmEffect_SO : ScriptableObject
{
    [Header("UI Info")]
    public string effectName;
    [TextArea(2, 4)]
    public string effectDescription;
    public abstract void ApplyEffect(CharacterStats player, CharacterStats enemy);
}
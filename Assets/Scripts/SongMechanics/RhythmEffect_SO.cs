using UnityEngine;

public abstract class RhythmEffect_SO : ScriptableObject
{
    public abstract void ApplyEffect(CharacterStats player, CharacterStats enemy);
}
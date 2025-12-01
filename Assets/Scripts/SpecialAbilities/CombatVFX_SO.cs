using UnityEngine;

public enum VFXType { Instant, Projectile }
public enum VFXSpawnLocation { Self, Target, Center }

[CreateAssetMenu(fileName = "New Combat VFX", menuName = "Riff Lord/Combat VFX")]
public class CombatVFX_SO : ScriptableObject
{
    [Header("Visuals")]
    [Tooltip("FX or projectile prefab")]
    public GameObject vfxPrefab;

    [Tooltip("Hit vfx if its a projectile.")]
    public GameObject hitVFXPrefab;

    [Header("Settings")]
    public VFXType type;
    public VFXSpawnLocation spawnLocation;

    [Tooltip("Speed if projectile duration if Instant effect.")]
    public float speedOrDuration = 10f;

    [Tooltip("Position fine adjustment.")]
    public Vector3 spawnOffset;

    [Tooltip("Delay after animation started")]
    public float spawnDelay = 0.5f;

    [Tooltip("SoundEffect (Optional).")]
    public AudioClip playSoundOnSpawn;

}
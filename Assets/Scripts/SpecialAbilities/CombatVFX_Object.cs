using UnityEngine;
using System;

public class CombatVFX_Object : MonoBehaviour
{
    private CombatVFX_SO data;
    private Transform target;
    private Action onHitCallback;
    private bool hasHit = false;


    public void Setup(CombatVFX_SO vfxData, Transform targetTransform, Action callback)
    {
        data = vfxData;
        target = targetTransform;
        onHitCallback = callback;

        if (data.playSoundOnSpawn != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(data.playSoundOnSpawn);
        }

        if (data.type == VFXType.Instant)
        {
            if (onHitCallback != null) onHitCallback.Invoke();

            Destroy(gameObject, data.speedOrDuration);
        }
    }

    void Update()
    {
        if (data == null || data.type != VFXType.Projectile) return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = target.position + new Vector3(0, 1f, 0);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, data.speedOrDuration * Time.deltaTime);
        transform.LookAt(targetPos);

        if (Vector3.Distance(transform.position, targetPos) < 0.2f && !hasHit)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        hasHit = true;

        if (data.hitVFXPrefab != null)
        {
            Instantiate(data.hitVFXPrefab, transform.position, Quaternion.identity);
        }

        if (onHitCallback != null)
        {
            onHitCallback.Invoke();
        }

        Destroy(gameObject);
    }
}
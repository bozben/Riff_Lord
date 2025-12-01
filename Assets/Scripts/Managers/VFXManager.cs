using UnityEngine;
using System;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    public void PlayVFX(CombatVFX_SO vfxData, Transform user, Transform target, Action onHitCallback)
    {
        if (vfxData == null)
        {
            onHitCallback?.Invoke();
            return;
        }

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        switch (vfxData.spawnLocation)
        {
            case VFXSpawnLocation.Self:
                spawnPos = user.position;
                spawnRot = user.rotation;
                break;
            case VFXSpawnLocation.Target:
                spawnPos = target.position;
                spawnRot = target.rotation;
                break;
            case VFXSpawnLocation.Center:
                spawnPos = Vector3.zero;
                break;
        }

        spawnPos += vfxData.spawnOffset;

        if (vfxData.vfxPrefab != null)
        {
            GameObject vfxObj = Instantiate(vfxData.vfxPrefab, spawnPos, spawnRot);

            CombatVFX_Object vfxScript = vfxObj.GetComponent<CombatVFX_Object>();
            if (vfxScript != null)
            {
                vfxScript.Setup(vfxData, target, onHitCallback);
            }
            else
            {
                onHitCallback?.Invoke();
                Destroy(vfxObj, 2f);
            }
        }
        else
        {
            onHitCallback?.Invoke();
        }
    }
}
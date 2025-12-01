using UnityEngine;

public class PlayerCurrencyManager : MonoBehaviour
{
    public int currentGold;
    public int currentSoul;

    public static PlayerCurrencyManager instance;
    private void Awake()
    {
        if(instance != null && instance != this )
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void AddGold(int gold)
    {
        currentGold += gold;
    }

    public void RemoveGold(int gold)
    {
        currentGold -= gold;
        if (currentGold < 0)
        {
            currentGold = 0;
        }
        
    }

    public void AddSoul(int soul)
    {
        currentSoul += soul;
    }

    public void RemoveSoul(int soul)
    {
        currentSoul -= soul;
        if (currentSoul < 0)
        {
            currentSoul = 0;
        }
    }
    
}

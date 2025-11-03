using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;
    public int currentHealth;
    public int currentSP;


    public int maxHealth;
    public int attackPower;
    public int defense;
    public int speed;
    public int maxSP;


    private void Awake()
    {
        if (characterData == null)
        {
            Debug.LogError(gameObject.name + " has no character data.");
            return;
        }
        SetupStats();
    }

    public void SetupStats()
    {
        maxHealth = characterData.baseHealth;
        currentHealth = maxHealth;

        attackPower = characterData.baseAttack;
        defense = characterData.baseDefense;
        speed = characterData.baseSpeed;
        if (characterData.specialAbility != null)
        {
            maxSP = characterData.specialAbility.spCost;
        }
        else
        {
            
            maxSP = 0;
            Debug.LogWarning(characterData.characterName + " has no special ability");
        }
        currentSP = 0;
    }

    public void TakeDamage(int damage)
    {
        float defenseMultiplier = (float)defense / 100f;
        float damageReduction = damage * defenseMultiplier;
        int damageToTake = Mathf.RoundToInt(damage - damageReduction);

        // adjust min damage
        damageToTake = Mathf.Max(1, damageToTake);

        currentHealth -= damageToTake;
        Debug.Log(transform.name + ", " + defense + "% def " + " reduce " + damage + " to " + damageToTake);


        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log(transform.name + " " + amount + "HP healed");
    }
    private void Die()
    {
        Debug.Log(transform.name + " dead!");
    }
    public void GainSP(int amount)
    {
        currentSP = Mathf.Clamp(currentSP + amount, 0, maxSP);
        Debug.Log(transform.name + " " + amount + " SP gained! Current SP: " + currentSP);
    }

    public bool HasEnoughSPForSpecial()
    {
        return currentSP >= maxSP;
    }

    public void UseSpecialAttack()
    {
        currentSP = 0;
        Debug.Log(transform.name + " used special attack");
    }
}

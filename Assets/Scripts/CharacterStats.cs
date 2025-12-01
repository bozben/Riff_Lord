using System.Collections;
using TMPro;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    private Animator animator;

    public CharacterData_SO characterData;

    private TextMeshPro currentLevelTextObj;
    [Header("Level Settings")]
    public int manualLevel = 0;

    public int currentHealth;
    public int currentSP;

    public int maxHealth;
    public int attackPower;
    public int defense;
    public int speed;
    public int maxSP;

    [Header("Level Indicator Settings")]
    public GameObject levelTextPrefab;
    public Vector3 textOffset = new Vector3(0, 2.5f, 0);
    public Vector3 textRotation = Vector3.zero;

    [Header("Visuals")]
    public GameObject floatingTextPrefab;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (characterData == null)
        {
            Debug.LogError(gameObject.name + " has no character data.");
            return;
        }
        SetupStats();
    }

    public void SetupStats()
    {
        int levelToUse = characterData.level;
        if (manualLevel > 0)
        {
            levelToUse = manualLevel;
        }
        int levelsGained = levelToUse - 1;
        maxHealth = characterData.originalHealth + (levelsGained * characterData.healthGrowth);
        attackPower = characterData.originalAttack + (levelsGained * characterData.attackGrowth);
        defense = characterData.originalDefense + (levelsGained * characterData.defenseGrowth);
        speed = characterData.originalSpeed + (levelsGained * characterData.speedGrowth);
        currentHealth = maxHealth;
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
        SpawnLevelText(levelToUse);
    }
    private void SpawnLevelText(int lvl)
    {
        if (currentLevelTextObj != null)
        {
            currentLevelTextObj.text = "Lv." + lvl;
            return;
        }
        if (levelTextPrefab != null)
        {
            GameObject txtObj = Instantiate(levelTextPrefab, transform);
            txtObj.transform.localPosition = textOffset;
            txtObj.transform.rotation = Quaternion.Euler(textRotation);
            currentLevelTextObj = txtObj.GetComponent<TextMeshPro>();
            if (currentLevelTextObj != null)
            {
                currentLevelTextObj.text = "Lv." + lvl;
            }
        }
    }
    public void TakeDamage(int damage)
    {
        float defenseMultiplier = (float)defense / 100f;
        float damageReduction = damage * defenseMultiplier;
        int damageToTake = Mathf.RoundToInt(damage - damageReduction);

        // adjust min damage
        damageToTake = Mathf.Max(1, damageToTake);

        
        currentHealth -= damageToTake;
        ShowFloatingText(damageToTake, false);
        Debug.Log(transform.name + ", " + defense + "% def " + " reduce " + damage + " to " + damageToTake);
        if (currentHealth > 0)
        {
            if (animator != null)
            {
                AudioManager.instance.PlaySFX(characterData.hurtSFX);
                animator.SetTrigger("Hurt");
            }
        }

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
        ShowFloatingText(amount, true);
    }
    private void Die()
    {
        if (animator != null)
        {
            AudioManager.instance.PlaySFX(characterData.deathSFX);
            animator.SetTrigger("Die");
        }
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
    public void ApplyTemporarySpeedBuff(int amount, float duration)
    {
        StartCoroutine(SpeedBuffRoutine(amount, duration));
    }

    private IEnumerator SpeedBuffRoutine(int amount, float duration)
    {
        speed += amount;
        Debug.Log(transform.name + " speed increased by " + amount);

        yield return new WaitForSeconds(duration);

        speed -= amount;
        Debug.Log(transform.name + " speed returned to normal.");
    }
    private void ShowFloatingText(int amount, bool isHeal)
    {
        if (floatingTextPrefab != null)
        {
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = Random.Range(1.5f, 2.5f);
            float zOffset = -1.0f;

            Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, zOffset);

            GameObject ftGO = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            ftGO.GetComponent<FloatingText>().Setup(amount, isHeal);
        }
    }
    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        if (animator != null)
        {
            animator.Play("Walk");
        }

        Vector3 startPosition = transform.position;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;

        if (animator != null)
        {
            animator.CrossFade("Idle", 0.1f);
        }
    }
    
}

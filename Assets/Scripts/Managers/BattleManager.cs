using System.Collections;
using UnityEngine;

public enum BattleState { START, WAITING, ACTION, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    [Header("Battle State")]
    public BattleState state;

    [Header("Combatants")]
    public GameObject championPrefab;
    public GameObject heroPrefab;

    public Transform championSpawnPoint;
    public Transform heroSpawnPoint;

    private CharacterStats championStats;
    private ATB_Controller championATB;

    private CharacterStats heroStats;
    private ATB_Controller heroATB;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        if (state == BattleState.WAITING)
        {
            if (championATB.isTurnReady)
            {
                state = BattleState.ACTION;
                StartCoroutine(CharacterTurn(championStats, championATB, heroStats, heroATB));
            }
            else if (heroATB.isTurnReady)
            {
                state = BattleState.ACTION;
                StartCoroutine(CharacterTurn(heroStats, heroATB, championStats, championATB));
            }
        }
    }

    IEnumerator SetupBattle()
    {
        GameObject championGO = Instantiate(championPrefab, championSpawnPoint);
        GameObject heroGO = Instantiate(heroPrefab, heroSpawnPoint);

        championStats = championGO.GetComponent<CharacterStats>();
        championATB = championGO.GetComponent<ATB_Controller>();

        heroStats = heroGO.GetComponent<CharacterStats>();
        heroATB = heroGO.GetComponent<ATB_Controller>();

        Debug.Log("Battle Begin: " + championStats.characterData.characterName + " vs " + heroStats.characterData.characterName);

        yield return new WaitForSeconds(1f);

        state = BattleState.WAITING;
    }

    IEnumerator CharacterTurn(CharacterStats attacker, ATB_Controller attackerATB, CharacterStats target, ATB_Controller targetATB)
    {
        Debug.Log(attacker.characterData.characterName + "'s turn...");

        yield return new WaitForSeconds(1f);

        if (attacker.HasEnoughSPForSpecial())
        {
            Debug.Log(attacker.characterData.characterName + " Speacial Ability: " + attacker.characterData.specialAbility.abilityName);
            attacker.characterData.specialAbility.ActivateAbility(attacker, target);
            attacker.UseSpecialAttack(); 
        }
        else
        {
            // --- TEMEL SALDIRI ---
            Debug.Log(attacker.characterData.characterName + " attacks!");
            target.TakeDamage(attacker.attackPower);
            attacker.GainSP(25); 
        }

        yield return new WaitForSeconds(1.5f);

        if (target.currentHealth <= 0)
        {
            state = (target == heroStats) ? BattleState.WON : BattleState.LOST;
            EndBattle();
        }
        else
        {
            attackerATB.ResetATB();
            state = BattleState.WAITING;
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            Debug.Log("Won!");
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("Lose!");
        }
    }
}
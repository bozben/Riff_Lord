using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, WAITING, ACTION, ANIMATING, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject victoryPanel;
    [Header("Battle State")]
    public BattleState state;

    [Header("Combatants")]
    public Transform[] championSpawnPoints; 
    public Transform[] heroSpawnPoints;

    [Header("UI")]
    public Battle_UI battleUI;
    public Shop_UI shopUI;

    [Header("Post-Battle UI")]
    public GameObject postBattlePanel;
    public TextMeshProUGUI postBattleTitleText;
    public TextMeshProUGUI postBattleSurvivedText;
    public TextMeshProUGUI postBattleDamageText;
    public TextMeshProUGUI postBattleComboText;
    public TextMeshProUGUI postBattleGoldText;
    public TextMeshProUGUI postBattleSoulText;
    public Button postBattleContinueButton;

    [Header("Intro Settings")]
    public GateController gateController; 
    public Transform gateSpawnPoint; 
    public Transform championOffScreenPoint;
    public float walkSpeed = 5f;

    private Dictionary<CharacterStats, int> damageDealtByChampion = new Dictionary<CharacterStats, int>();

    private List<CharacterStats> championStatsList = new List<CharacterStats>();
    private List<ATB_Controller> championATBList = new List<ATB_Controller>();
    private List<Animator> championAnimators = new List<Animator>();

    private List<CharacterStats> heroStatsList = new List<CharacterStats>();
    private List<ATB_Controller> heroATBList = new List<ATB_Controller>();
    private List<Animator> heroAnimators = new List<Animator>();

    private WaveData_SO currentWave;
    private int totalEnemyHealthInWave;
    private Coroutine speedBuffCoroutine;
    private float survivalTime = 0f;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }



    void Update()
    {
        if (state == BattleState.WAITING || state == BattleState.ACTION || state == BattleState.ANIMATING)
        {
            survivalTime += Time.deltaTime;
        }

        if (state == BattleState.WAITING)
        {
            foreach (ATB_Controller champATB in championATBList)
            {
                if (champATB.isTurnReady)
                {
                    CharacterStats attacker = champATB.GetComponent<CharacterStats>();
                    StartCoroutine(CharacterTurn(attacker, champATB));
                    return;
                }
            }

            foreach (ATB_Controller heroATB in heroATBList)
            {
                if (heroATB.isTurnReady)
                {
                    CharacterStats attacker = heroATB.GetComponent<CharacterStats>();
                    StartCoroutine(CharacterTurn(attacker, heroATB));
                    return;
                }
            }
        }

        if (championStatsList.Count > 0 && heroStatsList.Count > 0)
        {
            battleUI.UpdateChampionUI(championStatsList[0], championATBList[0]);
            battleUI.UpdateHeroUI(heroStatsList[0], heroATBList[0]);
        }
    }

    IEnumerator SetupBattle()
    {
        survivalTime = 0f;

        currentWave = GameManager.instance.GetCurrentWave();
        if (currentWave.waveMusic != null)
        {
            AudioManager.instance.PlayMusic(currentWave.waveMusic);
        }
        if (currentWave == null)
        {
            Debug.LogError("Wave info couldn't be found");
            yield break;
        }

        championStatsList.Clear(); championATBList.Clear(); championAnimators.Clear();
        heroStatsList.Clear(); heroATBList.Clear(); heroAnimators.Clear();

        GameObject[] oldHeroes = GameObject.FindGameObjectsWithTag("Hero");
        foreach (GameObject oldHero in oldHeroes) { Destroy(oldHero); }
        foreach (Transform spawnPoint in championSpawnPoints) { foreach (Transform child in spawnPoint) Destroy(child.gameObject); }

        damageDealtByChampion.Clear();


        List<CharacterData_SO> myTeam = GameManager.instance.selectedTeam;
        for (int i = 0; i < myTeam.Count; i++)
        {
            if (i >= championSpawnPoints.Length) break;

            CharacterData_SO champData = myTeam[i];
            if (champData == null) continue;

            Vector3 startPos = championSpawnPoints[i].position;
            startPos.x = -15f; 
            startPos.y += champData.spawnHeightOffset; 

            Quaternion startRot = Quaternion.Euler(0, champData.spawnRotationY, 0);

            GameObject champGO = Instantiate(champData.characterPrefab, startPos, startRot);

            CharacterStats stats = champGO.GetComponent<CharacterStats>();
            stats.characterData = champData;
            stats.SetupStats();

            ATB_Controller atb = champGO.GetComponent<ATB_Controller>();
            Animator anim = champGO.GetComponentInChildren<Animator>();
            if (stats.characterData.animatorOverride != null) anim.runtimeAnimatorController = stats.characterData.animatorOverride;

            championStatsList.Add(stats);
            championATBList.Add(atb);
            championAnimators.Add(anim);
            damageDealtByChampion.Add(stats, 0);
        }

        totalEnemyHealthInWave = 0;
        for (int i = 0; i < currentWave.enemiesInWave.Count; i++)
        {
            if (i >= heroSpawnPoints.Length) break;

            WaveEnemyData waveEntry = currentWave.enemiesInWave[i];
            int targetLevel = waveEntry.level;

            Transform spawnPoint = heroSpawnPoints[i];
            CharacterData_SO enemyData = waveEntry.enemyData;

            Vector3 gatePos = heroSpawnPoints[i].position;
            gatePos.x = 12f; 
            gatePos.y += enemyData.spawnHeightOffset;

            Quaternion startRot = Quaternion.Euler(0, enemyData.spawnRotationY, 0);

            GameObject heroGO = Instantiate(enemyData.characterPrefab, gatePos, startRot);
            heroGO.tag = "Hero";

            CharacterStats stats = heroGO.GetComponent<CharacterStats>();
            stats.characterData = enemyData;
            stats.manualLevel = targetLevel;
            stats.SetupStats();
            ATB_Controller atb = heroGO.GetComponent<ATB_Controller>();
            Animator anim = heroGO.GetComponentInChildren<Animator>();
            if (stats.characterData.animatorOverride != null) anim.runtimeAnimatorController = stats.characterData.animatorOverride;

            heroStatsList.Add(stats);
            heroATBList.Add(atb);
            heroAnimators.Add(anim);
            totalEnemyHealthInWave += stats.maxHealth;
        }

        GateController gate = FindFirstObjectByType<GateController>();
        if (gate != null)
        {
            yield return StartCoroutine(gate.OpenGateRoutine());
        }
        else
        {
            Debug.LogWarning("GateController bulunamadý, kapý açýlmadan devam ediliyor.");
            yield return new WaitForSeconds(1f);
        }

        List<Coroutine> walkRoutines = new List<Coroutine>();
        StartCoroutine(ShowWaveBannerRoutine());

        // Champ Walk
        for (int i = 0; i < championStatsList.Count; i++)
        {
            Vector3 targetPos = championSpawnPoints[i].position;
            targetPos.y += championStatsList[i].characterData.spawnHeightOffset;

            float distance = Vector3.Distance(championStatsList[i].transform.position, targetPos);
            float calculatedDuration = distance / walkSpeed;

            walkRoutines.Add(StartCoroutine(championStatsList[i].MoveToPosition(targetPos, calculatedDuration)));

            yield return new WaitForSeconds(0.5f);
        }

        // Hero Walk
        for (int i = 0; i < heroStatsList.Count; i++)
        {
            Vector3 targetPos = heroSpawnPoints[i].position;
            targetPos.y += heroStatsList[i].characterData.spawnHeightOffset;

            float distance = Vector3.Distance(heroStatsList[i].transform.position, targetPos);
            float calculatedDuration = distance / walkSpeed;

            walkRoutines.Add(StartCoroutine(heroStatsList[i].MoveToPosition(targetPos, calculatedDuration)));

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(2.5f);

        if (gate != null)
        {
            StartCoroutine(gate.CloseGateRoutine());
        }

        Debug.Log("Fight started: Wave : " + (GameManager.instance.currentWaveIndex + 1) + " - " + currentWave.waveName);

        if (championStatsList.Count > 0 && heroStatsList.Count > 0)
        {
            battleUI.SetupUI(championStatsList[0], heroStatsList[0]);
        }
        RhythmManager rhythmManager = FindFirstObjectByType<RhythmManager>();
        if (rhythmManager != null)
        {
            rhythmManager.BeginRhythmGame();
        }

        state = BattleState.WAITING;
    }

    IEnumerator CharacterTurn(CharacterStats attacker, ATB_Controller attackerATB)
    {
        state = BattleState.ACTION;
        PauseAllATBs(true);

        Debug.Log(attacker.characterData.characterName + "'s turn...");

        List<CharacterStats> myTeam = null;
        List<CharacterStats> enemyTeam = null;
        List<Animator> myAnimators = null;

        bool isAttackerChampion = championStatsList.Contains(attacker);

        if (isAttackerChampion)
        {
            myTeam = championStatsList;
            enemyTeam = heroStatsList;
            myAnimators = championAnimators;
        }
        else
        {
            myTeam = heroStatsList;
            enemyTeam = championStatsList;
            myAnimators = heroAnimators;
        }

        CharacterStats target = null;
        bool isHealingAction = false;

        int attackerIndex = myTeam.IndexOf(attacker);
        CharacterRole role = attacker.characterData.role;

        if (role == CharacterRole.Support)
        {
            if (attackerIndex == 0)
            {
                role = CharacterRole.Melee;
            }
            else
            {
                isHealingAction = true;
                target = myTeam.Where(c => c.currentHealth > 0).OrderBy(c => c.currentHealth).FirstOrDefault();
            }
        }

        if (role == CharacterRole.Melee)
        {
            if (attackerIndex == 0)
            {
                target = enemyTeam.FirstOrDefault(c => c.currentHealth > 0);
            }
            else
            {
                target = null;
            }
        }
        else if (role == CharacterRole.Ranged)
        {
            var livingEnemies = enemyTeam.Where(c => c.currentHealth > 0).ToList();
            if (livingEnemies.Count > 0)
            {
                target = livingEnemies[Random.Range(0, livingEnemies.Count)];
            }
        }

        if (target == null)
        {
            attackerATB.ResetATB();
            PauseAllATBs(false);
            state = BattleState.WAITING;
            yield break;
        }

        Animator attackerAnimator = myAnimators[attackerIndex];
        if (attackerAnimator != null)
        {
            string animTrigger = attacker.HasEnoughSPForSpecial() ? "Super" : "Attack";
            attackerAnimator.SetTrigger(animTrigger);
            if (attacker.HasEnoughSPForSpecial())
            {
                if (attacker.characterData.superSFX != null)
                    AudioManager.instance.PlaySFX(attacker.characterData.superSFX);
            }
            else
            {
                if (attacker.characterData.attackSFX != null)
                    AudioManager.instance.PlaySFX(attacker.characterData.attackSFX);
            }
        }

        bool actionCompleted = false;
        float waitTime = 0.5f; 

        if (attacker.HasEnoughSPForSpecial())
        {
            if (attacker.characterData.specialAbility.abilityVFX != null)
            {
                waitTime = attacker.characterData.specialAbility.abilityVFX.spawnDelay;
            }
        }
        else if (!isHealingAction)
        {
            if (attacker.characterData.attackVFX != null)
            {
                waitTime = attacker.characterData.attackVFX.spawnDelay;
            }
        }

        yield return new WaitForSeconds(waitTime);
        if (attacker.HasEnoughSPForSpecial())
        {
            VFXManager.instance.PlayVFX(attacker.characterData.specialAbility.abilityVFX, attacker.transform, target.transform, () =>
            {
                attacker.characterData.specialAbility.ActivateAbility(attacker, target);
                attacker.UseSpecialAttack();
                actionCompleted = true;
            });
        }
        else
        {
            if (isHealingAction)
            {
                VFXManager.instance.PlayVFX(null, attacker.transform, target.transform, () =>
                {
                    int healAmount = attacker.attackPower;
                    target.Heal(healAmount);
                    actionCompleted = true;
                });
            }
            else
            {
                VFXManager.instance.PlayVFX(attacker.characterData.attackVFX, attacker.transform, target.transform, () =>
                {
                    int damage = attacker.attackPower;
                    if (attacker.characterData.role == CharacterRole.Ranged && attackerIndex == 0)
                    {
                        damage = Mathf.RoundToInt(damage * 0.5f);
                    }

                    target.TakeDamage(damage);


                    if (isAttackerChampion && damageDealtByChampion.ContainsKey(attacker))
                    {
                        damageDealtByChampion[attacker] += damage;
                    }

                    attacker.GainSP(25);
                    actionCompleted = true;
                });
            }
        }

        float timeOut = 5.0f;
        while (!actionCompleted && timeOut > 0)
        {
            timeOut -= Time.deltaTime;
            yield return null; 
        }
        yield return new WaitForSeconds(1.0f);

        if (!isHealingAction && target.currentHealth <= 0)
        {

            List<CharacterStats> targetTeamList = (enemyTeam.Contains(target)) ? enemyTeam : myTeam;

            List<ATB_Controller> targetATBList = (targetTeamList == championStatsList) ? championATBList : heroATBList;
            List<Animator> targetAnimList = (targetTeamList == championStatsList) ? championAnimators : heroAnimators;
            Transform[] targetSpawnPoints = (targetTeamList == championStatsList) ? championSpawnPoints : heroSpawnPoints;

            int deadIndex = targetTeamList.IndexOf(target);

            if (deadIndex != -1)
            {
                targetTeamList.RemoveAt(deadIndex);
                targetATBList.RemoveAt(deadIndex);
                targetAnimList.RemoveAt(deadIndex);
                
                if (target != null)
                {
                    StartCoroutine(HandleEnemyDeathVisuals(target.gameObject));
                }
                yield return new WaitForSeconds(1.0f);

            }

            if (targetTeamList.Count > 0)
            {
                RepositionTeam(targetTeamList, targetSpawnPoints);
                yield return new WaitForSeconds(1.0f);
            }

            if (targetTeamList.Count == 0)
            {
                state = (targetTeamList == heroStatsList) ? BattleState.WON : BattleState.LOST;
                EndBattle();
                yield break;
            }
        }

        attackerATB.ResetATB();
        PauseAllATBs(false);
        state = BattleState.WAITING;
    }

    private void PauseAllATBs(bool isPaused)
    {
        foreach (var atb in championATBList) if (atb != null) atb.isPaused = isPaused;
        foreach (var atb in heroATBList) if (atb != null) atb.isPaused = isPaused;
    }

    void EndBattle()
    {
        PauseAllATBs(true);
        int soulGained = 0;
        int goldGained = 0;

        if (state == BattleState.WON)
        {
            postBattleTitleText.text = "VICTORY";
            goldGained = currentWave.rewards.maxGold;
            soulGained = currentWave.rewards.soulsOnVictory;
            goldGained += Mathf.RoundToInt(goldGained * 0.3f);

            GameManager.instance.PlayerWonWave();
        }
        else if (state == BattleState.LOST)
        {
            postBattleTitleText.text = "DEFEAT";
            soulGained = 0;
            float damagePercentage = (float)damageDealtByChampion.Values.Sum() / totalEnemyHealthInWave;
            if (damagePercentage >= 0.66f) goldGained = Mathf.RoundToInt(currentWave.rewards.maxGold * 0.66f);
            else if (damagePercentage >= 0.33f) goldGained = Mathf.RoundToInt(currentWave.rewards.maxGold * 0.33f);
            else goldGained = 0;
        }

        PlayerCurrencyManager.instance.AddGold(goldGained);
        PlayerCurrencyManager.instance.AddSoul(soulGained);

        postBattleSurvivedText.text = "Survived : " + Mathf.RoundToInt(survivalTime) + " seconds";
        ComboManager cm = FindFirstObjectByType<ComboManager>();
        if (cm != null) postBattleComboText.text = "Max Combo : x" + cm.GetPeakCombo();

        postBattleDamageText.text = "Total Damage : " + damageDealtByChampion.Values.Sum();
        postBattleGoldText.text = "Gold Earned : " + goldGained;
        postBattleSoulText.text = "Soul Earned : " + soulGained;
        postBattlePanel.SetActive(true);
    }
    public void OnContinueAfterBattle()
    {
        postBattlePanel.SetActive(false);

        if (GameManager.instance.currentWaveIndex >= GameManager.instance.waves.Count)
        {
            if (AudioManager.instance != null) AudioManager.instance.musicSource.Stop();
            Debug.Log("Game Won!!!");
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);

            }
        }
        else
        {
            if (AudioManager.instance != null) AudioManager.instance.musicSource.Stop();
            if (shopUI != null)
            {
                shopUI.OpenShop();
            }
        }
    }

    private void ApplyRhythmBuff() { }
    private IEnumerator StatChangeRoutine(StatBuffEffect_SO buff, int overrideAmount)
    {
        CharacterStats targetStats = null;

        if (buff.target == StatBuffEffect_SO.Target.Player)
        {
            targetStats = championStatsList.FirstOrDefault(c => c != null && c.currentHealth > 0);
        }
        else
        {
            targetStats = heroStatsList.FirstOrDefault(h => h != null && h.currentHealth > 0);
        }

        if (targetStats == null) yield break;

        int originalValue = 0;
        switch (buff.statToModify)
        {
            case StatBuffEffect_SO.StatType.Speed: originalValue = targetStats.characterData.baseSpeed; break;
            case StatBuffEffect_SO.StatType.Attack: originalValue = targetStats.characterData.baseAttack; break;
            case StatBuffEffect_SO.StatType.Defense: originalValue = targetStats.characterData.baseDefense; break;
        }

        int modifiedValue = originalValue + (originalValue * overrideAmount / 100);

        switch (buff.statToModify)
        {
            case StatBuffEffect_SO.StatType.Speed: targetStats.speed = modifiedValue; break;
            case StatBuffEffect_SO.StatType.Attack: targetStats.attackPower = modifiedValue; break;
            case StatBuffEffect_SO.StatType.Defense: targetStats.defense = modifiedValue; break;
        }

        Debug.Log(targetStats.name + " buffed: " + buff.statToModify);

        yield return new WaitForSeconds(buff.duration);

        switch (buff.statToModify)
        {
            case StatBuffEffect_SO.StatType.Speed: targetStats.speed = originalValue; break;
            case StatBuffEffect_SO.StatType.Attack: targetStats.attackPower = originalValue; break;
            case StatBuffEffect_SO.StatType.Defense: targetStats.defense = originalValue; break;
        }
    }

    private void RepositionTeam(List<CharacterStats> teamList, Transform[] spawnPoints)
    {
        for (int i = 0; i < teamList.Count; i++)
        {
            Vector3 newTargetPos = spawnPoints[i].position;

            newTargetPos.y += teamList[i].characterData.spawnHeightOffset;

            float distance = Vector3.Distance(teamList[i].transform.position, newTargetPos);
            if (distance < 0.1f) continue;

            float moveDuration = distance / (walkSpeed * 1.5f);
            StartCoroutine(teamList[i].MoveToPosition(newTargetPos, moveDuration));
        }
    }

    private IEnumerator HandleEnemyDeathVisuals(GameObject targetGO)
    {
        yield return new WaitForSeconds(1.0f);

        float t = 0;
        Vector3 originalScale = targetGO.transform.localScale;

        while (t < 1)
        {
            t += Time.deltaTime * 2; 
            targetGO.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(targetGO);
    }

    private void OnEnable()
    {
        if (RhythmEvents.instance != null)
        {
            RhythmEvents.instance.OnNoteHit += ApplyRhythmBuff; 
        }
    }
    private void OnDisable()
    {
        if (RhythmEvents.instance != null)
        {
            RhythmEvents.instance.OnNoteHit -= ApplyRhythmBuff;
        }
    }

    public void ApplyDynamicEffect(StatBuffEffect_SO buffRef, int amount)
    {
        StartCoroutine(StatChangeRoutine(buffRef, amount));
    }
    private IEnumerator ShowWaveBannerRoutine()
    {
        int current = GameManager.instance.currentWaveIndex + 1;
        int total = GameManager.instance.waves.Count;

        if (battleUI.waveTitleText != null)
        {
            battleUI.waveTitleText.text = "WAVE " + current + " / " + total;

            CanvasGroup cg = battleUI.waveTitleText.GetComponent<CanvasGroup>();
            if (cg == null) cg = battleUI.waveTitleText.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            battleUI.waveTitleText.gameObject.SetActive(true);

            float duration = 0.5f;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, timer / duration);
                yield return null;
            }
            cg.alpha = 1f;

            yield return new WaitForSeconds(2.0f);

            timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, timer / duration);
                yield return null;
            }
            cg.alpha = 0f;

            battleUI.waveTitleText.gameObject.SetActive(false);
        }
    }

}
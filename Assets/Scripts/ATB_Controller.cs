using UnityEngine;
[RequireComponent(typeof(CharacterStats))]
public class ATB_Controller : MonoBehaviour
{
    private CharacterStats characterStats;

    public float maxATB = 100f;
    public float currentATB = 0f;

    [HideInInspector]
    public bool isTurnReady = false;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        if (isTurnReady)
        {
            return;
        }
        currentATB += characterStats.speed * Time.deltaTime;

        if (currentATB >= maxATB)
        {
            currentATB = maxATB;
            isTurnReady = true;

            Debug.Log(characterStats.characterData.characterName + "'s turn!");
        }
    }
    public void ResetATB()
    {
        currentATB = 0f;
        isTurnReady = false;
    }
}

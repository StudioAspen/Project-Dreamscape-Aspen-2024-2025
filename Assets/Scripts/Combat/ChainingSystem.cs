using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChainingSystem : MonoBehaviour
{
    private PlayerCombat playerCombat;
    private LevelSystem levelSystem;

    [Header("Settings")]
    [SerializeField] private float timeBetween = 1f;
    private float timer;

    [Header("Rewards")]
    /// <summary>
    /// Dictionary that stores the milestones (int) as keys and bonuses (float) as values. Upon reaching a milestone, the corresponding bonus is applied.
    /// </summary>
    [SerializeField, SerializedDictionary("Chain Milestone", "% EXP Bonus")] private SerializedDictionary<int, float> chainRewards = new();

    public int ChainCount { get; private set; }

    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        levelSystem = GetComponent<LevelSystem>();
    }

    private void Start()
    {
        ResetChain();
    }

    private void OnEnable()
    {
        levelSystem.OnEXPAdded += LevelSystem_OnEXPAdded;
        playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
    }

    private void OnDisable()
    {
        levelSystem.OnEXPAdded -= LevelSystem_OnEXPAdded;
        playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;
    }

    private void Update()
    {
        HandleChaining();
    }

    private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        AddChain();
    }

    private void HandleChaining()
    {
        if (ChainCount > 0)
        {
            timer += Time.deltaTime;
        }

        if (timer > timeBetween)
        {
            ResetChain();
        }
    }

    public void AddChain()
    {
        ChainCount++;
        timer = 0f;
    }

    private void ResetChain()
    {
        ChainCount = 0;
        timer = 0;
    }

    private void LevelSystem_OnEXPAdded(int addedAmount)
    {
        // Just in case the dictionary is empty
        if(chainRewards.Count == 0)
        {
            Debug.LogWarning("You need to configure chain rewards for the Player's chaining system");
            return;
        }

        if (ChainCount == 0) return; // No need to check for rewards if there is no chain

        List<int> milestones = new List<int>(chainRewards.Keys);
        milestones.Sort(); // Ensures the milestones are in ascending order

        // Finds the largest milestone that is less than or equal to the chain count
        int currentMilestone = 0;
        foreach(int milestone in milestones)
        {
            if (milestone > ChainCount) break;
            currentMilestone = milestone;
        }

        // There should be no reward for not reaching any milestone
        if (currentMilestone == 0) return;

        // Get the bonus multiplier and add bonus exp if nonzero
        float bonusEXPMultiplier = chainRewards[currentMilestone];
        int bonusEXP = Mathf.RoundToInt(bonusEXPMultiplier * addedAmount);
        if (bonusEXP > 0) levelSystem.AddEXP(bonusEXP, false); // False because you dont want to cause an infinite loop of adding EXP and giving bonus EXP

        //Debug.Log($"Bonus EXP Multiplier: {bonusEXPMultiplier}, Bonus EXP: {bonusEXP}");
    }
}

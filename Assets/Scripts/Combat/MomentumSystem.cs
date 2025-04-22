using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;

public class MomentumSystem : MonoBehaviour
{
    private Player player;
    private LevelSystem levelSystem;

    [Header("Settings")]
    [SerializeField] private float timeBetween = 1f;
    private float timer;

    [Header("Rewards")]
    /// <summary>
    /// Dictionary that stores the milestones (int) as keys and bonuses (float) as values. Upon reaching a milestone, the corresponding bonus is applied.
    /// </summary>
    [SerializeField, SerializedDictionary("Chain Milestone", "% EXP Bonus")] private SerializedDictionary<int, float> chainRewards = new();

    public int Momentum { get; private set; }
    public Action<int> OnMomentumUpdated;

    private void Awake()
    {
        player = GetComponent<Player>();
        levelSystem = GetComponent<LevelSystem>();
    }

    private void Start()
    {
        ResetMomentum();
    }

    private void OnEnable()
    {
        player.OnEntityTakeDamage += Player_OnEntityTakeDamage;
        player.OnKillEntity += Player_OnKillEntity;
        levelSystem.OnEXPAdded += LevelSystem_OnEXPAdded;
    }

    private void OnDisable()
    {
        player.OnEntityTakeDamage -= Player_OnEntityTakeDamage;
        player.OnKillEntity -= Player_OnKillEntity;
        levelSystem.OnEXPAdded -= LevelSystem_OnEXPAdded;
    }

    void Update()
    {
        HandleMomentum();
    }

    // Momentum resets to 0 when the player takes damage.
    private void Player_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source) => ResetMomentum();

    // Momentum increases by 1 every time the player kills an entity.
    private void Player_OnKillEntity(Entity victim) => AddMomentum();

    private void HandleMomentum()
    {
        if (Momentum > 0)
        {
            timer += Time.deltaTime;
        }
        if (timer > timeBetween)
        {
            ResetMomentum();
        }
    }

    public void AddMomentum()
    {
        Momentum++;
        OnMomentumUpdated?.Invoke(Momentum);
        timer = 0f;
    }

    private void ResetMomentum()
    {
        Momentum = 0;
        OnMomentumUpdated?.Invoke(Momentum);
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

        if (Momentum == 0) return; // No need to check for rewards if there is no chain

        List<int> milestones = new List<int>(chainRewards.Keys);
        milestones.Sort(); // Ensures the milestones are in ascending order

        // Finds the largest milestone that is less than or equal to the chain count
        int currentMilestone = 0;
        foreach(int milestone in milestones)
        {
            if (milestone > Momentum) break;
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

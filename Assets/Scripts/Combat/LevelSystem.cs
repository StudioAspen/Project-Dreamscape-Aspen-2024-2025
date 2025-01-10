using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    private Entity entity;

    [field: Header("Config")]
    [field: SerializeField] public int Level { get; protected set; } = 1;
    [SerializeField] private int baseMaxEXP = 10;
    [SerializeField] private int maxEXPLinearGrowth = 10;
    [SerializeField] private float maxEXPExponentialGrowth = 1.2f;

    public int CurrentEXP { get; private set; }
    public int MaxEXP { get; private set; }

    public Action OnLevelUp = delegate { };

    private void Awake()
    {
        entity = GetComponent<Entity>();

        MaxEXP = CalculateMaxEXP();

        entity.OnKillEntity += Entity_OnKillEntity;
    }

    private void OnDestroy()
    {
        entity.OnKillEntity -= Entity_OnKillEntity;
    }

    private void Entity_OnKillEntity(Entity entity)
    {
        AddEXP(9);
    }

    #region EXP Handling
    /// <summary>
    /// Adds the specified amount of experience points (EXP) to the current experience.
    /// If the current experience exceeds the maximum experience, triggers a level up.
    /// </summary>
    /// <param name="amount">The amount of experience points to add.</param>
    public void AddEXP(int amount)
    {
        CurrentEXP += amount;

        if (CurrentEXP >= MaxEXP)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Increases the level, handling any level up events.
    /// </summary>
    public void LevelUp()
    {
        OnLevelUp?.Invoke();

        CurrentEXP -= MaxEXP;
        Level++;
        MaxEXP = CalculateMaxEXP();

        AddEXP(0); // Check if the player has enough EXP to level up again
    }

    /// <summary>
    /// Calculates the maximum experience points (EXP) based on the current level.
    /// </summary>
    /// <returns>The maximum EXP for the current level.</returns>
    private int CalculateMaxEXP()
    {
        int linearGrowth = maxEXPLinearGrowth * (Level - 1);

        int useExponentialGrowthMultiplier = (Level <= 1) ? 0 : 1; // If the level is at most 1, don't use exponential growth
        int exponentialGrowth = useExponentialGrowthMultiplier * (int)Mathf.Pow(maxEXPExponentialGrowth, Level - 1);

        return baseMaxEXP + linearGrowth + exponentialGrowth;
    }    
    #endregion
}
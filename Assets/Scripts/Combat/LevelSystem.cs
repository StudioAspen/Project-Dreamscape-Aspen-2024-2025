using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    private Entity entity;

    [Header("Config")]
    [SerializeField] private int baseMaxEXP = 10;
    [SerializeField] private int maxEXPLinearGrowth = 10;
    [SerializeField] private float maxEXPExponentialGrowth = 1.2f;

    public int Level { get; private set; } = 1;
    public int CurrentEXP { get; private set; }
    public int MaxEXP { get; private set; }

    /// <summary>
    /// Action that is invoked when the player levels up.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>int newLevel</c>: The new level after levelling up.</description></item>
    /// </list>
    /// </remarks>
    public Action<int> OnLevelUp = delegate { };
    /// <summary>
    /// Action that is invoked when the player gains EXP.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>int newLevel</c>: The amount of EXP added.</description></item>
    /// </list>
    /// </remarks>
    public Action<int> OnEXPAdded = delegate { };

    private void Awake()
    {
        entity = GetComponent<Entity>();

        CurrentEXP = 0;
        MaxEXP = CalculateMaxEXP();
        AddEXP(0);
    }

    private void OnEnable()
    {
        entity.OnKillEntity += Entity_OnKillEntity;
    }

    private void OnDisable()
    {
        entity.OnKillEntity -= Entity_OnKillEntity;
    }

    private void Entity_OnKillEntity(Entity victim)
    {
        if (Slime.IsEntityACloneSlime(victim)) return; // Cloned slimes dont drop exp

        Enemy victimAsEnemy = victim as Enemy;
        if(victimAsEnemy == null) return;

        int expReward = victimAsEnemy.EXPValue.GetIntValue();
        AddEXP(expReward);
        //Debug.Log($"Added {expReward} from {victimAsEnemy.gameObject.name}");
    }

    #region EXP Handling
    /// <summary>
    /// Adds the specified amount of experience points (EXP) to the current experience.
    /// If the current experience exceeds the maximum experience, triggers a level up.
    /// </summary>
    /// <param name="amount">The amount of experience points to add.</param>
    /// <param name="willInvokeAction">Whether to invoke the action. When listenting to the action and applying AddEXP again, set this to false to avoid infinite recursion.</param>
    public void AddEXP(int amount, bool willInvokeAction = true)
    {
        if(Level < 1)
        {
            Debug.LogError("Cannot be less than level 1, fixing level.");

            Level = 1;
            CurrentEXP = 0;
            MaxEXP = CalculateMaxEXP();
            return;
        }

        if(amount < 0)
        {
            Debug.LogError("Cannot add negative EXP.");
            return;
        }

        if(willInvokeAction) OnEXPAdded.Invoke(amount);
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
        CurrentEXP -= MaxEXP;
        Level++;
        MaxEXP = CalculateMaxEXP();

        OnLevelUp?.Invoke(Level);

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
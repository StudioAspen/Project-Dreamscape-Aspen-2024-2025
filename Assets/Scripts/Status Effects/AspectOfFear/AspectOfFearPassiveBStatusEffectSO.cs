using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Aspect of Fear Passive B", menuName = "Status Effect/Aspects/Aspect of Fear/Passive B")]
public class AspectOfFearPassiveBStatusEffectSO : StatusEffectSO
{
    public enum Buff
    {
        DAMAGE,
        SPEED,
        //DEFENSE,
    }

    [field: Header("Aspect of Fear Passive B: Settings")]
    [field: SerializeField] public int MaxStacks { get; private set; } = 5;
    [field: SerializeField] public float StackTimerReset { get; private set; } = 5f;
    [field: SerializeField, SerializedDictionary("Buff", "Multiplier")] public SerializedDictionary<Buff, float> StatBuffs { get; private set; } = new();
    private int currentStacks;
    private float timer;

    [field: Header("Aspect of Fear Passive B Expanded: Settings")]
    [field: SerializeField] public float OnKillBuffExtension { get; private set; } = 0; // How many more secs will buff last
    [field: SerializeField] public float OnKillStatStealFraction { get; private set; } = 0;
    [field: SerializeField] public float OnKillSizeGrowth { get; private set; } = 0;

    private Dictionary<Buff, Stat> buffStatPairs = new Dictionary<Buff, Stat>();
    private HashSet<Buff> activeBuffs = new HashSet<Buff>(); // checks for current active buffs, so non-active buffs can be activated when stacks are filled again

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        // predefine buffs with the correct stat here
        buffStatPairs = new()
        {
            { Buff.DAMAGE, entity.DamageModifier },
            { Buff.SPEED, entity.StatusSpeedModifier },
            //{ Buff.DEFENSE, entity.StatusSpeedModifier },
        };

        entity.OnStunEntity += Entity_OnStunEntity;
    }

    public override void Cancel()
    {
        base.Cancel();

        ResetStacks();

        entity.OnStunEntity -= Entity_OnStunEntity;
        entity.OnKillEntity -= Entity_OnKillEntity;
    }

    public override void Update()
    {
        base.Update();

        HandleStacks();
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        AspectOfFearPassiveBStatusEffectSO overridingStatusEffect = newStatusEffect as AspectOfFearPassiveBStatusEffectSO;

        MaxStacks = overridingStatusEffect.MaxStacks;
        StackTimerReset = overridingStatusEffect.StackTimerReset;

        // Only expanded version has this > 0
        if(overridingStatusEffect.OnKillBuffExtension > 0)
        {
            entity.OnKillEntity += Entity_OnKillEntity;
            OnKillBuffExtension = overridingStatusEffect.OnKillBuffExtension;
            OnKillStatStealFraction = overridingStatusEffect.OnKillStatStealFraction;
            OnKillSizeGrowth = overridingStatusEffect.OnKillSizeGrowth;
        }
    }

    private void Entity_OnStunEntity(Entity stunner, Entity victim, float stunDuration)
    {
        AddStack();
    }

    private void Entity_OnKillEntity(Entity victim)
    {
        if (OnKillBuffExtension == 0) return; // If not expanded version
        if (activeBuffs.Count == 0) return;

        // Steal stats
        entity.DamageModifier.AddMultiplier(1f + victim.DamageModifier.GetFloatValue() * OnKillStatStealFraction, this);
        entity.DamageModifier.AddMultiplier(1f + victim.StatusSpeedModifier.GetFloatValue() * OnKillStatStealFraction, this);

        timer -= OnKillBuffExtension; // Extend buff duration
        entity.SizeScale.AddMultiplier(OnKillSizeGrowth, this);
    }

    private void AddStack()
    {
        // reset timer and add stack
        timer = 0f;
        currentStacks++;

        Debug.Log("STACKS = " + currentStacks);
        if(currentStacks >= MaxStacks)
        {
            OnMaxStacksReached();
        }
    }

    private void ResetStacks()
    {
        Debug.Log("Reset buffs");

        timer = 0f;
        currentStacks = 0;

        foreach(Buff buff in new HashSet<Buff>(activeBuffs))
        {
            buffStatPairs[buff].ClearBuffsFromSource(this);
        }
        activeBuffs.Clear();

        entity.SizeScale.ClearBuffsFromSource(this);
    }

    private void HandleStacks()
    {
        if (currentStacks <= 0) return;

        timer += entity.LocalDeltaTime;
        if(timer > StackTimerReset)
        {
            ResetStacks();
            return;
        }
    }

    private void OnMaxStacksReached()
    {
        Debug.Log("max stacks");

        // create a list of non-active buffs that are not currently active
        List<Buff> nonActiveBuffs = new List<Buff>();
        foreach(Buff buff in System.Enum.GetValues(typeof(Buff)))
        {
            if(!activeBuffs.Contains(buff)) nonActiveBuffs.Add(buff);
        }

        // if all buffs are active, reset timer for stack to reset
        if (nonActiveBuffs.Count == 0)
        {
            timer = 0f;
            currentStacks = 0;
            return;
        }

        // select a random buff from the available list
        Buff selectedBuff = nonActiveBuffs[Random.Range(0, nonActiveBuffs.Count)];

        // apply the selected buff and mark it as active
        // then refresh timer, stacks, and duration of buffs
        buffStatPairs[selectedBuff].AddMultiplier(StatBuffs[selectedBuff], this);
        activeBuffs.Add(selectedBuff);
        Debug.Log($"Aspect of Fear Passive B: Applied {selectedBuff} {StatBuffs[selectedBuff]}x");

        timer = 0f;
        currentStacks = 0;
    }
}

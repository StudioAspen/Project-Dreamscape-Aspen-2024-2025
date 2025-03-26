using UnityEngine;

public class TickStatusEffectSO : StatusEffectSO
{
    [field: Header("Tick Status Effect: Settings")]
    [field: SerializeField] public int Ticks { get; protected set; } = 2;
    [field: SerializeField] public float TickDuration { get; protected set; } = 0.5f;
    private protected int currentTicks;
    private float tickTimer;

    /// <summary>
    /// Updates the status effect by using a tick timer to count up the currentTicks.
    /// Expires when currentTicks equals Ticks.
    /// Override this function if you want to customize the update behavior.
    /// </summary>
    public override void Update()
    {
        base.Update();

        tickTimer += Time.deltaTime;
        if(tickTimer > TickDuration)
        {
            tickTimer = 0;
            currentTicks++;

            OnTick();
        }

        if(currentTicks >= Mathf.RoundToInt(Ticks * GetSourceBuffTypeDurationMultiplier()))
        {
            OnExpire();
        }
    }

    /// <summary>
    /// Called when the status effect ticks.
    /// Override this function to add custom behavior for each tick.
    /// </summary>
    private protected virtual void OnTick()
    {

    }

    /// <summary>
    /// Overrides the current status effect with a new status effect by adding more ticks and changing the tick duration.
    /// Override this function if you want to customize the override behavior.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to override with.</param>
    /// <returns>True if the override is successful, false otherwise.</returns>
    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        TickStatusEffectSO overridingStatusEffect = newStatusEffect as TickStatusEffectSO;

        Ticks += overridingStatusEffect.Ticks;
        TickDuration = overridingStatusEffect.TickDuration;
    }

    /// <summary>
    /// Overrides the number of active ticks
    /// </summary>
    /// <param name="newTicks">The new ticks to override with.</param>
    public void OverrideTicks(int newTicks)
    {
        Ticks = newTicks;
    }
}


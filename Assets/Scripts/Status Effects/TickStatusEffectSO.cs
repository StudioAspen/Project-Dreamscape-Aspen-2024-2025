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

        if(currentTicks >= Ticks)
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
    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        Ticks += (newStatusEffect as TickStatusEffectSO).Ticks;
        TickDuration = (newStatusEffect as TickStatusEffectSO).TickDuration;
        return true;
    }
}


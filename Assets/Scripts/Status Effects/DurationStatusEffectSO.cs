using UnityEngine;

public class DurationStatusEffectSO : StatusEffectSO
{
    [field: Header("Duration Status Effect: Settings")]
    [field: SerializeField] public float Duration { get; protected set; } = 1f;
    public float RemainingDuration { get; protected set; }

    /// <summary>
    /// Called when the status effect is applied.
    /// RemainingDuration is set to the Duration here.
    /// Override this function if you want to customize the OnApply behavior.
    /// </summary>
    private protected override void OnApply()
    {
        base.OnApply();

        RemainingDuration = Duration;
    }

    /// <summary>
    /// Updates the status effect by decreasing the remaining duration and checking if it has expired.
    /// Override this function if you want to customize the update behavior.
    /// </summary>
    public override void Update()
    {
        base.Update();

        RemainingDuration -= GetLocalDeltaTime();

        if (RemainingDuration <= 0)
        {
            OnExpire();
        }
    }

    /// <summary>
    /// Overrides the current status effect with a new status effect by extending the current duration.
    /// Override this function if you want to customize the override behavior.
    /// </summary>
    /// <param name="newStatusEffect">The new status effect to override with.</param>
    /// <returns>True if the override is successful, false otherwise.</returns>
    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        base.OnStack(newStatusEffect);

        DurationStatusEffectSO overridingStatusEffect = newStatusEffect as DurationStatusEffectSO;

        RemainingDuration += overridingStatusEffect.Duration;
    }

    /// <summary>
    /// Overrides the current duration
    /// </summary>
    /// <param name="newDuration">The new duration to override with.</param>
    public void OverrideDuration(float newDuration)
    {
        Duration = newDuration;
    }
}

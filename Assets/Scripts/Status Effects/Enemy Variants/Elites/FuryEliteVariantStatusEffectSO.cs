using UnityEngine;

[CreateAssetMenu(fileName = "Fury Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Fury")]
public class FuryEliteVariantStatusEffectSO : EliteVariantStatusEffectSO
{
    [field: Header("Config")]
    [field: SerializeField] public float TimeScaleMultiplier { get; private set; } = 1.5f;

    private protected override void OnApply()
    {
        base.OnApply();

        enemy.LocalTimeScale.AddMultiplier(TimeScaleMultiplier, this);
    }

    public override void Cancel()
    {
        base.Cancel();

        enemy.LocalTimeScale.ClearBuffsFromSource(this);
    }
}
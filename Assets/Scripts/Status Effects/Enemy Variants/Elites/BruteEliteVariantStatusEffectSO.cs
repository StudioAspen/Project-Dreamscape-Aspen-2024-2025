using UnityEngine;

[CreateAssetMenu(fileName = "Brute Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Brute")]
public class BruteEliteVariantStatusEffectSO : EliteVariantStatusEffectSO
{
    [field: Header("Config")]
    [field: SerializeField] public float TimeScaleMultiplier { get; private set; } = 0.5f;

    private protected override void OnApply()
    {
        base.OnApply();

        enemy.SetLocalTimeScaleModifier(enemy.LocalTimeScaleModifier * TimeScaleMultiplier);
    }

    public override void Cancel()
    {
        base.Cancel();

        enemy.SetLocalTimeScaleModifier(enemy.LocalTimeScaleModifier / TimeScaleMultiplier);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Brute Elite Variant", menuName = "Status Effect/Enemy Variants/Elite/Brute")]
public class BruteEliteVariantStatusEffectSO : EliteVariantStatusEffectSO
{
    [field: Header("Config")]
    [field: SerializeField] public float MovementSpeedMultiplier { get; private set; } = 0.67f;

    private protected override void OnApply()
    {
        base.OnApply();

        enemy.SetStatusSpeedModifier(enemy.SpeedModifier * MovementSpeedMultiplier);
    }

    public override void Cancel()
    {
        base.Cancel();

        enemy.SetStatusSpeedModifier(enemy.SpeedModifier / MovementSpeedMultiplier);
    }
}
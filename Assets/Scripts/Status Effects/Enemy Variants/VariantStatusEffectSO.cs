using UnityEngine;

public class VariantStatusEffectSO : StatusEffectSO
{
    private protected Enemy enemy;
    private protected EntityRendererManager entityRendererManager;

    [field: Header("Variant Config")]
    [field: SerializeField] public string Name { get; private set; } = "Variant";
    [field: SerializeField] public float MaxHealthMultiplier { get; private set; } = 1.25f;
    [field: SerializeField] public float EXPValueMultiplier { get; private set; } = 1.25f;
    [field: SerializeField] public float SizeMultiplier { get; private set; } = 1.25f;
    [field: SerializeField] public float DamageMultiplier { get; private set; } = 1.25f;

    private void OnValidate()
    {
        Stackable = false; // force unstackable
    }

    private protected override void OnApply()
    {
        base.OnApply();

        enemy = entity as Enemy;
        if (enemy == null)
        {
            Debug.LogError($"{GetType()} can only be applied to an Enemy entity.");
            RemoveSelf();
            return;
        }

        enemy.MaxHealth.AddMultiplier(MaxHealthMultiplier, this);
        enemy.HealToFull(false); // Heal to full after setting max health without spawning numbers

        enemy.EXPValue.AddMultiplier(EXPValueMultiplier, this);
        enemy.SizeScale.AddMultiplier(SizeMultiplier, this);
        enemy.DamageModifier.AddMultiplier(DamageMultiplier, this);
    }

    public override void Cancel()
    {
        base.Cancel();

        enemy.MaxHealth.ClearBuffsFromSource(this);
        enemy.EXPValue.ClearBuffsFromSource(this);
        enemy.SizeScale.ClearBuffsFromSource(this);
        enemy.DamageModifier.ClearBuffsFromSource(this);
    }
}

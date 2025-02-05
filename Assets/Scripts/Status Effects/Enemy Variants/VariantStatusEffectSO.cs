using UnityEngine;

public class VariantStatusEffectSO : StatusEffectSO
{
    private protected Enemy enemy;
    private protected EntityRendererManager entityRendererManager;

    [field: Header("Variant Config")]
    [field: SerializeField] public string Name { get; private set; } = "Variant";
    [field: SerializeField] public Material VariantMaterial { get; private set; }
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
            entityStatusEffectorOwner.RemoveStatusEffect(GetType(), true);
            return;
        }

        entityRendererManager = enemy.GetComponent<EntityRendererManager>();
        if (entityRendererManager != null)
        {
            entityRendererManager.RemoveAllMaterials();
            entityRendererManager.AddMaterial(VariantMaterial);
        }

        enemy.SetMaxHealthModifier(enemy.MaxHealthModifier * MaxHealthMultiplier);
        enemy.SetEXPValueMultiplier(enemy.EXPValueMultiplier * EXPValueMultiplier);
        enemy.SetSizeScaleModifier(enemy.SizeScaleModifier * SizeMultiplier);
        enemy.SetDamageModifier(enemy.DamageModifier * DamageMultiplier);
    }

    public override void Cancel()
    {
        base.Cancel();

        if (entityRendererManager != null) entityRendererManager.RestoreOriginalMaterials();

        enemy.SetMaxHealthModifier(enemy.MaxHealthModifier / MaxHealthMultiplier, false);
        enemy.SetEXPValueMultiplier(enemy.EXPValueMultiplier / EXPValueMultiplier);
        enemy.SetSizeScaleModifier(enemy.SizeScaleModifier / SizeMultiplier);
        enemy.SetDamageModifier(enemy.DamageModifier / DamageMultiplier);
    }
}

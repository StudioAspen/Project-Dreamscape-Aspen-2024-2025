using UnityEngine;

public class EliteVariantStatusEffectSO : VariantStatusEffectSO
{
    [field: Header("Elite Variant Config")]
    [field: SerializeField] public Material EliteMaterial { get; private set; }

    private protected override void OnApply()
    {
        base.OnApply();

        entityRendererManager = enemy.GetComponent<EntityRendererManager>();
        if (entityRendererManager != null)
        {
            entityRendererManager.RemoveAllMaterials();
            entityRendererManager.AddMaterial(EliteMaterial);
        }
    }

    public override void Cancel()
    {
        base.Cancel();

        if (entityRendererManager != null) entityRendererManager.RestoreOriginalMaterials();
    }
}

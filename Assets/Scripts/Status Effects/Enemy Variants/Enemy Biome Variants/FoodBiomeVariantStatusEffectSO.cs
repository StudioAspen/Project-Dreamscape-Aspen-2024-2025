using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Biome Enemy Status Variants/Food Biome")]
public class FoodBiomeVariantStatusEffectSO : BiomeVariantStatusEffectSO {
    [field: Header("Biome Enemy Status Effect Config")]
    [field: SerializeField] public float MovementSpeedMultiplier { get; private set; } = 1.337f;
    [field: SerializeField][field: Range(0f,1f)] public float FoodDropChance { get; private set; } = .15f;
    
    
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
     
        Debug.LogWarning("Effect applied to " + enemy.name);
        
        enemy.StatusSpeedModifier.AddMultiplier(MovementSpeedMultiplier, this);
    }

    public override void Cancel()
    {
        base.Cancel();
        
        enemy.StatusSpeedModifier.ClearBuffsFromSource(this);
    }
    
}
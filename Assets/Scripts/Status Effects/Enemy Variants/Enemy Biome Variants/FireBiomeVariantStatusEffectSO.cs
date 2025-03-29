using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Biome Enemy Status Variants/Fire Biome")]
public class FireBiomeVariantStatusEffectSO : BiomeVariantStatusEffectSO
{
    
    [field: Header("Biome Enemy Status Effect Config")]
    [field: SerializeField] public float MovementSpeedMultiplier { get; private set; } = .67f;
    [field: SerializeField] private BurnStatusEffectSO burnStatusEffect;
    
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
        
        enemy.StatusSpeedModifier.AddMultiplier(MovementSpeedMultiplier, this);
        enemy.OnEntityDealDamage += BurnVictim;


    }

    public override void Cancel()
    {
        base.Cancel();
        
        enemy.StatusSpeedModifier.ClearBuffsFromSource(this);
        enemy.OnEntityDealDamage -= BurnVictim;
        
    }

    private void BurnVictim(Entity attacker, Entity victim, Vector3 hitPoint, int damage) {
        if (victim == null) return;
        if (attacker == null) return;
        EntityStatusEffector.TryApplyStatusEffect(victim.gameObject, burnStatusEffect, enemy.gameObject);
    }
    
    
    
}
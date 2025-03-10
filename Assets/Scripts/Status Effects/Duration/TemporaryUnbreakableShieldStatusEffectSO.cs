using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unbreakable Shield Status Effect", menuName = "Status Effect/Unbreakable shield")]

public class TemporaryUnbreakableShieldStatusEffectSO : DurationStatusEffectSO
{
    [field: Header("Config")]

    [field: SerializeField] public ShieldVFX ShieldVFXPrefab { get; private set; }

    private ShieldVFX shieldVFXInstance;
    private protected override void OnApply()
    {
        base.OnApply();
        entity.MaxHealth.AddMultiplier(0, this);
        shieldVFXInstance = Instantiate(ShieldVFXPrefab, entity.GetColliderCenterPosition(), Quaternion.identity);
        shieldVFXInstance.Init(entity.GetColliderLargestSize() / 2, entity.transform, entity.GetColliderCenterPosition() - entity.transform.position);
        shieldVFXInstance.PlayStartAnimation();

        entity.OnEntityTakeDamage += Entity_OnEntityTakeDamage;

    }

    private protected override void OnExpire()
    {
        base.OnExpire();

        entity.MaxHealth.ClearBuffsFromSource(this);

        if (shieldVFXInstance != null) shieldVFXInstance.PlayEndAnimation(() => Destroy(shieldVFXInstance.gameObject));

        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;

    }

    public override void Cancel()
    {

        base.Cancel();

        entity.MaxHealth.ClearBuffsFromSource(this);

        if (shieldVFXInstance != null) shieldVFXInstance.PlayEndAnimation(() => Destroy(shieldVFXInstance.gameObject));

        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;


    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        entity.Heal(damage); 
    }


}

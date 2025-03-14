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
        
        entity.SetInvincible(true);

        shieldVFXInstance = Instantiate(ShieldVFXPrefab, entity.GetColliderCenterPosition(), Quaternion.identity);
        shieldVFXInstance.Init(entity.GetColliderLargestSize() / 2, entity.transform, entity.GetColliderCenterPosition() - entity.transform.position);
        shieldVFXInstance.PlayStartAnimation();
    }

    private protected override void OnExpire()
    {
        base.OnExpire();

        entity.SetInvincible(false);

        if (shieldVFXInstance != null) shieldVFXInstance.PlayEndAnimation(() => Destroy(shieldVFXInstance.gameObject));
    }

    public override void Cancel()
    {
        base.Cancel();

        entity.SetInvincible(false);

        if (shieldVFXInstance != null) shieldVFXInstance.PlayEndAnimation(() => Destroy(shieldVFXInstance.gameObject));
    }
}

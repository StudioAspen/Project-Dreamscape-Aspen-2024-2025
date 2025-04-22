using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Hammer Flaming Aura")]
public class HammerFlamingAuraStatusEffectSO : StatusEffectSO
{
    private CapsuleCollider hammerHeadCollider;

    [field: Header("Hammer Flaming Aura: Settings")]
    [field: SerializeField] public float HammerHeadHitboxMultiplier { get; private set; } = 1.1f;

    private protected override void OnApply()
    {
        base.OnApply();

        hammerHeadCollider = entity.GetComponentInChildren<Weapon>().GetComponent<CapsuleCollider>();
        if (hammerHeadCollider == null)
        {
            Debug.LogError($"{name}: Hammer head capsule collider not found on player: {entity.name}");
            RemoveSelf(); // If theres no Weapon, remove this passive
            return;
        }

        hammerHeadCollider.radius *= HammerHeadHitboxMultiplier;
        hammerHeadCollider.height *= HammerHeadHitboxMultiplier;
    }

    public override void Cancel()
    {
        base.Cancel();

        hammerHeadCollider.radius /= HammerHeadHitboxMultiplier;
        hammerHeadCollider.height /= HammerHeadHitboxMultiplier;
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        base.OnStack(newStatusEffect);

        HammerFlamingAuraStatusEffectSO overridingStatusEffect = newStatusEffect as HammerFlamingAuraStatusEffectSO;

        float newToOldRatio = overridingStatusEffect.HammerHeadHitboxMultiplier / HammerHeadHitboxMultiplier;

        hammerHeadCollider.radius *= newToOldRatio;
        hammerHeadCollider.height *= newToOldRatio;
    }
}

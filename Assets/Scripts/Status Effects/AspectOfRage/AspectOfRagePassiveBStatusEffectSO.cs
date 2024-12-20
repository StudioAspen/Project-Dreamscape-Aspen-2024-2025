using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive B")]
public class AspectOfRagePassiveBStatusEffectSO : StatusEffectSO
{
    [field: Header("Aspect of Rage Passive B: Settings")]
    [field: SerializeField] public ChargeAttackActivatedStatusEffectSO ChargedAttackActivatedStatusEffect { get; private set; }

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        entityStatusEffectorOwner.ApplyStatusEffect(ChargedAttackActivatedStatusEffect, entity.gameObject);
    }

    public override void Cancel()
    {
        base.Cancel();

        entityStatusEffectorOwner.RemoveStatusEffect(ChargedAttackActivatedStatusEffect.GetType(), true);
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        return true;
    }
}
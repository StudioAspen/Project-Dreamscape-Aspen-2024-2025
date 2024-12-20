using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Charge Attack Activated")]
public class ChargeAttackActivatedStatusEffectSO : StatusEffectSO
{
    private protected override void OnApply()
    {
        base.OnApply();
    }

    public override void Cancel()
    {
        base.Cancel();
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        // add expansion logic here when stacked

        return true;
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Ambition/Passive A")]
public class AspectofAmbitionPassiveAStatusEffectSO: StatusEffectSO
{
    [field: Header("Aspect of Ambition Passive A: Settings")]

    private void OnValidate()
    {
    }

    private protected override void OnApply()
    {
        base.OnApply();
    }

    public override void Cancel()
    {
        base.Cancel();
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
    }
}
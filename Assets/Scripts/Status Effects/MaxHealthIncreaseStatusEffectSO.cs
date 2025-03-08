using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Max Health Increase", menuName = "Status Effect/Max Health Increase")]
public class MaxHealthIncreaseStatusEffectSO : StatusEffectSO
{
    [field: Header("Max Health Increase Effect: Settings")]
    [SerializeField] private int healthIncrease;
    //its just a flat increase right now i hope thats like.. what they wanted LOL i mean thats all it said on the design doc
    private protected override void OnApply()
    {
        base.OnApply();
        entity.MaxHealth.AddFlatAmount(healthIncrease, this);
        entity.Heal(healthIncrease);
    }

    public override void Cancel()
    {
        base.Cancel();
        entity.MaxHealth.ClearFlatIncreaseFromSource(this);
    }
}

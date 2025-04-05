using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Ambition Basic Stat Upgrade", menuName = "Status Effect/Aspects/Aspect of Ambition/Basic Stat Upgrade")]
public class AmbitionBasicStatUpgradeStatusEffectSO : StatusEffectSO
{
    [field: Header("Attack Speed Increase Effect: Settings")]
    [SerializeField] private float attackSpeedIncreaseMultiplier = 1.25f;
    [SerializeField] private float movementSpeedIncreaseMultiplier = 1.25f;
    
    private protected override void OnApply()
    {
        base.OnApply();
        entity.LocalTimeScale.AddMultiplier(attackSpeedIncreaseMultiplier, this);
        entity.StatusSpeedModifier.AddMultiplier(movementSpeedIncreaseMultiplier, this);
    }

    public override void Cancel()
    {
        base.Cancel();
        entity.LocalTimeScale.RemoveMultiplier(attackSpeedIncreaseMultiplier, this);
        entity.StatusSpeedModifier.RemoveMultiplier(movementSpeedIncreaseMultiplier, this);
    }
}

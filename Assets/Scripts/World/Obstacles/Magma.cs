using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magma : MonoBehaviour
{
    [SerializeField] private BurnStatusEffectSO burnStatusEffect;

    private void OnTriggerEnter(Collider other)
    {
        EntityStatusEffector.TryApplyStatusEffect(other.gameObject, burnStatusEffect, gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        BurnStatusEffectSO currentBurnStatus = EntityStatusEffector.TryGetStatusEffect<BurnStatusEffectSO>(other.gameObject);
        if (currentBurnStatus == null)
        {
            EntityStatusEffector.TryApplyStatusEffect(other.gameObject, burnStatusEffect, gameObject);
        }
        else
        {
            currentBurnStatus.OverrideTicks(burnStatusEffect.Ticks);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BurnStatusEffectSO currentBurnStatus = EntityStatusEffector.TryGetStatusEffect<BurnStatusEffectSO>(other.gameObject);
        if (currentBurnStatus == null)
        {
            EntityStatusEffector.TryApplyStatusEffect(other.gameObject, burnStatusEffect, gameObject);
        }
        else
        {
            currentBurnStatus.OverrideTicks(burnStatusEffect.Ticks);
        }
    }
}

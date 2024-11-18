using UnityEngine;

public class BuffAspectNodeNode : AspectNodeNode
{
    [field: Header("Status Effect")]
    [field: SerializeField] public StatusEffectSO StatusEffect { get; private set; }

    public override void ApplyAspect(AspectsManager aspectsManager)
    {
        base.ApplyAspect(aspectsManager);

        EntityStatusEffector ownerStatusEffector = aspectsManager.GetComponentInChildren<EntityStatusEffector>();
        if (ownerStatusEffector == null)
        {
            Debug.LogError($"No EntityStatusEffector found in children of {aspectsManager.gameObject.name}");
        }

        if(StatusEffect.Stackable)
        {
            ownerStatusEffector.OverrideAndExtendStatusEffect(StatusEffect, ownerStatusEffector.gameObject);
        }
        else
        {
            ownerStatusEffector.ApplyStatusEffect(StatusEffect, ownerStatusEffector.gameObject);
        }    
    }
}

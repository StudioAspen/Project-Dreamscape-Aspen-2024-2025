using UnityEngine;

public class StatusEffectAspectNodeNode : AspectNodeNode
{
    [field: Header("Status Effect")]
    [field: SerializeField] public StatusEffectSO StatusEffect { get; private set; }

    public override void ApplyAspect(AspectsManager aspectsManager)
    {
        base.ApplyAspect(aspectsManager);
        
        if(StatusEffect == null)
        {
            Debug.LogError($"No StatusEffect found in {name}");
            return;
        }

        EntityStatusEffector ownerStatusEffector = aspectsManager.GetComponentInChildren<EntityStatusEffector>();
        if (ownerStatusEffector == null)
        {
            Debug.LogError($"No EntityStatusEffector found in children of {aspectsManager.gameObject.name}");
            return;
        }

        ownerStatusEffector.ApplyStatusEffect(StatusEffect, ownerStatusEffector.gameObject); 
    }
}

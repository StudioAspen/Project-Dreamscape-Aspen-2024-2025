using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]

public class AmbitionDashState: PlayerDashState
{
    private protected Collider[] colliderHits;
    private protected TemporaryWindWackerStatusEffectSO WindWackerStatusEffect;
    private protected HashSet<Entity> hitEntities;
    public float speedBuff { get; protected set; }

    public override void OnEnter()
    {
        base.OnEnter();

        player.IgnoreOtherEntityCollisions(true);
    }

    public override void OnExit()
    {
        base.OnExit();
        
        player.IgnoreOtherEntityCollisions(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        colliderHits = player.GetCharacterControllerOverlaps();
        foreach (var hit in colliderHits)
        {
            if (hit.TryGetComponent(out Entity entity))
            {
                if (!hitEntities.Contains(entity))
                {
                    if (entity == player) continue;

                    if (entity.Team == player.Team) continue;

                    EntityStatusEffector.TryApplyStatusEffect(entity.gameObject, WindWackerStatusEffect, player.gameObject);

                    hitEntities.Add(entity);
                }
            }
        }
    }

    // notes for later:
    // add a check so status effect is applied 
    // once to enemy entity *DONE*
    //
    // use hashmap or hashset to do so *DONE*
    //
    // implement speed buff after dashing through enemy
}
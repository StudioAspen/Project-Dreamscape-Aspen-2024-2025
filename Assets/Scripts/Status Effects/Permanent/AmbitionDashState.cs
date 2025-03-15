using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]

public class AmbitionDashState: PlayerDashState
{
    public override void OnEnter()
    {
        base.OnEnter();
        player.ApplyStatusEffect<AspectofAmbitionPassiveAStatusEffectSO>();
        player.IgnoreOtherEntityCollisions(true);
    }

    public override void OnExit()
    {
        base.OnExit();
        player.RemoveStatusEffect<AspectofAmbitionPassiveAStatusEffectSO>();
        player.IgnoreOtherEntityCollisions(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        colliderHits = Player.GetCharacterControllerOverlaps();
        foreach (var hit in colliderHits)
        {
            if (hit.TryGetComponent(out Entity entity))
            {
                if (entity == player) continue;

                if(entity.Team == player.Team) continue;

                entity.ApplyStatusEffect<TemporaryWindWackerStatusEffectSO>();
            }
        }
    }

    // notes for later:
    // add a check so status effect is applied 
    // once to enemy entity
    //
    // use hashmap or hashset to do so
    //
    // implement speed buff after dashing through enemy
}
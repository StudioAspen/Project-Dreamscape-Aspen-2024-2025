using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Follower Memory Throw Ability", menuName = "Memory Abilities/Follower/Throw Hammer")]
public class PlayerFollowerThrowAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip HammerAnimationClip { get; private set; }
    [field: SerializeField] public FollowerHammerAbility PrefabHammerAttack { get; private set; }

    // check if player is not in a specific state for errors
    public override bool CanUseAbility(Player player)
    {
        return base.CanUseAbility(player);
    }

    // cancels ability and changing to a state
    public override bool CanCancelAbility(Player player, EntityBaseState desiredState)
    {
        return desiredState == player.PlayerAbilityState || desiredState == player.DefaultState;
    }

    public override void OnEnter()
    {
        // plays an animation of throwing hammer 
        //player.PlayOneShotAnimation(HammerAnimationClip);

        // start the hammer attack
        FollowerHammerAbility hammer = Instantiate(PrefabHammerAttack, player.transform);
        hammer.Init(player);
        player.ChangeState(player.DefaultState);
    }

    public override void OnExit()
    {
    
    }

    public override void OnUpdate()
    {
    }

    public override void OnOnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }
}

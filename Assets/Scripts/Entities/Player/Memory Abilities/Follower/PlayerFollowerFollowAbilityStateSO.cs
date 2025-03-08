using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "Follower Memory Charge Ability", menuName = "Memory Abilities/Follower/Follow")]
public class PlayerFollowerFollowAbilityStateSO : PlayerAbilityStateSO
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
        player.PlayOneShotAnimation(HammerAnimationClip);

        // start the hammer attack
        FollowerHammerAbility hammer = Instantiate(PrefabHammerAttack);
        hammer.Init(player);
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

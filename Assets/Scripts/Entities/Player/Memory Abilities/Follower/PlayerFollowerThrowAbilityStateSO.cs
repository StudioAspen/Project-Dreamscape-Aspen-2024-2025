using Dreamscape.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Follower Memory Throw Ability", menuName = "Memory Abilities/Follower/Throw Hammer")]
public class PlayerFollowerThrowAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip OneHandThrowAnimationClip { get; private set; }
    [field: SerializeField] public float Duration { get; private set; } = 1.5f;
    [field: SerializeField] public FollowerHammerAbility PrefabHammerAttack { get; private set; }

    private float timer;

    public override bool CanUseAbility(Player player)
    {
        bool cannotUseAbility =
            !player.IsGrounded ||
            player.CurrentState == player.PlayerAttackState ||
            player.CurrentState == player.PlayerChargeState ||
            player.CurrentState == player.PlayerDashState;

        return !cannotUseAbility;
    }

    public override bool CanCancelAbility(Player player, EntityBaseState desiredState)
    {
        return desiredState == player.PlayerAbilityState || desiredState == player.DefaultState;
    }

    public override void OnEnter()
    {
        player.UseRootMotion = true;

        // plays an animation of throwing hammer 
        player.PlayOneShotAnimation(OneHandThrowAnimationClip, Duration);

        timer = 0;

        playerCombat.OnFireAbility += PlayerCombat_OnFireAbility;
    }

    public override void OnExit()
    {
        player.UseRootMotion = false;
        playerCombat.OnFireAbility -= PlayerCombat_OnFireAbility;
    }

    public override void OnUpdate()
    {
        timer += player.LocalDeltaTime;
        if(timer > Duration)
        {
            player.ChangeState(player.DefaultState, true);
            return;
        }
    }

    public override void OnOnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    private void PlayerCombat_OnFireAbility(AnimationEvent eventData)
    {
        // start the hammer attack
        FollowerHammerAbility hammer = ObjectPoolerManager.Instance.SpawnPooledObject<FollowerHammerAbility>(PrefabHammerAttack.gameObject, player.GetColliderCenterPosition());
        hammer.Init(player);
    }
}

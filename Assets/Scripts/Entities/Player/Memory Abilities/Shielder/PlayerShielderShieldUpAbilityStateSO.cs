using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shielder Memory Shield Up Ability", menuName = "Memory Abilities/Shielder/Shield Up")]
public class PlayerShielderShieldUpAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public TemporaryUnbreakableShieldStatusEffectSO UnbreakableShieldStatusEffect { get; private set; }
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float AnimationDuration { get; private set; } = 1f;

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
        timer = 0;
        player.PlayOneShotAnimation(AnimationClip, AnimationDuration);
    }

    public override void OnExit()
    {
        EntityStatusEffector.TryApplyStatusEffect(player.gameObject, UnbreakableShieldStatusEffect, player.gameObject);
    }

    public override void OnUpdate()
    {
        timer += player.LocalDeltaTime;
        if (timer > AnimationDuration)
        {
            player.ChangeState(player.DefaultState, true);
            return;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaper Memory Ground Pound Ability", menuName = "Memory Abilities/Leaper/Ground Pound")]
public class PlayerLeaperGroundPoundAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip GroundPoundAnimationClip { get; private set; }
    [field: SerializeField] public AnimationClip GroundImpactAnimationClip { get; private set; }
    [field: SerializeField] public float GroundPoundForce { get; private set; } = 15f;
    [field: SerializeField] public float AOEDamageMultiplier { get; private set; } = 1.5f;
    [field: SerializeField] public float AOERadius { get; private set; } = 4f;
    [field: SerializeField] public float AOELaunchForce { get; private set; } = 7.5f;
    [field: SerializeField] public float AOEStunDuration { get; private set; } = 3f;
    [field: SerializeField] public float RecoverDuration { get; private set; } = 1f;

    private bool hasRecoveredStarted;
    private float recoverTimer;

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
        player.PlayOneShotAnimation(GroundPoundAnimationClip);

        player.Launch(Vector3.down, GroundPoundForce);

        player.SetSpeedModifier(0f);

        player.IgnoreOtherEntityCollisions();

        hasRecoveredStarted = false;
        recoverTimer = 0f;
    }

    public override void OnExit()
    {
        player.IgnoreOtherEntityCollisions(false);
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        if(recoverTimer >= RecoverDuration)
        {
            player.ChangeState(player.DefaultState);
            return;
        }

        if (hasRecoveredStarted) recoverTimer += player.LocalDeltaTime;

        if(player.IsGrounded && !hasRecoveredStarted)
        {
            hasRecoveredStarted = true;

            player.PlayOneShotAnimation(GroundImpactAnimationClip);

            Entity.DamageEnemyEntitiesWithAOELaunch(player, player.transform.position, AOERadius, AOEDamageMultiplier, AOELaunchForce, AOEStunDuration);

            CustomDebug.InstantiateTemporarySphere(player.transform.position, AOERadius, 0.25f, new Color(1f, 0, 0, 0.2f));

            CameraShakeManager.Instance.ShakeCamera(15f,1f, 1f);
        }
    }
}

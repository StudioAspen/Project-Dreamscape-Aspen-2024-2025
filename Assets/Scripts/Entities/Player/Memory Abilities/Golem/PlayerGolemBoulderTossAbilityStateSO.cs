using System;
using System.Collections;
using System.Runtime.InteropServices;
using Dreamscape.Abilities;
using UnityEngine;

[CreateAssetMenu(fileName = "Golem Memory Boulder Toss Ability", menuName = "Memory Abilities/Golem/BoulderToss")]
public class PlayerGolemBoulderTossAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip OneHandThrowAnimationClip { get; private set; }
    [field: SerializeField] public AnimationClip TwoHandThrowAnimationClip { get; private set; }
    [field: SerializeField] public float TossDuration { get; private set; } = 1.5f;
    
    [field: SerializeField] public GameObject BoulderPrefab { get; private set; }

    [field: SerializeField] public float BounceHeight { get; private set; } = 5f;
    [field: SerializeField] public float SpawnForwardOffset { get; private set; } = 0f;
    [field: SerializeField] public float GroundOffset { get; private set; } = .75f;

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
        player.PlayOneShotAnimation(UnityEngine.Random.Range(0, 2) == 0 ? OneHandThrowAnimationClip : TwoHandThrowAnimationClip, TossDuration);
        player.UseRootMotion = true;

        timer = 0f;

        playerCombat.OnFireAbility += PlayerCombat_OnFireAbility;
    }

    public override void OnExit()
    {
        player.UseRootMotion = false;

        playerCombat.OnFireAbility -= PlayerCombat_OnFireAbility;
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        timer += player.LocalDeltaTime;
        if (timer > TossDuration)
        {
            player.ChangeState(player.DefaultState, true);
            return;
        }

        player.ApplyRotationToNextMovement();
    }

    private void PlayerCombat_OnFireAbility(AnimationEvent eventData)
    {
        Boulder spawnedAbility = ObjectPoolerManager.Instance.SpawnPooledObject<Boulder>(BoulderPrefab, player.GetColliderCenterPosition() + (player.transform.forward * SpawnForwardOffset) + (Vector3.up * (GroundOffset + BounceHeight)));
        spawnedAbility.SetBounceHeight(BounceHeight);
        spawnedAbility.Init(player);
    }
}

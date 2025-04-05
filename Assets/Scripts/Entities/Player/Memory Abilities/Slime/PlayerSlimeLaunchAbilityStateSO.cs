using System.Collections;
using System.Collections.Generic;
using Dreamscape.Abilities;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime Memory Launch Ability", menuName = "Memory Abilities/Slime/SlimeLaunch")]
public class PlayerSlimeLaunchAbilityStateSO : PlayerAbilityStateSO
{

    [field: Header("Config")]
    [field: SerializeField] public AnimationClip LaunchAnimationClip { get; private set; }
    [field: SerializeField] public float AnimationDuration { get; private set; } = 1f;


    [field: SerializeField] public GameObject SlimePrefab { get; private set; }
    [field: SerializeField] public float SlimeVelocity { get; private set; } = 5f;

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
        //Play Launch Animation Clip:
        player.PlayOneShotAnimation(LaunchAnimationClip, AnimationDuration);

        playerCombat.OnFireAbility += PlayerCombat_OnFireAbility;
    }


    public override void OnExit()
    {
        //Conditions at the End:
        playerCombat.OnFireAbility -= PlayerCombat_OnFireAbility;

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

    //Function Called When the Ability is Fired:
    private void PlayerCombat_OnFireAbility(AnimationEvent eventData)
    {
        SlimeLaunch spawnedAbility = ObjectPoolerManager.Instance.SpawnPooledObject<SlimeLaunch>(SlimePrefab);
        spawnedAbility.SetVelocity(SlimeVelocity);
        spawnedAbility.SetSlimePrefab(SlimePrefab);

        spawnedAbility.Init(player);
    }
}

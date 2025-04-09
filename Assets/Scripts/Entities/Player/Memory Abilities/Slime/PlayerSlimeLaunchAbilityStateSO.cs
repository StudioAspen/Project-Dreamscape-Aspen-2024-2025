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
    [field: SerializeField] public GameObject SlimeTrail { get; private set; }
    [field: SerializeField] public float SlimeVelocity { get; private set; } = 5f;
    [field: SerializeField] public float ArcHeight { get; private set; } = 5f;
    [field: SerializeField] public LayerMask SlimeTrailLayer { get; private set; } //layerMask used to detect and allow the Slime Trail to Instantiate on top of.



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
        player.UseRootMotion = true;


        playerCombat.OnFireAbility += PlayerCombat_OnFireAbility;
    }


    public override void OnExit()
    {
        //Conditions at the End:
        player.UseRootMotion = false;

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
        spawnedAbility.SetSlimeTrail(SlimeTrail);
        spawnedAbility.SetArc(ArcHeight);
        spawnedAbility.SetIgnoredLayers(SlimeTrailLayer);

        spawnedAbility.Init(player);
    }
}

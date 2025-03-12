using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Golem Memory Target Detected Ability", menuName = "Memory Abilities/Golem/Target Detected")]
public class PlayerGolemTargetDetectedAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public PlayerGolemBoulderTossAbilityStateSO BoulderTossState { get; private set; }
    [field: SerializeField] public float TargetDetectedDuration { get; private set; } = 2f;

    private float timer;

    public override bool CanUseAbility(Player player)
    {
        return BoulderTossState.CanUseAbility(player);
    }

    public override bool CanCancelAbility(Player player, EntityBaseState desiredState)
    {
        return desiredState == player.PlayerAbilityState || desiredState == player.DefaultState;
    }

    public override void OnEnter()
    {
        player.PlayOneShotAnimation(AnimationClip, TargetDetectedDuration);

        // player.SetSpeedModifier(0f);

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        timer += player.LocalDeltaTime;

        if (timer > TargetDetectedDuration)
        {
            player.PlayerAbilityState.TryChangeAbilityState(BoulderTossState, true);
            return;
        }

        //player.ApplyRotationToNextMovement();
        //player.RotateToTargetRotation();
    }
}

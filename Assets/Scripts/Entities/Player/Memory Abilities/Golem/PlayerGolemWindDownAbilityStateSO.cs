using DG.Tweening;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Golem Memory Wind Down Ability", menuName = "Memory Abilities/Golem/Wind Down")]
public class PlayerGolemWindDownAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public PlayerGolemBoulderTossAbilityStateSO BoulderTossState { get; private set; }
    [field: SerializeField] public float WindDownDuration { get; private set; } = 2f;

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
        player.PlayDefaultAnimation();

        timer = 0f;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        timer += player.LocalDeltaTime;

        if (timer > WindDownDuration)
        {
            player.ChangeState(player.DefaultState);
            return;
        }

        player.ApplyRotationToNextMovement();
        player.LookAt(player.transform.position + player.TargetForwardDirection, BoulderTossState.rotationSpeed);

    }
}
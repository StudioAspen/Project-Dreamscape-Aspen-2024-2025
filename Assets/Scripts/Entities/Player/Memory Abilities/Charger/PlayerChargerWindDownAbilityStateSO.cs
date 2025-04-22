using DG.Tweening;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Charger Memory Wind Down Ability", menuName = "Memory Abilities/Charger/Wind Down")]
public class PlayerChargerWindDownAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    [field: SerializeField] public PlayerChargerChargeAbilityStateSO ChargeState { get; private set; }
    [field: SerializeField] public float WindDownDuration { get; private set; } = 2f;

    private float timer;

    public override bool CanUseAbility(Player player)
    {
        return ChargeState.CanUseAbility(player);
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
        player.SetSpeedModifier(0f);
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

        float easedSpeedModifier = DOVirtual.EasedValue(ChargeState.ChargeSpeedModifier, 0, timer/WindDownDuration, Ease.OutQuad);
        player.SetSpeedModifier(easedSpeedModifier);

        player.ApplyRotationToNextMovement();
        player.LookAt(player.transform.position + player.TargetForwardDirection, ChargeState.ChargeRotationSpeed);

        player.UpdateHorizontalVelocity(player.transform.forward);
        player.ApplyHorizontalVelocity();
    }
}
using UnityEngine;

public class PlayerChargeState : PlayerBaseState
{
    private PlayerInputReader playerInputReader;
    private PlayerCombat playerCombat;

    private int attackInputNumber;

    private float timer;
    private float duration;

    public PlayerChargeState(Player player) : base(player)
    {
        this.player = player;
        playerCombat = player.GetComponent<PlayerCombat>();
        playerInputReader = player.GetComponent<PlayerInputReader>();
    }

    /// <summary>
    /// Sets the charge attack input number.
    /// 1 is Attack1, 2 is Attack2
    /// </summary>
    /// <param name="attackInputNumber">The attack input number to set.</param>
    public void SetChargeAttackInput(int attackInputNumber)
    {
        this.attackInputNumber = attackInputNumber;
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("Charge");

        player.SetSpeedModifier(0);

        timer = 0f;

        playerCombat.OnChargeStart?.Invoke(attackInputNumber);

        ChargeAttackActivatedStatusEffectSO chargeAttackActivatedStatusEffect =
            EntityStatusEffector.TryGetStatusEffect<ChargeAttackActivatedStatusEffectSO>(player.gameObject);
        duration = chargeAttackActivatedStatusEffect == null ? Mathf.Infinity : chargeAttackActivatedStatusEffect.MaxChargeDuration;
    }

    public override void OnExit()
    {
        playerCombat.OnChargeRelease?.Invoke(attackInputNumber, timer);
    }

    public override void Update()
    {
        timer += player.LocalDeltaTime;

        player.ApplyGravity();

        player.TransitionToAnimation("Charge");

        if(timer >= duration)
        {
            player.ChangeState(player.DefaultState);
            playerInputReader.OnComboAction?.Invoke(attackInputNumber == 1 ? ComboAction.CHARGED_ATTACK1 : ComboAction.CHARGED_ATTACK2);
            return;
        }

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToHorizontalSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.AccelerateToHorizontalSpeed(0f);
        }

        player.RotateToTargetRotation();
        player.InstantlySetHorizontalSpeed(player.GetHorizontalVelocity().magnitude);
        player.ApplyHorizontalVelocity();
    }

    public override void FixedUpdate()
    {

    }
}

using UnityEngine;

public class PlayerChargeStateSO : PlayerBaseStateSO
{
    private PlayerCombat playerCombat;

    private int attackInputNumber;

    public float Timer { get; private set; }
    private float duration;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);
        playerCombat = player.GetComponent<PlayerCombat>();
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

        Timer = 0f;

        playerCombat.OnChargeStart?.Invoke(attackInputNumber);

        ChargeAttackActivatedStatusEffectSO chargeAttackActivatedStatusEffect =
            EntityStatusEffector.TryGetStatusEffect<ChargeAttackActivatedStatusEffectSO>(player.gameObject);
        duration = chargeAttackActivatedStatusEffect == null ? Mathf.Infinity : chargeAttackActivatedStatusEffect.MaxChargeDuration;
    }

    public override void OnExit()
    {
        playerCombat.OnChargeRelease?.Invoke(attackInputNumber, Timer);

        Timer = 0f;
    }

    public override void Update()
    {
        Timer += player.LocalDeltaTime;

        player.ApplyGravity();

        player.TransitionToAnimation("Charge");

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

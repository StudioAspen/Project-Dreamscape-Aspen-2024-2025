using UnityEngine;

[System.Serializable]
public class PlayerChargeState : PlayerBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }

    private PlayerCombat playerCombat;

    public ChargeAttackActivatedStatusEffectSO ChargeActivatedStatusEffect { get; private set; }

    private int attackInputNumber;

    public float Timer { get; private set; }
    private float duration;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        playerCombat = player.GetComponent<PlayerCombat>();
    }

    /// <summary>
    /// Sets the status effect that was responsible for enabling Player Charge.
    /// </summary>
    /// <param name="chargeStatusEffect">The status effect that was responsible for enabling Player Charge</param>
    public void SetChargeStatusEffect(ChargeAttackActivatedStatusEffectSO chargeStatusEffect)
    {
        ChargeActivatedStatusEffect = chargeStatusEffect;
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
        player.PlayOneShotAnimation(AnimationClip);

        player.SetSpeedModifier(0);

        Timer = 0f;

        playerCombat.OnChargeStart?.Invoke(attackInputNumber);

        duration = ChargeActivatedStatusEffect == null ? Mathf.Infinity : ChargeActivatedStatusEffect.MaxChargeDuration;
    }

    public override void OnExit()
    {
        playerCombat.OnChargeRelease?.Invoke(attackInputNumber, Timer);

        Timer = 0f;
    }

    public override void OnUpdate()
    {
        Timer += player.LocalDeltaTime;

        player.ApplyGravity();

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


    /// <summary>
    /// Checks if the player can perform a charged attack.
    /// </summary>
    /// <returns>True if the player can perform a charged attack, false otherwise.</returns>
    public bool CanChargedAttack()
    {
        if (!EntityStatusEffector.HasStatusEffect<ChargeAttackActivatedStatusEffectSO>(player.gameObject)) return false;
        if (player.CurrentState == player.EntityLaunchState) return false;
        if (player.CurrentState == player.EntityStunnedState) return false;
        if (player.CurrentState == player.PlayerAttackState && !playerCombat.CanCombo) return false;

        return true;
    }

    /// <summary>
    /// Checks if the player can charge.
    /// </summary>
    /// <returns>True if the player can charge, false otherwise.</returns>
    public bool CanCharge()
    {
        if (!EntityStatusEffector.HasStatusEffect<ChargeAttackActivatedStatusEffectSO>(player.gameObject)) return false;
        if (player.CurrentState == player.PlayerChargeState) return false;
        if (player.CurrentState == player.PlayerAttackState) return false;
        if (player.CurrentState == player.PlayerDashState) return false;
        if (player.CurrentState == player.EntityLaunchState) return false;
        if (player.CurrentState == player.EntityStunnedState) return false;

        return true;
    }
}

using KBCore.Refs;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputReader : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private PlayerInput playerInput;
    [SerializeField, Self] private Player player;
    [SerializeField, Self] private PlayerCombat playerCombat;

    public Action<ComboAction> OnComboAction = delegate { };

    public Vector3 MoveDirection { get; private set; }

    [Header("Hold Thresholds")]
    [SerializeField] private float attackReleaseThreshold = 0.25f;
    private float attack1HoldTimer = 0f;
    private float attack2HoldTimer = 0f;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void OnEnable()
    {
        playerInput.actions["Movement"].performed += PlayerInput_OnMovementPerformed;
        playerInput.actions["Movement"].canceled += PlayerInput_OnMovementCanceled;

        playerInput.actions["Jump"].performed += PlayerInput_OnJumpPerformed;

        playerInput.actions["Dash"].performed += PlayerInput_OnDashPerformed;

        playerInput.actions["Sprint"].started += PlayerInput_OnSprintStarted;
        playerInput.actions["Sprint"].canceled += PlayerInput_OnSprintCanceled;    
    }

    private void OnDisable()
    {
        MoveDirection = Vector3.zero;

        playerInput.actions["Movement"].performed -= PlayerInput_OnMovementPerformed;
        playerInput.actions["Movement"].canceled -= PlayerInput_OnMovementCanceled;

        playerInput.actions["Jump"].performed -= PlayerInput_OnJumpPerformed;

        playerInput.actions["Dash"].performed -= PlayerInput_OnDashPerformed;

        playerInput.actions["Sprint"].started -= PlayerInput_OnSprintStarted;
        playerInput.actions["Sprint"].canceled -= PlayerInput_OnSprintCanceled;
    }

    private void Update()
    {
        HandleAttackHoldInputs();
    }

    private void PlayerInput_OnMovementPerformed(InputAction.CallbackContext context)
    {
        MoveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }

    private void PlayerInput_OnMovementCanceled(InputAction.CallbackContext context)
    {
        MoveDirection = Vector3.zero;
    }

    private void PlayerInput_OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (!player.CanJump()) return;

        player.ChangeState(player.PlayerJumpState);
    }

    private void PlayerInput_OnDashPerformed(InputAction.CallbackContext context)
    {
        if(!player.CanDash()) return;

        OnComboAction?.Invoke(ComboAction.DASH);
        player.ChangeState(player.PlayerDashState);
    }

    private void PlayerInput_OnSprintStarted(InputAction.CallbackContext context)
    {
        if(!player.CanSprint()) return;

        player.IsSprinting = true;
    }

    private void PlayerInput_OnSprintCanceled(InputAction.CallbackContext context)
    {
        player.IsSprinting = false;
    }

    private void HandleAttackHoldInputs()
    {
        if (playerInput.actions["Attack1"].IsPressed()) attack1HoldTimer += Time.unscaledDeltaTime;
        if (playerInput.actions["Attack2"].IsPressed()) attack2HoldTimer += Time.unscaledDeltaTime;

        // Charging
        if (attack1HoldTimer > attackReleaseThreshold) OnAttack1Charging();
        if (attack2HoldTimer > attackReleaseThreshold) OnAttack2Charging();

        if (playerInput.actions["Attack1"].WasReleasedThisFrame())
        {
            if (attack1HoldTimer < attackReleaseThreshold) // regular swing
            {
                OnAttack1Performed();
            }
            else // charged swing
            {
                OnAttack1ChargedPerformed();
            }
            attack1HoldTimer = 0f;
        }

        if (playerInput.actions["Attack2"].WasReleasedThisFrame())
        {
            if (attack2HoldTimer < attackReleaseThreshold) // regular swing
            {
                OnAttack2Performed();
            }
            else // charged swing
            {
                OnAttack2ChargedPerformed();
            }
            attack2HoldTimer = 0f;
        }
    }

    private void OnAttack1Performed()
    {
        if (!playerCombat.CanBasicAttack()) return;

        OnComboAction?.Invoke(ComboAction.ATTACK1);
    }

    private void OnAttack2Performed()
    {
        if (!playerCombat.CanBasicAttack()) return;

        OnComboAction?.Invoke(ComboAction.ATTACK2);
    }

    private void OnAttack1ChargedPerformed()
    {
        if (!playerCombat.CanChargedAttack()) return;

        player.ChangeState(player.DefaultState);
        OnComboAction?.Invoke(ComboAction.CHARGED_ATTACK1);
    }

    private void OnAttack2ChargedPerformed()
    {
        if (!playerCombat.CanChargedAttack()) return;

        player.ChangeState(player.DefaultState);
        OnComboAction?.Invoke(ComboAction.CHARGED_ATTACK2);
    }

    private void OnAttack1Charging()
    {
        if (!playerCombat.CanCharge()) return;

        player.ChangeState(player.PlayerChargeState);
    }

    private void OnAttack2Charging()
    {
        if (!playerCombat.CanCharge()) return;

        player.ChangeState(player.PlayerChargeState);
    }
}

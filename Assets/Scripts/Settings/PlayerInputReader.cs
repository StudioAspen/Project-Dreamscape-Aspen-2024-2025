using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputReader : MonoBehaviour
{
    private PlayerInput playerInput;
    private Player player;

    public Action<ComboAction> OnComboAction = delegate { };

    public Vector3 MoveDirection { get; private set; }

    [Header("Hold Thresholds")]
    [SerializeField] private float attackReleaseThreshold = 0.25f;
    private float attack1HoldTimer = 0f;
    private float attack2HoldTimer = 0f;

    [Header("Input Buffer")]
    [SerializeField] private float bufferDuration = 0.3f; // Time in seconds to keep inputs in the buffer
    public Queue<(ComboAction, float timestamp)> InputBuffer = new Queue<(ComboAction, float)>();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
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

        ProcessInputBuffer();
    }

    /// <summary>
    /// Buffers the specified combo action with the current timestamp.
    /// </summary>
    /// <param name="action">The combo action to buffer.</param>
    private void BufferInput(ComboAction action)
    {
        InputBuffer.Enqueue((action, Time.time));
    }

    /// <summary>
    /// Processes the input buffer by checking if the oldest input is still valid and performing the corresponding action.
    /// </summary>
    private void ProcessInputBuffer()
    {
        // Dead players can't perform actions
        if (player.CurrentState == player.EntityDeathState) return;

        if (InputBuffer.Count == 0) return;

        // Get the oldest input in the buffer
        var (action, timestamp) = InputBuffer.Peek();

        // Remove expired inputs
        if (Time.time - timestamp > bufferDuration)
        {
            InputBuffer.Dequeue();
            return;
        }

        // Check if the action can be performed
        if (CanPerformBufferedAction(action))
        {
            if(action == ComboAction.JUMP) player.ChangeState(player.PlayerJumpState);
            if (action == ComboAction.DASH) player.ChangeState(player.PlayerDashState);

            OnComboAction?.Invoke(action);
            InputBuffer.Dequeue();
        }
    }

    /// <summary>
    /// Determines whether the specified combo action can be performed.
    /// </summary>
    /// <param name="action">The combo action to check.</param>
    /// <returns>True if the combo action can be performed, false otherwise.</returns>
    private bool CanPerformBufferedAction(ComboAction action)
    {
        switch (action)
        {
            case ComboAction.ATTACK1:
                return player.PlayerAttackState.CanBasicAttack();
            case ComboAction.CHARGED_ATTACK1:
                return player.PlayerChargeState.CanChargedAttack();
            case ComboAction.ATTACK2:
                return player.PlayerAttackState.CanBasicAttack();
            case ComboAction.CHARGED_ATTACK2:
                return player.PlayerChargeState.CanChargedAttack();
            case ComboAction.DASH:
                return player.PlayerDashState.CanDash();
            case ComboAction.JUMP:
                return player.PlayerJumpState.CanJump();
            default:
                break;
        }

        return false;
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
        BufferInput(ComboAction.JUMP);
    }

    private void PlayerInput_OnDashPerformed(InputAction.CallbackContext context)
    {
        BufferInput(ComboAction.DASH);
    }

    private void PlayerInput_OnSprintStarted(InputAction.CallbackContext context)
    {
        if(!player.PlayerSprintState.CanSprint()) return;

        player.PlayerSprintState.IsSprinting = true;
    }

    private void PlayerInput_OnSprintCanceled(InputAction.CallbackContext context)
    {
        player.PlayerSprintState.IsSprinting = false;
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
        BufferInput(ComboAction.ATTACK1);
    }

    private void OnAttack2Performed()
    {
        BufferInput(ComboAction.ATTACK2);
    }

    private void OnAttack1ChargedPerformed()
    {
        BufferInput(ComboAction.CHARGED_ATTACK1);
    }

    private void OnAttack2ChargedPerformed()
    {
        BufferInput(ComboAction.CHARGED_ATTACK2);
    }

    private void OnAttack1Charging()
    {
        if (!player.PlayerChargeState.CanCharge()) return;

        player.PlayerChargeState.SetChargeAttackInput(1);
        player.ChangeState(player.PlayerChargeState);
    }

    private void OnAttack2Charging()
    {
        if (!player.PlayerChargeState.CanCharge()) return;

        player.PlayerChargeState.SetChargeAttackInput(2);
        player.ChangeState(player.PlayerChargeState);
    }
}

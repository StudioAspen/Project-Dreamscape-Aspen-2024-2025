using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputReader : MonoBehaviour
{
    private PlayerControls playerControls;

    private Player player;
    private MemorySystem memorySystem;

    /// <summary>
    /// Action that is invoked when the player fires a combo action.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>ComboAction incomingAction</c>: The combo action that was just fired.</description></item>
    /// </list>
    /// </remarks>
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
        player = GetComponent<Player>();
        memorySystem = player.GetComponent<MemorySystem>();
    }

    private void OnEnable()
    {
        playerControls = FindObjectOfType<GameInputManager>().PlayerControls;
        
        playerControls.Gameplay.Movement.performed += PlayerControls_OnMovementPerformed;
        playerControls.Gameplay.Movement.canceled += PlayerControls_OnMovementCanceled;

        playerControls.Gameplay.Jump.performed += PlayerControls_OnJumpPerformed;

        playerControls.Gameplay.Dash.performed += PlayerControls_OnDashPerformed;

        playerControls.Gameplay.MemoryAbility.performed += PlayerControls_OnMemoryAbilityPerformed;
    }

    private void OnDisable()
    {
        MoveDirection = Vector3.zero;

        playerControls.Gameplay.Movement.performed -= PlayerControls_OnMovementPerformed;
        playerControls.Gameplay.Movement.canceled -= PlayerControls_OnMovementCanceled;

        playerControls.Gameplay.Jump.performed -= PlayerControls_OnJumpPerformed;

        playerControls.Gameplay.Dash.performed -= PlayerControls_OnDashPerformed;

        playerControls.Gameplay.MemoryAbility.performed -= PlayerControls_OnMemoryAbilityPerformed;
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
        InputBuffer.Enqueue((action, Time.unscaledTime));
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
        if (Time.unscaledTime - timestamp > bufferDuration)
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

    private void PlayerControls_OnMovementPerformed(InputAction.CallbackContext context)
    {
        MoveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }

    private void PlayerControls_OnMovementCanceled(InputAction.CallbackContext context)
    {
        MoveDirection = Vector3.zero;
    }

    private void PlayerControls_OnJumpPerformed(InputAction.CallbackContext context)
    {
        BufferInput(ComboAction.JUMP);
    }

    private void PlayerControls_OnDashPerformed(InputAction.CallbackContext context)
    {
        BufferInput(ComboAction.DASH);
    }

    private void PlayerControls_OnMemoryAbilityPerformed(InputAction.CallbackContext context)
    {
        if (player.CurrentState == player.PlayerAbilityState) return;

        //Debug.LogWarning("Cannot activate memory ability: Memory system not implemented yet.");
        memorySystem.TryActivateMemoryAbility();
    }

    private void HandleAttackHoldInputs()
    {
        if (playerControls.Gameplay.Attack1.IsPressed()) attack1HoldTimer += Time.unscaledDeltaTime;
        if (playerControls.Gameplay.Attack2.IsPressed()) attack2HoldTimer += Time.unscaledDeltaTime;

        // Charging
        if (attack1HoldTimer > attackReleaseThreshold) OnAttack1Charging();
        if (attack2HoldTimer > attackReleaseThreshold) OnAttack2Charging();

        if (playerControls.Gameplay.Attack1.WasReleasedThisFrame())
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

        if (playerControls.Gameplay.Attack2.WasReleasedThisFrame())
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

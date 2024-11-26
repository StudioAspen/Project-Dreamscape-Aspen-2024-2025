using DG.Tweening;
using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private PlayerInputReader input;
    [SerializeField, Self] private Player player;
    [SerializeField, Self] private Animator animator;

    [field: Header("Settings")]
    [field: SerializeField] public Weapon Weapon { get; private set; }
    [HideInInspector] public bool IsAnimationPlaying;
    [HideInInspector] public bool CanCombo;
    [HideInInspector] public bool CanCancelAnimation;

    [field: Header("Combo")]
    [SerializeField] private float comboResetDelay = 1f;
    public List<PlayerActions> CurrentInputsList { get; private set; } = new List<PlayerActions>();
    private List<ComboDataSO> potentialCombos = new List<ComboDataSO>();
    private List<ComboDataSO> predictedCombos = new List<ComboDataSO>();

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void OnEnable()
    {
        input.Attack1.AddListener(Input_HandleAttack1Input);
        //input.Attack1Charged.AddListener(Input_HandleAttack1ChargedInput);
        //input.Attack1Charging.AddListener(Input_HandleAttackChargingInput);
        input.Attack2.AddListener(Input_HandleAttack2Input);
        //input.Attack2Charged.AddListener(Input_HandleAttack2ChargedInput);
        //input.Attack2Charging.AddListener(Input_HandleAttackChargingInput);

        input.OnPlayerActionInput.AddListener(Input_HandleOnPlayerActionInput);

        player.OnGrounded.AddListener(Player_OnGrounded);
        player.OnAirborne.AddListener(Player_OnAirborne);
    }

    private void OnDisable()
    {
        input.Attack1.RemoveListener(Input_HandleAttack1Input);
        //input.Attack1Charged.RemoveListener(Input_HandleAttack1ChargedInput);
        //input.Attack1Charging.RemoveListener(Input_HandleAttackChargingInput);
        input.Attack2.RemoveListener(Input_HandleAttack2Input);
        //input.Attack2Charged.RemoveListener(Input_HandleAttack2ChargedInput);
        //input.Attack2Charging.RemoveListener(Input_HandleAttackChargingInput);

        input.OnPlayerActionInput.RemoveListener(Input_HandleOnPlayerActionInput);

        player.OnGrounded.RemoveListener(Player_OnGrounded);
        player.OnAirborne.RemoveListener(Player_OnAirborne);
    }

    private void Update()
    {
        HandleWeaponTriggers();
    }

    private void Input_HandleAttack1Input()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerChargeState) return;
        if (player.CurrentState == player.EntityLaunchState) return;
        if (player.CurrentState == player.PlayerAttackState && !CanCombo) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.ATTACK1);
    }

    private void Input_HandleAttack1ChargedInput()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerAttackState) return;
        if (player.CurrentState == player.EntityLaunchState) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.CHARGED_ATTACK1);
    }

    private void Input_HandleAttackChargingInput()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerChargeState) return;
        if (player.CurrentState == player.PlayerAttackState) return;
        if (player.CurrentState == player.PlayerDashState) return;
        if (player.CurrentState == player.EntityLaunchState) return;

        player.ChangeState(player.PlayerChargeState);
    }

    private void Input_HandleAttack2Input()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerChargeState) return;
        if (player.CurrentState == player.EntityLaunchState) return;
        if (player.CurrentState == player.PlayerAttackState && !CanCombo) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.ATTACK2);
    }

    private void Input_HandleAttack2ChargedInput()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerAttackState) return;
        if (player.CurrentState == player.EntityLaunchState) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.CHARGED_ATTACK2);
    }

    private void Player_OnAirborne(Vector3 startAirbornePosition)
    {
        ResetCombos();
    }

    private void Player_OnGrounded(Vector3 startGroundedPosition)
    {
        ResetCombos();
    }

    private void Input_HandleOnPlayerActionInput(PlayerActions incomingAction)
    {
        CurrentInputsList.Add(incomingAction);

        GenerateComboLists(Weapon.GetCombos(!player.IsGrounded));

        // if the incoming action is not an attack action, the combo list is reset after a delay.
        if (!IsAttackAction(incomingAction)) StartDelayedComboReset();

        // if the incoming action doesn't create any valid combos, the combo list is restarted with only the new action.
        if (predictedCombos.Count == 0)
        {
            CurrentInputsList.Clear();
            CurrentInputsList.Add(incomingAction);
            GenerateComboLists(Weapon.GetCombos(!player.IsGrounded));
        }

        if (IsAttackAction(incomingAction)) TryExecuteCombo(ComboDataSO.GetLongestCombo(potentialCombos));
    }

    /// <summary>
    /// Tries to execute the given combo if it is not null and the player's current state allows it.
    /// </summary>
    /// <param name="combo">The combo to execute.</param>
    private void TryExecuteCombo(ComboDataSO combo)
    {
        if (combo == null)
        {
            //Debug.LogWarning($"Executed combo is null with combo lists:\n{PrintComboLists(false)}");
            return;
        }

        if (player.CurrentState == player.PlayerSlideState) return;
        if (player.CurrentState == player.EntityStaggeredState) return;

        player.PlayerAttackState.SetCombo(combo);
        player.ForceChangeState(player.PlayerAttackState);
    }

    /// <summary>
    /// Generates the combo lists based on the valid combos and the current inputs.
    /// </summary>
    /// <param name="validCombos">The list of valid combos.</param>
    private void GenerateComboLists(List<ComboDataSO> validCombos)
    {
        potentialCombos = new List<ComboDataSO>();
        predictedCombos = new List<ComboDataSO>();
        foreach (ComboDataSO weaponCombo in validCombos)
        {
            if (ComboDataSO.IsIn(weaponCombo.ComboInputs, CurrentInputsList)) potentialCombos.Add(weaponCombo);
            if (ComboDataSO.IsPotentiallyIn(weaponCombo.ComboInputs, CurrentInputsList)) predictedCombos.Add(weaponCombo);
        }
    }

    /// <summary>
    /// Resets the combo lists and clears the current inputs, potential combos, and predicted combos.
    /// </summary>
    public void ResetCombos()
    {
        DOTween.Kill("DelayedComboReset");

        CurrentInputsList.Clear();
        potentialCombos.Clear();
        predictedCombos.Clear();
    }

    /// <summary>
    /// Returns the current combo, potential combos, and predicted combos in printable format.
    /// </summary>
    /// <param name="willPrint">Whether to print the combo lists to the console.</param>
    /// <returns>A string representation of the combo lists.</returns>
    private string PrintComboLists(bool willPrint = true)
    {
        string result = "Current Combo: { ";

        for (int i = 0; i < CurrentInputsList.Count; i++)
        {
            result += CurrentInputsList[i].ToString();
            if (i != CurrentInputsList.Count - 1) result += ",";
            result += " ";
        }

        result += "}\nPotential Combos: { ";

        for (int i = 0; i < potentialCombos.Count; i++)
        {
            result += potentialCombos[i].name;
            if (i != potentialCombos.Count - 1) result += ",";
            result += " ";
        }

        result += "}\nPredicted Combos: { ";

        for (int i = 0; i < predictedCombos.Count; i++)
        {
            result += predictedCombos[i].name;
            if (i != predictedCombos.Count - 1) result += ",";
            result += " ";
        }

        result += "}";

        if (willPrint) Debug.Log(result);

        return result;
    }

    /// <summary>
    /// Checks if the given action is an attack action.
    /// </summary>
    /// <param name="action">The player action to check.</param>
    /// <returns>True if the action is an attack action, false otherwise.</returns>
    private bool IsAttackAction(PlayerActions action)
    {
        return action == PlayerActions.ATTACK1
            || action == PlayerActions.ATTACK2
            || action == PlayerActions.CHARGED_ATTACK1
            || action == PlayerActions.CHARGED_ATTACK2;
    }

    /// <summary>
    /// Starts a delayed reset of the combo lists by using DOTween to delay the execution of the ResetCombos method.
    /// </summary>
    private void StartDelayedComboReset()
    {
        DOTween.Kill("DelayedComboReset");
        DOVirtual.DelayedCall(comboResetDelay, ResetCombos).SetId("DelayedComboReset");
    }

    /// <summary>
    /// Handles the weapon triggers based on the player's current state.
    /// A backup in case the PlayerAttackState doesn't do it.
    /// If the player's current state is not the PlayerAttackState, it calls the EndHit method.
    /// </summary>
    private void HandleWeaponTriggers()
    {
        if (player.CurrentState != player.PlayerAttackState) EndHit();
    }

    /// <summary>
    /// Start the hit by enabling the weapon triggers.
    /// Called by an animation event.
    /// </summary>
    public void StartHit()
    {
        Weapon.EnableTriggers();
    }

    /// <summary>
    /// Ends the hit by disabling the weapon triggers.
    /// Called by an animation event.
    /// </summary>
    public void EndHit()
    {
        Weapon.DisableTriggers();
    }

    /// <summary>
    /// Allows the next combo to be executed mid-animation.
    /// Called by an animation event.
    /// </summary>
    public void EnableCombo()
    {
        CanCombo = true;
    }

    public void DisableCombo()
    {
        CanCombo = false;

        ResetCombos();
    }

    /// <summary>
    /// Finish the animation and clear the combo lists if animation cancellation is allowed.
    /// Animation cancelling is disabled for the first half of the attack animation to prevent premature cancelling bug.
    /// Called at the end of an attack animation through an event.
    /// </summary>
    public void FinishAnimation()
    {
        if (!CanCancelAnimation) return;

        IsAnimationPlaying = false;

        ResetCombos();
    }
}


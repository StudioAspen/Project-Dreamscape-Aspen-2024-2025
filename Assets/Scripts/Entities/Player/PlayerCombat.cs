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
    [Header("Player: Debug UI")]
    [SerializeField] private TMP_Text inputsText;
    [SerializeField] private TMP_Text comboText;

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
    private List<PlayerActions> currentComboList = new List<PlayerActions>();
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

        DebugUICombos();
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
        ClearComboLists();
    }

    private void Player_OnGrounded(Vector3 startGroundedPosition)
    {
        ClearComboLists();
    }

    public void ClearComboLists()
    {
        currentComboList.Clear();
        potentialCombos.Clear();
        predictedCombos.Clear();
    }

    private void Input_HandleOnPlayerActionInput(PlayerActions incomingAction)
    {
        currentComboList.Add(incomingAction);

        GenerateComboLists(Weapon.GetCombos(!player.IsGrounded));

        AttemptToExecuteCombo(incomingAction);
    }

    private void AttemptToExecuteCombo(PlayerActions incomingAction)
    {
        // if new action doesn't create any valid combos, restart the combo list with the new action
        if (predictedCombos.Count == 0) 
        {
            currentComboList.Clear();
            currentComboList.Add(incomingAction);
            GenerateComboLists(Weapon.GetCombos(!player.IsGrounded));
        }

        ExecuteCombo(ComboDataSO.GetLongestCombo(potentialCombos));
    }

    private void ExecuteCombo(ComboDataSO combo)
    {
        if(combo == null)
        {
            //Debug.LogWarning($"Executed combo is null with combo lists:\n{PrintComboLists(false)}");
            return;
        }

        if (player.CurrentState == player.PlayerSlideState) return;
        if (player.CurrentState == player.EntityStaggeredState) return;

        player.PlayerAttackState.SetCombo(combo);
        player.ForceChangeState(player.PlayerAttackState);

        comboText.text = "Combo: " + combo.name;
    }

    private void GenerateComboLists(List<ComboDataSO> validCombos)
    {
        potentialCombos = new List<ComboDataSO>();
        predictedCombos = new List<ComboDataSO>();
        foreach (ComboDataSO weaponCombo in validCombos)
        {
            if (ComboDataSO.IsIn(weaponCombo.ComboInputs, currentComboList)) potentialCombos.Add(weaponCombo);
            if (ComboDataSO.IsPotentiallyIn(weaponCombo.ComboInputs, currentComboList)) predictedCombos.Add(weaponCombo);
        }
    }

    private void DebugUICombos()
    {
        string inputs = "Inputs: ";

        for (int i = 0; i < currentComboList.Count; i++)
        {
            inputs += currentComboList[i].ToString();
            if (i != currentComboList.Count - 1) inputs += ",";
            inputs += " ";
        }

        inputsText.text = inputs;

        if (currentComboList.Count == 0) comboText.text = "Combo: ";
    }

    private string PrintComboLists(bool willPrint = true)
    {
        string result = "Current Combo: { ";

        for (int i = 0; i < currentComboList.Count; i++)
        {
            result += currentComboList[i].ToString();
            if (i != currentComboList.Count - 1) result += ",";
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

        if(willPrint) Debug.Log(result);
        
        return result;
    }

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

        ClearComboLists();
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

        ClearComboLists();
    }
}


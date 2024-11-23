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

    [Header("Combo")]
    [SerializeField] private float comboListenDuration = 1f;
    private float comboListenTimer;
    private List<PlayerActions> currentComboList = new List<PlayerActions>();
    private List<ComboDataSO> potentialCombos = new List<ComboDataSO>();
    private List<ComboDataSO> predictedCombos = new List<ComboDataSO>();

    public bool CanCombo;

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
        HandleComboList();
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
        comboListenTimer = 0;

        if (currentComboList.Count == 0)
        {
            currentComboList.Add(PlayerActions.AIRBORNE);
            GenerateComboLists();
            return;
        }

        currentComboList.Clear();
        currentComboList.Add(PlayerActions.AIRBORNE);
        GenerateComboLists();
    }

    private void Player_OnGrounded(Vector3 startGroundedPosition)
    {
        comboListenTimer = 0;

        currentComboList.Clear();

        GenerateComboLists();
    }

    private void HandleComboList()
    {
        if (player.CurrentState != player.PlayerChargeState && player.CurrentState != player.PlayerAttackState && comboListenTimer < comboListenDuration * 2f) comboListenTimer += Time.unscaledDeltaTime;

        if (comboListenTimer > comboListenDuration)
        {
            currentComboList.Clear();
            potentialCombos.Clear();
            predictedCombos.Clear();
        }
    }

    private void Input_HandleOnPlayerActionInput(PlayerActions incomingAction)
    {
        comboListenTimer = 0;

        currentComboList.Add(incomingAction);

        if (!player.IsGrounded && currentComboList.Count > 0)
        {
            if (currentComboList[0] != PlayerActions.AIRBORNE) currentComboList.Insert(0, PlayerActions.AIRBORNE);
        }
        GenerateComboLists();

        AttemptToExecuteACombo(incomingAction);
    }

    private void AttemptToExecuteACombo(PlayerActions incomingAction)
    {
        ComboDataSO comboToExecute = null;

        if (predictedCombos.Count == 0) // if new action doesn't create any valid combos
        {
            currentComboList.Clear();
            currentComboList.Add(incomingAction);

            if (!player.IsGrounded)
            {
                currentComboList.Insert(0, PlayerActions.AIRBORNE);
                GenerateComboLists();

                comboToExecute = ComboDataSO.GetLongestCombo(potentialCombos);

                if (comboToExecute != null)
                {
                    ExecuteCombo(comboToExecute);
                }
            }
            else
            {
                comboToExecute = ComboDataSO.GetSingleActionCombo(Weapon.Combos, incomingAction);
                if (comboToExecute != null)
                {
                    ExecuteCombo(comboToExecute);
                }
            }
        }
        else
        {
            comboToExecute = ComboDataSO.GetLongestCombo(potentialCombos);

            if (comboToExecute != null)
            {
                ExecuteCombo(comboToExecute);
            }
        }

        //PrintComboLists();
    }

    private void ExecuteCombo(ComboDataSO combo)
    {
        if (player.CurrentState == player.PlayerSlideState) return;
        if (player.CurrentState == player.EntityStaggeredState) return;

        player.PlayerAttackState.SetCombo(combo);
        player.ForceChangeState(player.PlayerAttackState);

        comboText.text = "Combo: " + combo.name;
    }

    private void GenerateComboLists()
    {
        potentialCombos = new List<ComboDataSO>();
        predictedCombos = new List<ComboDataSO>();
        foreach (ComboDataSO weaponCombo in Weapon.Combos)
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

    private void PrintComboLists()
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

        Debug.Log(result);
    }

    private void HandleWeaponTriggers()
    {
        if (player.CurrentState != player.PlayerAttackState) EndHit();
    }

    public void StartHit()
    {
        Weapon.EnableTriggers();
    }

    public void EndHit()
    {
        Weapon.DisableTriggers();
    }

    public void EnableCombo()
    {
        CanCombo = true;
    }

    public void DisableCombo()
    {
        CanCombo = true;
    }

    public void FinishAnimation()
    {
        IsAnimationPlaying = false;
    }
}


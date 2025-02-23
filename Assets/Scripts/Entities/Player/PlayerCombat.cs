using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Player player;
    private PlayerInputReader playerInputReader;
    private Animator animator;

    [field: Header("Settings")]
    [field: SerializeField] public Weapon Weapon { get; private set; }
    [HideInInspector] public bool CanCombo;

    [field: Header("Combo")]
    [SerializeField] private float nonAttackComboResetDelay = 1f;
    [field: SerializeField] public float AttackComboResetDelay { get; private set; } = 0.1f;
    private Coroutine delayedComboResetCoroutine;
    public List<ComboAction> CurrentInputsList { get; private set; } = new List<ComboAction>();
    private List<ComboDataSO> potentialCombos = new List<ComboDataSO>();
    private List<ComboDataSO> predictedCombos = new List<ComboDataSO>();

    /// <summary>
    /// Action that is invoked when the player starts charging.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>int attackInputNumber</c>: The attack input that is being charged. (1 or 2)</description></item>
    /// </list>
    /// </remarks>
    public Action<int> OnChargeStart = delegate { }; // parameter is which attack is charging
    /// <summary>
    /// Action that is invoked when the player releases the charge.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>int attackInputNumber</c>: The attack input that was charged. (1 or 2)</description></item>
    /// <item><description><c>float chargeDuration</c>: The charge duration.</description></item>
    /// </list>
    /// </remarks>
    public Action<int, float> OnChargeRelease = delegate { };

    private void Start()
    {
        player = GetComponent<Player>();
        playerInputReader = GetComponent<PlayerInputReader>();
        animator = GetComponent<Animator>();

        playerInputReader.OnComboAction += PlayerInputReader_OnComboAction;

        player.OnGrounded += Player_OnGrounded;
        player.OnAirborne += Player_OnAirborne;

        Weapon.OnWeaponHit += Weapon_OnWeaponHit;
    }

    private void OnDestroy()
    {
        playerInputReader.OnComboAction -= PlayerInputReader_OnComboAction;

        player.OnGrounded -= Player_OnGrounded;
        player.OnAirborne -= Player_OnAirborne;

        Weapon.OnWeaponHit -= Weapon_OnWeaponHit;
    }

    private void Update()
    {
        HandleWeaponTriggers();
    }

    private void Weapon_OnWeaponHit(Entity attacker, Entity victim, Vector3 hitPoint, int damage)
    {
        CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);
    }

    private void Player_OnAirborne(Vector3 startAirbornePosition)
    {
        ResetCombos();
    }

    private void Player_OnGrounded(Vector3 startGroundedPosition)
    {
        ResetCombos();
    }

    private void PlayerInputReader_OnComboAction(ComboAction incomingAction)
    {
        CurrentInputsList.Add(incomingAction);

        GenerateComboLists(Weapon.GetCombos(!player.IsGrounded));

        // if the incoming action is not an attack action, the combo list is reset after a delay.
        if (!IsAttackAction(incomingAction)) StartDelayedComboListsReset(nonAttackComboResetDelay);

        // if the incoming action doesn't create any valid combos, the combo list is restarted with only the new action.
        if (predictedCombos.Count == 0)
        {
            CurrentInputsList.Clear();
            CurrentInputsList.Add(incomingAction);
            GenerateComboLists(Weapon.GetCombos(!player.IsGrounded));
        }

        if (IsAttackAction(incomingAction))
        {
            bool successfullyExecutedCombo = TryExecuteCombo(ComboDataSO.GetLongestCombo(potentialCombos));
            if (!successfullyExecutedCombo && player.CurrentState == player.PlayerChargeState) player.ChangeState(player.DefaultState);
        }
    }

    /// <summary>
    /// Tries to execute the given combo if it is not null and the player's current state allows it.
    /// </summary>
    /// <param name="combo">The combo to execute.</param>
    /// <returns>Whether a combo was executed</returns>
    private bool TryExecuteCombo(ComboDataSO combo)
    {
        if (combo == null)
        {
            //Debug.LogWarning($"Executed combo is null with combo lists:\n{PrintComboLists(false)}");
            return false;
        }

        if (player.CurrentState == player.PlayerSlideState) return false;
        if (player.CurrentState == player.EntityStaggeredState) return false;

        player.PlayerAttackState.SetCombo(combo);
        player.ChangeState(player.PlayerAttackState, true);
        return true;
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
        if (delayedComboResetCoroutine != null) StopCoroutine(delayedComboResetCoroutine);

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
    private bool IsAttackAction(ComboAction action)
    {
        return action == ComboAction.ATTACK1
            || action == ComboAction.ATTACK2
            || action == ComboAction.CHARGED_ATTACK1
            || action == ComboAction.CHARGED_ATTACK2;
    }

    /// <summary>
    /// Starts a delayed reset of the combo lists by using DOTween to delay the execution of the ResetCombos method.
    /// </summary>
    /// /// <param name="delay">The delay until the combo lists are reset.</param>
    public void StartDelayedComboListsReset(float delay)
    {
        if(delayedComboResetCoroutine != null) StopCoroutine(delayedComboResetCoroutine);
        delayedComboResetCoroutine = StartCoroutine(DelayedComboResetCoroutine(delay));
    }

    private IEnumerator DelayedComboResetCoroutine(float delay)
    {
        float elapsedTime = 0;
        while(elapsedTime < delay)
        {
            elapsedTime += player.LocalDeltaTime;
            yield return null;

            if (player.CurrentState == player.PlayerAttackState)
            {
                delayedComboResetCoroutine = null;
                yield break;
            }
            if (player.CurrentState == player.PlayerChargeState)
            {
                delayedComboResetCoroutine = null;
                yield break;
            }
        }

        ResetCombos();

        delayedComboResetCoroutine = null;
    }

    /// <summary>
    /// Sets the speed of the combo animation.
    /// </summary>
    /// <param name="speed">The speed value to set.</param>
    public void SetComboAnimationSpeed(float speed)
    {
        animator.SetFloat("ComboAnimationSpeed", speed);
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
    /// Called by animation through an event.
    /// </summary>
    public void FireAbility()
    {
        if (player.CurrentState != player.PlayerAttackState) return;

        player.PlayerAttackState.FireAbility();
    }
}
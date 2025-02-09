using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerDebugUI : MonoBehaviour
{
    private Player player;
    private PlayerCombat playerCombat;
    private ChainingSystem chainingSystem;
    private MomentumSystem momentumSystem;
    private LevelSystem levelSystem;
    private HealthBarUI healthBarUI;

    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text inputsText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text chainText;
    [SerializeField] private TMP_Text momentumText;
    [SerializeField] private TMP_Text levelText;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        playerCombat = player.GetComponent<PlayerCombat>();
        chainingSystem = player.GetComponent<ChainingSystem>();
        momentumSystem = player.GetComponent<MomentumSystem>();
        levelSystem = player.GetComponent<LevelSystem>();
        healthBarUI = GetComponentInChildren<HealthBarUI>();

        player.OnEntityTakeDamage += Entity_OnEntityTakeDamage;
        
        if(playerCombat != null) if(playerCombat.Weapon != null) playerCombat.Weapon.OnWeaponStartSwing += Weapon_OnWeaponStartSwing;
    }

    private void Start()
    {
        // Need to delay start for a frame because of potential race condition as Player hp is also set in start.
        StartCoroutine(LateStartCoroutine());
    }

    private void LateStart()
    {
        healthBarUI.SetHealthBar(player.CurrentHealth, player.MaxHealth);
    }

    private IEnumerator LateStartCoroutine()
    {
        yield return null;

        LateStart();
    }

    private void OnDestroy()
    {
        player.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;

        if (playerCombat != null) if (playerCombat.Weapon != null) playerCombat.Weapon.OnWeaponStartSwing -= Weapon_OnWeaponStartSwing;

    }

    private void LateUpdate()
    {
        stateText.text = player.CurrentState.GetType().ToString();
        inputsText.text = playerCombat == null ? "Missing PlayerCombat component." : $"Inputs: {GetInputsListString()}";
        chainText.text = chainingSystem == null ? "Missing ChainingSystem component." : $"Chain: {chainingSystem.ChainCount}";
        momentumText.text = momentumSystem == null ? "Missing MomentumSystem component." : $"Momentum: {momentumSystem.Momentum}";
        levelText.text = levelSystem == null ? "Missing LevelSystem component." : $"Level: {levelSystem.Level}, EXP: {levelSystem.CurrentEXP}/{levelSystem.MaxEXP}";
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        healthBarUI.SetHealthBar(player.CurrentHealth - damage, player.MaxHealth);
    }

    private void Weapon_OnWeaponStartSwing(Entity source)
    {
        comboText.text = playerCombat == null ? "Missing PlayerCombat component." : $"Combo: {player.PlayerAttackState.ComboData.name}";
    }

    private string GetInputsListString()
    {
        string inputs = "";

        for (int i = 0; i < playerCombat.CurrentInputsList.Count; i++)
        {
            inputs += playerCombat.CurrentInputsList[i].ToString();
            if (i != playerCombat.CurrentInputsList.Count - 1) inputs += ",";
            inputs += " ";
        }

        inputsText.text = inputs;

        if (playerCombat.CurrentInputsList.Count == 0) comboText.text = "Combo: ";

        return inputs;
    }
}

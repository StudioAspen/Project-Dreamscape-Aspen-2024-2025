using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDebugUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Parent] private Player player;
    private PlayerCombat playerCombat;
    private ChainingSystem chainingSystem;
    private MomentumSystem momentumSystem;
    [SerializeField, Child] private HealthBarUI healthBarUI;

    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text inputsText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text chainText;
    [SerializeField] private TMP_Text momentumText;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        playerCombat = player.GetComponent<PlayerCombat>();
        chainingSystem = player.GetComponent<ChainingSystem>();
        momentumSystem = player.GetComponent<MomentumSystem>();

        player.OnEntityTakeDamage += Entity_OnEntityTakeDamage;
        
        if(playerCombat != null) if(playerCombat.Weapon != null) playerCombat.Weapon.OnWeaponStartSwing += Weapon_OnWeaponStartSwing;
    }

    private void Start()
    {
        healthBarUI.SetHealthBar(player.CurrentHealth, player.MaxHealth);
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

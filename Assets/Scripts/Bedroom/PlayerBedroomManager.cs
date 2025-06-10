using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBedroomManager : MonoBehaviour
{
    private Player player;
    
    [SerializeField] private BedroomItemCollectionSO bedroomItemCollection;

    private BedroomSaveData currentBedroomSaveData;

    private void Start()
    {
        player = GetComponent<Player>();

        currentBedroomSaveData = SaveLoadManager.LoadBedroomData();

        ApplyActivatedBedroomItemBuffs();

        player.OnKillEntity += Player_OnKillEntity;
    }

    private void OnDestroy()
    {
        player.OnKillEntity -= Player_OnKillEntity;
    }

    private void Player_OnKillEntity(Entity victim)
    {
        if (Slime.IsEntityACloneSlime(victim))
        {
            Debug.Log("Killed a clone slime, no bedroom currency added.");
            return;
        }

        int currencyToAdd = 1; // Default amount to add

        // TODO: Make different amounts based on entity type

        AddCurrencyToSave(currencyToAdd);
    }

    private void ApplyActivatedBedroomItemBuffs()
    {
        if(currentBedroomSaveData == null)
        {
            Debug.LogWarning("Current bedroom save data is null, cannot apply buffs.");
            return;
        }

        HashSet<int> activatedItemIDs = new HashSet<int>(currentBedroomSaveData.ActivatedItemIDs);
        if (activatedItemIDs == null)
        {
            Debug.LogWarning("Loaded items list is null.");
            return;
        }

        foreach (var item in bedroomItemCollection.Items)
        {
            if (activatedItemIDs.Contains(item.UniqueID))
            {
                Debug.Log($"Activating item: {item.DisplayName} with ID: {item.UniqueID}");
                ActivateItemBuff(item.ActivatedStatBuffType, item.BuffMultiplier, item.BuffFlatIncrease);
            }
        }
    }

    private void ActivateItemBuff(BedroomItemConfigSO.Stat statType, float multiplier, float flatIncrease)
    {
        Debug.Log($"BedroomItem: Applying {statType} with mult: {multiplier}, flat: {flatIncrease}");

        switch (statType)
        {
            case BedroomItemConfigSO.Stat.MAX_HEALTH:
                player.MaxHealth.AddMultiplier(multiplier, this);
                player.MaxHealth.AddFlatAmount(flatIncrease, this);
                break;
            case BedroomItemConfigSO.Stat.DEFENSE:
                player.Defense.AddMultiplier(multiplier, this);
                player.Defense.AddFlatAmount(flatIncrease, this);
                break;
            case BedroomItemConfigSO.Stat.SPEED:
                player.StatusSpeedModifier.AddMultiplier(multiplier, this);
                player.StatusSpeedModifier.AddFlatAmount(flatIncrease, this);
                break;
            case BedroomItemConfigSO.Stat.DAMAGE:
                player.DamageModifier.AddMultiplier(multiplier, this);
                player.DamageModifier.AddFlatAmount(flatIncrease, this);
                break;
            case BedroomItemConfigSO.Stat.ATTACK_SPEED:
                // TODO
                break;
            default:
                break;
        }
    }

    private void AddCurrencyToSave(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Attempted to add non-positive currency amount: " + amount);
            return;
        }

        currentBedroomSaveData.Currency += amount;
        SaveLoadManager.SaveBedroomData(currentBedroomSaveData);
    }
}
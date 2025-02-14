using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "Data", menuName = "Volcanic Biome Variant Status Effect SO")]
public class VolcanicBiomeVariantStatusEffectSO : BiomeVariantStatusEffectSO
{
    // Base Class: You have access to the Entity entity being affected and the GameObject source of the object that applied this effect

    [field: Header("Volcanic Biome Variant Status Effect: Settings")]
    [field: SerializeField] public BurnStatusEffectSO BurnStatusEffect { get; private set; }

    private protected override void OnApply()
    {
        base.OnApply();

        // your logic for when the effect is applied 
        //source.GetComponent<Entity>().OnEntityDealDamage += Entity_OnEntityDealDamage; // doesnt work as source is the spawner
        entity.OnEntityDealDamage += Entity_OnEntityDealDamage;
    }

    public override void Cancel()
    {
        base.Cancel();

        // your logic for when the effect is cancelled
        //source.GetComponent<Entity>().OnEntityDealDamage -= Entity_OnEntityDealDamage; // doesnt work as source is the spawner
        entity.OnEntityDealDamage -= Entity_OnEntityDealDamage;
    }

    private void Entity_OnEntityDealDamage(Entity attacker, Entity victim, Vector3 hitPoint, int damageValue)
    {
        //EntityStatusEffector.TryApplyStatusEffect(victim.gameObject, BurnStatusEffect, attacker.gameObject);
        //victim.gameObject.GetComponent<EntityStatusEffector>().ApplyStatusEffect(BurnStatusEffect, source.gameObject);

        Debug.Log("player hit valconooooooooooooooooooooo");
    }
}

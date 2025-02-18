using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityDebugCanvasUI : MonoBehaviour
{
    private Entity entity;
    private EntityStatusEffector entityStatusEffector;
    private HealthBarUI healthBarUI;

    [Header("References")]
    [SerializeField] private TMP_Text entityStateText;
    [SerializeField] private TMP_Text entityNameText;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityStatusEffector = entity.GetComponent<EntityStatusEffector>();
        healthBarUI = GetComponentInChildren<HealthBarUI>();

        entity.OnEntityTakeDamage += Entity_OnEntityTakeDamage;
        entity.OnEntityHeal += Entity_OnEntityHeal;
    }

    private void OnDestroy()
    {
        entity.OnEntityTakeDamage -= Entity_OnEntityTakeDamage;
        entity.OnEntityHeal -= Entity_OnEntityHeal;
    }

    private void OnEnable()
    {
        // Need to delay OnEnable for a frame because of potential race condition as entity hp is also set in OnEnable.
        StartCoroutine(LateOnEnableCoroutine());
    }

    private void LateOnEnable()
    {
        healthBarUI.SetHealthBar(entity.CurrentHealth, entity.MaxHealth.GetIntValue());

        EliteVariantStatusEffectSO eliteStatus = null;
        if (entityStatusEffector != null)
        {
            foreach (StatusEffectSO statusEffect in entityStatusEffector.CurrentStatusEffects.Values)
            {
                EliteVariantStatusEffectSO eliteVariantStatusEffect = statusEffect as EliteVariantStatusEffectSO;
                if (eliteVariantStatusEffect != null)
                {
                    eliteStatus = eliteVariantStatusEffect;
                    break;
                }
            }
        }

        BiomeVariantStatusEffectSO biomeVariantStatus = null;
        if (entityStatusEffector != null)
        {
            foreach (StatusEffectSO statusEffect in entityStatusEffector.CurrentStatusEffects.Values)
            {
                BiomeVariantStatusEffectSO biomeVariantStatusEffect = statusEffect as BiomeVariantStatusEffectSO;
                if (biomeVariantStatusEffect != null)
                {
                    biomeVariantStatus = biomeVariantStatusEffect;
                    break;
                }
            }
        }

        if (entityNameText != null) entityNameText.text = 
                $"{(eliteStatus == null ? "" : $"Elite {eliteStatus.Name} ")}" +
                $"{(biomeVariantStatus == null ? "" : $"{biomeVariantStatus.Name} ")}" +
                $"{entity.GetType()}";
    }

    private IEnumerator LateOnEnableCoroutine()
    {
        yield return null;

        LateOnEnable();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        entityStateText.text = entity.CurrentState.GetType().ToString();
    }

    private void Entity_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        healthBarUI.SetHealthBar(entity.CurrentHealth - damage, entity.MaxHealth.GetIntValue());
    }

    private void Entity_OnEntityHeal(Entity healedEntity, int healValue)
    {
        healthBarUI.SetHealthBar(entity.CurrentHealth + healValue, entity.MaxHealth.GetIntValue());
    }
}

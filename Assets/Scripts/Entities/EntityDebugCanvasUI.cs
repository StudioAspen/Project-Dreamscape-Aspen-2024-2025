using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityDebugCanvasUI : MonoBehaviour
{
    private Entity entity;
    private HealthBarUI healthBarUI;

    [Header("References")]
    [SerializeField] private TMP_Text entityStateText;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
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
        StartCoroutine(LateOnEnableCoroutine());
    }

    private void LateOnEnable()
    {
        healthBarUI.SetHealthBar(entity.CurrentHealth, entity.MaxHealth);
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
        healthBarUI.SetHealthBar(entity.CurrentHealth - damage, entity.MaxHealth);
    }

    private void Entity_OnEntityHeal(Entity healedEntity, int healValue)
    {
        healthBarUI.SetHealthBar(entity.CurrentHealth + healValue, entity.MaxHealth);
    }
}

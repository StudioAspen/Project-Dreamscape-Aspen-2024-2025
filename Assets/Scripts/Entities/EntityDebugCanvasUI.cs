using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityDebugCanvasUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Parent] private Entity entity;
    [SerializeField, Child] private HealthBarUI healthBarUI;
    [SerializeField] private TMP_Text entityStateText;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        entity.OnEntityTakeDamage.AddListener(Entity_OnEntityTakeDamage);
    }

    private void OnDestroy()
    {
        entity.OnEntityTakeDamage.RemoveListener(Entity_OnEntityTakeDamage);
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
}

using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline;
using UnityEngine;

public class ChainingSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private PlayerCombat playerCombat;
    [SerializeField] private TMP_Text debugText;

    [Header("Settings")]
    [SerializeField] private float timeBetween = 1f;
    private float timer;

    private int chainCount;
    public int ChainCount => chainCount;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Start()
    {
        Reset();
    }

    private void OnEnable()
    {
        playerCombat.Weapon.OnWeaponHit.AddListener(PlayerWeapon_OnWeaponHit);
    }

    private void OnDisable()
    {
        playerCombat.Weapon.OnWeaponHit.RemoveListener(PlayerWeapon_OnWeaponHit);
    }

    private void Update()
    {
        HandleChaining();
    }

    private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        AddChain();
    }

    private void HandleChaining()
    {
        if (chainCount > 0)
        {
            timer += Time.deltaTime;
        }

        if (timer > timeBetween)
        {
            Reset();
        }
    }

    public void AddChain()
    {
        chainCount++;
        timer = 0f;

        debugText.text = $"Chain: {chainCount}";
    }

    private void Reset()
    {
        chainCount = 0;
        timer = 0;

        debugText.text = $"Chain: {chainCount}";
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChainingSystem : MonoBehaviour
{
    private PlayerCombat playerCombat;

    [Header("Settings")]
    [SerializeField] private float timeBetween = 1f;
    private float timer;

    public int ChainCount { get; private set; }

    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Start()
    {
        ResetChain();
    }

    private void OnEnable()
    {
        playerCombat.Weapon.OnWeaponHit += PlayerWeapon_OnWeaponHit;
    }

    private void OnDisable()
    {
        playerCombat.Weapon.OnWeaponHit -= PlayerWeapon_OnWeaponHit;
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
        if (ChainCount > 0)
        {
            timer += Time.deltaTime;
        }

        if (timer > timeBetween)
        {
            ResetChain();
        }
    }

    public void AddChain()
    {
        ChainCount++;
        timer = 0f;
    }

    private void ResetChain()
    {
        ChainCount = 0;
        timer = 0;
    }
}

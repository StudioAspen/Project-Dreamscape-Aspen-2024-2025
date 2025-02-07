using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChainingSystem : MonoBehaviour
{
    private PlayerCombat playerCombat;
    private LevelSystem levelSystem;

    [Header("Settings")]
    [SerializeField] private float timeBetween = 1f;
    [SerializeField] private int X = 12;
    [SerializeField] private int Y = 18;
    [SerializeField] private int Z = 24;
    [SerializeField] private int A = 30;
    private float timer;

    public int ChainCount { get; private set; }

    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        levelSystem = GetComponent<LevelSystem>();
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

    private void LevelSystem_OnEXPAdded(int addedAmount)
    {
        int expBoost = 0;

        if (ChainCount > 6 && ChainCount < X)
        {
            expBoost = Mathf.RoundToInt(addedAmount * 0.25f);
        }
        else if (ChainCount >= X && ChainCount < Y)
        {
            expBoost = Mathf.RoundToInt(addedAmount * 0.5f);
        }
        else if (ChainCount >= Y && ChainCount < Z)
        {
            expBoost = Mathf.RoundToInt(addedAmount * 0.75f);
        }
        else if (ChainCount >= Z && ChainCount < A)
        {
            expBoost = Mathf.RoundToInt(addedAmount * 2f);
        }

        if (expBoost > 0)
        {
            levelSystem.AddEXP(expBoost);
        }
    }
}

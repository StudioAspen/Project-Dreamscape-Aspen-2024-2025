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

    [SerializeField] private float firstBoost = 0.25f;
    [SerializeField] private float secondBoost = 0.5f;
    [SerializeField] private float thirdBoost = 0.75f;
    [SerializeField] private float fourthBoost = 2f;
    private float timer;

    public int ChainCount { get; private set; }

    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        levelSystem = GetComponent<LevelSystem>();

        levelSystem.OnEXPAdded += LevelSystem_OnEXPAdded;
    }

    private void OnDestroy()
    {
        levelSystem.OnEXPAdded -= LevelSystem_OnEXPAdded;
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
            expBoost = Mathf.RoundToInt(addedAmount * firstBoost);
        }
        else if (ChainCount >= X && ChainCount < Y)
        {
            expBoost = Mathf.RoundToInt(addedAmount * secondBoost);
        }
        else if (ChainCount >= Y && ChainCount < Z)
        {
            expBoost = Mathf.RoundToInt(addedAmount * thirdBoost);
        }
        else if (ChainCount >= Z && ChainCount < A)
        {
            expBoost = Mathf.RoundToInt(addedAmount * fourthBoost);
        }

        if (expBoost > 0)
        {
            levelSystem.AddEXP(expBoost);
        }
    }
}

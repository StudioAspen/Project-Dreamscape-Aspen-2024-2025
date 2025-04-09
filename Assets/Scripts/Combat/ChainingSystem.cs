using System;
using UnityEngine;

public class ChainingSystem : MonoBehaviour
{
    private Player player;
    private PlayerCombat playerCombat;

    [Header("Settings")]
    [SerializeField] private float baseTimeBetween = 5f;
    [SerializeField] private float timeBetweenMultiplier = 0.975f;
    private float timeBetween;
    private float timer;

    [Header("Rewards")]
    [SerializeField] private float percentDamageBonus;
    private float currentDamageBonus = 1;
    [SerializeField] private float maxDamageBonus;
    [SerializeField] private float percentMoveSpeedBonus;
    private float currentMoveSpeedBonus = 1;
    [SerializeField] private float maxMoveSpeedBonus;
    [SerializeField] private int healAmount;

    public int ChainCount { get; private set; }
    public Action<int> OnChainUpdated = delegate {};

    private void Awake()
    {
        player = GetComponent<Player>();
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

    // Chain increases by 1 every time the player lands a hit on an entity.
    private void PlayerWeapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue) => AddChain();


    // Chain resets to 0 if the player has not landed a hit within timeBetween seconds.
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

    private void AddChain()
    {
        ChainCount++;
        OnChainUpdated?.Invoke(ChainCount);
        timer = 0;
        timeBetween = timeBetween * timeBetweenMultiplier;

        if(ChainCount % 10 == 0)
        {
            //if ChainCount reaches mutliple of 10 you get healed.
            player.Heal(healAmount);
        }
        else if(ChainCount % 2 == 1)
        {
            //activates every odd increment of ChainCount (1,3,5..)
            if(currentDamageBonus < maxDamageBonus)
            {
                //if damage bonus isnt maxed out already, add percent bonus
                player.DamageModifier.RemoveMultiplier(currentDamageBonus, this);
                currentDamageBonus += percentDamageBonus;
                player.DamageModifier.AddMultiplier(currentDamageBonus, this);
            }
        }
        else
        {
            //activates every even increment (2,4,6..)
            if(currentMoveSpeedBonus < maxMoveSpeedBonus)
            {
                //if speed bonus hasnt maxed out add percent bonus
                player.StatusSpeedModifier.RemoveMultiplier(currentMoveSpeedBonus, this);
                currentMoveSpeedBonus += percentMoveSpeedBonus;
                player.StatusSpeedModifier.AddMultiplier(currentMoveSpeedBonus, this);
            }
        }
    }

    private void ResetChain()
    {
        timer = 0;
        timeBetween = baseTimeBetween;
        ChainCount = 0;
        OnChainUpdated?.Invoke(ChainCount);
        //resets modifiers yay
        player.StatusSpeedModifier.ClearBuffsFromSource(this);
        currentMoveSpeedBonus = 1;
        player.DamageModifier.ClearBuffsFromSource(this);
        currentDamageBonus = 1;
    }
}

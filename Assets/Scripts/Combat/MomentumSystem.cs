using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class MomentumSystem : MonoBehaviour
{
    private Player player;

    [Header("Settings")]
    [SerializeField] private float baseTimeBetween = 5f;
    [SerializeField] private float timeBetweenMultiplier = 0.975f;
    private float timer;
    private float timeBetween;

    [SerializeField] private float percentDamageBonus;
    private float currentDamageBonus = 1;
    [SerializeField] private float maxDamageBonus;
    [SerializeField] private float percentMoveSpeedBonus;
    private float currentMoveSpeedBonus = 1;
    [SerializeField] private float maxMoveSpeedBonus;
    [SerializeField] private int healAmount;

    private int momentum;
    public int Momentum => momentum;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        ResetMomentum();
    }

    private void OnEnable()
    {
        player.OnEntityTakeDamage += Player_OnEntityTakeDamage;
        player.OnKillEntity += Player_OnKillEntity;
    }

    private void OnDisable()
    {
        player.OnEntityTakeDamage -= Player_OnEntityTakeDamage;
        player.OnKillEntity -= Player_OnKillEntity;
    }

    void Update()
    {
        HandleMomentum();
    }

    private void Player_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        ResetMomentum();
    }

    private void Player_OnKillEntity(Entity victim)
    {
        AddMomentum();
    }

    private void HandleMomentum()
    {
        if (momentum > 0)
        {
            timer += Time.deltaTime;
        }
        if (timer > timeBetween)
        {
            ResetMomentum();
        }
    }

    private void AddMomentum()
    {
        momentum++;
        timer = 0;
        timeBetween = timeBetween * timeBetweenMultiplier;

        if(momentum % 10 == 0)
        {
            //if momentum reaches mutliple of 10 you get healed yay
            player.Heal(healAmount);
        }
        else if(momentum % 2 == 1)
        {
            //activates every odd increment of momentum (1,3,5..)
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

    private void ResetMomentum()
    {
        timer = 0;
        timeBetween = baseTimeBetween;
        momentum = 0;
        //resets modifiers yay
        player.StatusSpeedModifier.ClearBuffsFromSource(this);
        currentMoveSpeedBonus = 1;
        player.DamageModifier.ClearBuffsFromSource(this);
        currentDamageBonus = 1;
    }
}

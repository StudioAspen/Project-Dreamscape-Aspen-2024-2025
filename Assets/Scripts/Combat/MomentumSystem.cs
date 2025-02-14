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

    void Start()
    {
        Reset();
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
        Reset();
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
            Reset();
        }
    }

    private void AddMomentum()
    {
        momentum++;
        timer = 0;
        timeBetween = timeBetween * timeBetweenMultiplier;


        if(momentum % 10 == 0)
        {
            player.Heal(healAmount);
            Debug.Log("momentum heal");
        }
        else if(momentum % 2 == 1)
        {
            if(currentDamageBonus < maxDamageBonus)
            {
                player.SetDamageModifier(player.DamageModifier / currentDamageBonus);
                currentDamageBonus += percentDamageBonus;
                player.SetDamageModifier(player.DamageModifier * currentDamageBonus);
            }
            Debug.Log("damage mod = " + currentDamageBonus);
        }
        else
        {
            if(currentMoveSpeedBonus < maxMoveSpeedBonus)
            {
                player.SetSpeedModifier(player.SpeedModifier / currentMoveSpeedBonus);
                currentMoveSpeedBonus += percentMoveSpeedBonus;
                player.SetSpeedModifier(player.SpeedModifier * currentMoveSpeedBonus);
            }
            Debug.Log("speed mod = " + currentMoveSpeedBonus);
        }

    }

    private void Reset()
    {
        timer = 0;
        timeBetween = baseTimeBetween;
        momentum = 0;
        player.SetSpeedModifier(player.SpeedModifier / currentMoveSpeedBonus);
        currentMoveSpeedBonus = 1;
        player.SetDamageModifier(player.DamageModifier / currentDamageBonus);
        currentDamageBonus = 1;
    }

}

using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MomentumSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private Player player;

    [Header("Settings")]
    [SerializeField] private float baseTimeBetween = 5f;
    [SerializeField] private float timeBetweenMultiplier = 0.975f;
    private float timer;
    private float timeBetween;

    private int momentum;
    public int Momentum => momentum;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    void Start()
    {
        Reset();
    }

    private void OnEnable()
    {
        player.OnEntityTakeDamage.AddListener(Player_OnEntityTakeDamage);
        player.OnKillEntity.AddListener(Player_OnKillEntity);
    }

    private void OnDisable()
    {
        player.OnEntityTakeDamage.RemoveListener(Player_OnEntityTakeDamage);
        player.OnKillEntity.RemoveListener(Player_OnKillEntity);
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
    }

    private void Reset()
    {
        timer = 0;
        timeBetween = baseTimeBetween;
        momentum = 0;
    }

}

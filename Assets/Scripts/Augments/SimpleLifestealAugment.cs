using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLifestealAugment : Augment
{

    [field: Header("Augment Parameters")]
    [field: SerializeField] public float InitialPercent { get; private set; } = 0.0f;
    [field: SerializeField] public float MaxPercent { get; private set; } = 0.2f;
    [field: SerializeField] public float LifestealStep { get; private set; } = 0.05f;

    private Weapon weapon;
    private ChainingSystem chaining;


    protected void Awake()
    {
        base.Awake();
        weapon = GetComponent<AugmentManager>().Player.GetComponentInChildren<Weapon>();
        chaining = GetComponent<AugmentManager>().Player.GetComponentInChildren<ChainingSystem>();
    }


    protected override void Start()
    {
        base.Start();
    }


    protected void OnEnable()
    {
        base.OnEnable();
        weapon.OnWeaponHit.AddListener(Weapon_OnWeaponHit);
    }

    
    void Update()
    {
        
    }


    private void Weapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        if (source != player) { return; }
        float lifestealPercent = Mathf.Clamp(InitialPercent + (chaining.ChainCount - 1) * LifestealStep, InitialPercent, MaxPercent);
        int healValue = Mathf.RoundToInt(damageValue * lifestealPercent);
        if (healValue > 0) {
            player.Heal(Mathf.RoundToInt(damageValue * lifestealPercent));
        }
    }
}
 
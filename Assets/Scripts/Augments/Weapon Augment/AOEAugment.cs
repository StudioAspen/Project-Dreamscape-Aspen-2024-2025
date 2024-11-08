using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAugment : Augment
{

    [field: Header("Augment Parameters")]
    [field: SerializeField] public float aoeExplosionRadius { get; private set; } = 2.0f;
    [field: SerializeField] public float aoeDamage { get; private set; } = 0.2f;

    private Weapon weapon;
    private ChainingSystem chaining;
    private Entity entity;


    protected override void Awake()
    {
        base.Awake();
        Level = 1;

        // gets weapon script off player to apply aoe dmg
        weapon = GetComponent<AugmentManager>().Player.GetComponentInChildren<Weapon>();
        // to count every other hit to apply aoe
        chaining = GetComponent<AugmentManager>().Player.GetComponentInChildren<ChainingSystem>();

        SphereCollider sphereAOE = GetComponent<AugmentManager>().Player.AddComponent<SphereCollider>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        weapon.OnWeaponHit.AddListener(AOEHit);
    }

    void Update()
    {

    }

    private void AOEHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        // if not player dont aoe
        if (source != player) { return; }

        // make a list and grab all entities nearby
        List<Entity> enemyList = new List<Entity>();
        enemyList = entity.GetNearbyEntities(aoeExplosionRadius);

        Debug.Log("enemy hit");
    }
}

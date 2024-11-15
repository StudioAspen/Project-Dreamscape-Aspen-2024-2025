using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAugment : Augment
{

    [field: Header("Augment Parameters")]
    [field: SerializeField] public float aoeExplosionRadius { get; private set; } = 2.0f;
    [field: SerializeField] public int aoeDamage { get; private set; } = 2;

    private Weapon weapon;
    private ChainingSystem chaining;
    private Entity entity;


    protected override void Awake()
    {
        // uses the awake from augment base
        base.Awake();
        Level = 1;

        // gets weapon script off player to apply aoe dmg
        weapon = GetComponent<AugmentManager>().Player.GetComponentInChildren<Weapon>();
        // to count every other hit to apply aoe
        chaining = GetComponent<AugmentManager>().Player.GetComponentInChildren<ChainingSystem>();
    }

    protected override void Start()
    {
        // uses the start from augment base
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
        enemyList = victim.GetNearbyEntities(aoeExplosionRadius);
        for(int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].TakeDamage(aoeDamage, hitPoint, null);
        }

        // creates a sphere of the explosion radius
        GameObject wqe = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        wqe.transform.position = hitPoint;
        wqe.GetComponent<Collider>().isTrigger = true;
        wqe.transform.localScale = aoeExplosionRadius * Vector3.one;
        Destroy(wqe, 1);

        Debug.Log("enemy hit");
    }
}

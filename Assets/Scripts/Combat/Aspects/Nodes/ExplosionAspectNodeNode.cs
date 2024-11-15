using System.Collections.Generic;
using UnityEngine;

// on hit, creates a explosion that damages those in it
public class ExplosionAspectNodeNode : AspectNodeNode
{
    [field: Header("Augment Parameters")]
    [field: SerializeField] public float aoeExplosionRadius { get; private set; } = 2.0f;
    [field: SerializeField] public int aoeDamage { get; private set; } = 2;

    private Entity entity;

    public override void ApplyAspect(AspectsManager aspectsManager)
    {
        base.ApplyAspect(aspectsManager);

        Weapon ownerWeapon = aspectsManager.GetComponentInChildren<Weapon>();
        ChainingSystem chainingSystem = aspectsManager.GetComponentInChildren<ChainingSystem>();

        ownerWeapon.OnWeaponHit.AddListener(AOEHit);
    }

    private void AOEHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        // make a list and grab all entities nearby
        List<Entity> enemyList = new List<Entity>();
        enemyList = victim.GetNearbyEntities(aoeExplosionRadius);
        for (int i = 0; i < enemyList.Count; i++)
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

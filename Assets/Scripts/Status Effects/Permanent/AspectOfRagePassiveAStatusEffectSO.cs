using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive A")]
public class AspectOfRagePassiveAStatusEffectSO : StatusEffectSO
{
    private Weapon ownerWeapon;

    [field: Header("Aspect of Rage Passive A: Settings")]
    [field: SerializeField] public float AOEExplosionRadius { get; private set; } = 2.0f;
    [field: SerializeField] public int AOEDamage { get; private set; } = 2;
    [field: SerializeField] public StatusEffectSO BurningRageStack { get; private set; }

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        ownerWeapon = entity.GetComponentInChildren<Weapon>();
        if(ownerWeapon == null)
        {
            Debug.LogError($"{name}: Weapon not found on entity: {entity.name}");
            return;
        }

        //ownerWeapon.OnWeaponHit.AddListener(WeaponExplosion_OnWeaponHit);
        ownerWeapon.OnWeaponHit.AddListener(WeaponStacks_OnWeaponHit);
    }

    public override void Cancel()
    {
        base.Cancel();

        //ownerWeapon.OnWeaponHit.RemoveListener(WeaponExplosion_OnWeaponHit);
        ownerWeapon.OnWeaponHit.RemoveListener(WeaponStacks_OnWeaponHit);
    }

    public override bool Override(StatusEffectSO newStatusEffect)
    {
        if (!base.Override(newStatusEffect)) return false;

        // add expansion logic here when stacked

        return true;
    }

    // for explosion
    private void WeaponExplosion_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        // make a list and grab all entities nearby
        List<Entity> enemyList = Entity.GetEntitiesThroughAOE(hitPoint, AOEExplosionRadius);
        for (int i = 0; i < enemyList.Count; i++) // loop through all entities and filter out friendly ones
        {
            Entity enemy = enemyList[i]; // current entity in the loop

            if (enemy.Team == source.Team) continue; // filter out friendly entities

            enemy.TakeDamage(AOEDamage, enemy.GetComponent<Collider>().ClosestPointOnBounds(hitPoint), source.gameObject); // deal damage to enemy entities
        }

        CreateTemporaryVisualizer(hitPoint, AOEExplosionRadius, 0.25f);
    }

    // for stacks
    private void WeaponStacks_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        
        EntityStatusEffector statusEffector = victim.GetComponent<EntityStatusEffector>();
        if (statusEffector == null) { return; }
        statusEffector.ApplyStatusEffect(BurningRageStack, victim.gameObject);
    }

    private void CreateTemporaryVisualizer(Vector3 hitPoint, float radius, float expireDuration)
    {
        // creates a sphere of the explosion radius
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = hitPoint;
        sphere.GetComponent<Collider>().isTrigger = true;
        sphere.transform.localScale = radius * Vector3.one;
        SetTransparent(sphere.GetComponent<Renderer>().material);
        sphere.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        sphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.2f);
        Destroy(sphere, expireDuration);

        Debug.Log("enemy hit");
    }

    private void SetTransparent(Material targetMaterial)
    {
        if (targetMaterial == null) return;

        targetMaterial.shader = Shader.Find("Universal Render Pipeline/Unlit");

        // Change Surface Type to Transparent
        targetMaterial.SetFloat("_Surface", 1); // 1 = Transparent, 0 = Opaque

        // Enable required shader keywords
        targetMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        targetMaterial.DisableKeyword("_SURFACE_TYPE_OPAQUE");

        // Set rendering mode for transparency
        targetMaterial.SetOverrideTag("RenderType", "Transparent");
        targetMaterial.SetInt("_ZWrite", 0); // Disable ZWrite for transparency
        targetMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        targetMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        // Apply the changes to the material
        targetMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}

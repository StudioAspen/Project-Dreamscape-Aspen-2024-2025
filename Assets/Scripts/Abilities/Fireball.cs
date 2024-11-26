using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Fireball : MonoBehaviour, IPoolableObject
{
    [Header("References")]
    [SerializeField, Self] private Rigidbody rigidBody;

    [Header("Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float aoeRadius = 5f;

    private Vector3 direction;
    private GameObject caster;
    private int team;
    private float damagePercent;
    private Vector2Int damageRange;

    private ObjectPool<GameObject> pool;

    /* spawning a fireball from another script
            ObjectPooler spawner = GameObject.Find("AbilitiesPooler").GetComponent<ObjectPooler>();
            if (spawner == null) return;

            spawner.ChangePrefab(FireballPrefab.gameObject);

            Fireball fireball = spawner.SpawnObject<Fireball>(GetColliderCenterPosition());
            fireball.Fire(transform.forward, gameObject, Team, 100f);
     * */

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
        caster = null;
        team = 0;
        damagePercent = 0f;
        damageRange = Vector2Int.zero;
    }

    private void OnDisable()
    {
        // logic for when the fireball gets released to the pool
    }

    /// <summary>
    /// Fires a fireball in the specified direction.
    /// </summary>
    /// <param name="direction">The direction in which to fire the fireball.</param>
    /// <param name="caster">The GameObject that casted the fireball.</param>
    /// <param name="team">The team of the fireball.</param>
    /// <param name="damagePercent">The percentage of damage to apply to the entities hit by the fireball.</param>
    public void Fire(Vector3 direction, GameObject caster, int team, float damagePercent)
    {
        this.direction = direction;
        this.caster = caster;
        this.team = team;
        this.damagePercent = damagePercent;

        StartCoroutine(FireballMove());
    }

    /// <summary>
    /// Fires a fireball in the specified direction.
    /// You can also specify a damage range to calculate the damage for entities hit by the fireball.
    /// </summary>
    /// <param name="direction">The direction in which to fire the fireball.</param>
    /// <param name="caster">The GameObject that casted the fireball.</param>
    /// <param name="team">The team of the fireball.</param>
    /// <param name="damagePercent">The percentage of damage to apply to the entities hit by the fireball.</param>
    /// <param name="damageRange">The damage range to calculate the damage for entities hit by the fireball.</param>
    public void Fire(Vector3 direction, GameObject caster, int team, float damagePercent, Vector2Int damageRange)
    {
        this.direction = direction;
        this.caster = caster;
        this.team = team;
        this.damagePercent = damagePercent;
        this.damageRange = damageRange;

        StartCoroutine(FireballMove());
    }

    private IEnumerator FireballMove()
    {
        float distanceTraveled = 0f;

        while (distanceTraveled < maxDistance)
        {
            Vector3 moveDistanceThisFrame = speed * Time.deltaTime * direction.normalized;

            transform.Translate(moveDistanceThisFrame);

            distanceTraveled += moveDistanceThisFrame.magnitude;

            yield return null;
        }

        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity enemy))
        {
            if (enemy.CurrentState == enemy.EntityDeathState) return; // if theyre already dying
            if (enemy.Team == team) return; // if theyre on the same team

            Explode();
        }
    }

    private void Explode()
    {
        List<Entity> entitiesHit = Entity.GetEntitiesThroughAOE(transform.position, aoeRadius);

        foreach (Entity entity in entitiesHit)
        {
            if (entity.Team != team) 
            {
                int damage = 0;

                // if caster is an entity, calculate damage based on entity stats
                if(caster.TryGetComponent(out Entity casterEntity)) damage = casterEntity.CalculateDamage(damagePercent);
                // otherwise 
                else damage = Random.Range(damageRange.x, damageRange.y);

                entity.TakeDamage(damage, transform.position, caster);
            }
        }

        //insert explosion vfx here:
        CreateTemporaryVisualizer(transform.position, aoeRadius, 0.25f);

        if (pool == null)
        {
            Destroy(gameObject);
            return;
        }

        pool.Release(gameObject);
    }

    void OnDrawGizmos()
    {
        //Visualize AOE radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
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

    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }
}

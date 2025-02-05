using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.VFX;

public class Geyser : MonoBehaviour
{
    private CapsuleCollider capsuleCollider;

    [SerializeField] private VisualEffect geyserVFX;

    [Header("Config")]
    [SerializeField] private Vector2 eruptIntervalRange = new Vector2(3f, 15f);
    [SerializeField] private Vector2 eruptDurationRange = new Vector2(3f, 6f);
    [SerializeField] private Vector2 eruptHeightRange = new Vector2(3, 5);
    [SerializeField] private Vector2Int eruptDamageRange = new Vector2Int(10, 15);
    [SerializeField] private float damageTickDuration = 1f;

    private Dictionary<Entity, float> geyseredEntities = new();

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        IgnoreOtherEntityCollisions(true);
    }

    private void Start()
    {
        geyserVFX.Stop();

        StartCoroutine(PeriodicEruptCoroutine());
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        capsuleCollider = GetComponent<CapsuleCollider>();

        Gizmos.color = Color.green;
        CustomDebug.DrawWireCapsule(transform.position + capsuleCollider.radius * Vector3.up, transform.position + (eruptHeightRange.x - capsuleCollider.radius) * Vector3.up, capsuleCollider.radius);
        Gizmos.color = Color.red;
        CustomDebug.DrawWireCapsule(transform.position + capsuleCollider.radius * Vector3.up, transform.position + (eruptHeightRange.y - capsuleCollider.radius) * Vector3.up, capsuleCollider.radius);
#endif
    }

    private void Update()
    {
        foreach (Entity entity in geyseredEntities.Keys.ToList())
        {
            if (geyseredEntities.TryGetValue(entity, out float remainingTime))
            {
                geyseredEntities[entity] -= Time.deltaTime;
                if (geyseredEntities[entity] <= 0)
                {
                    int damage = Random.Range(eruptDamageRange.x, eruptDamageRange.y);
                    entity.TakeDamage(damage, entity.GetColliderCenterPosition(), gameObject);

                    geyseredEntities[entity] = damageTickDuration;
                } 
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Entity hitEntity = other.gameObject.GetComponent<Entity>();
        if (hitEntity == null) hitEntity = other.gameObject.GetComponentInParent<Entity>();
        if (hitEntity == null) return;

        if (!geyseredEntities.ContainsKey(hitEntity))
        {
            geyseredEntities.Add(hitEntity, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Entity hitEntity = other.gameObject.GetComponent<Entity>();
        if (hitEntity == null) hitEntity = other.gameObject.GetComponentInParent<Entity>();
        if (hitEntity == null) return;

        if (geyseredEntities.ContainsKey(hitEntity))
        {
            geyseredEntities.Remove(hitEntity);
        }
    }

    private IEnumerator PeriodicEruptCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(eruptIntervalRange.x, eruptIntervalRange.y));

            geyserVFX.Play();

            float currentEruptHeight = Random.Range(eruptHeightRange.x, eruptHeightRange.y);
            UpdateCapsuleColliderHitbox(currentEruptHeight);
            geyserVFX.transform.localScale = new Vector3(1, currentEruptHeight, 1);

            IgnoreOtherEntityCollisions(false);

            yield return new WaitForSeconds(Random.Range(eruptDurationRange.x, eruptDurationRange.y));

            geyserVFX.Stop();

            IgnoreOtherEntityCollisions(true);

            geyseredEntities.Clear();
        }
    }

    /// <summary>
    /// Changes the size of the capsule collider to match the height.
    /// </summary>
    /// <param name="height">The height of the desired hitbox.</param>
    private void UpdateCapsuleColliderHitbox(float height)
    {
        capsuleCollider.height = height;
        capsuleCollider.center = new Vector3(0, height / 2, 0);
    }

    /// <summary>
    /// Ignores or includes collisions with other entities.
    /// </summary>
    /// <param name="willIgnore">True to ignore collisions with other entities, false to include collisions.</param>
    public void IgnoreOtherEntityCollisions(bool willIgnore = true)
    {
        capsuleCollider.excludeLayers = willIgnore ? LayerMask.GetMask("Entity", "Damageable Entity") : 0;
    }
}

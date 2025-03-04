using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class GeyserEruptingState : GeyserBaseState
{
    private CapsuleCollider capsuleCollider;

    [field: Header("Config")]
    [field: SerializeField] public VisualEffect GeyserVFX { get; private set; }
    [field: SerializeField] public Vector2 DurationRange { get; private set; } = new Vector2(3f, 6f);
    [field: SerializeField] public Vector2 HeightRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2Int DamageRange { get; private set; } = new Vector2Int(10, 15);
    [field: SerializeField] public float DamageTickDuration { get; private set; } = 1f;

    private Dictionary<Entity, float> geyseredEntities = new();

    private float timer;
    private float randomDuration;
    private float randomHeight;

    private protected override void Init()
    {
        base.Init();
        capsuleCollider = GetComponent<CapsuleCollider>();
        GeyserVFX.Stop();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        capsuleCollider = GetComponent<CapsuleCollider>();

        Gizmos.color = Color.green;
        CustomDebug.DrawWireCapsule(transform.position + capsuleCollider.radius * Vector3.up, transform.position + (HeightRange.x - capsuleCollider.radius) * Vector3.up, capsuleCollider.radius);
        Gizmos.color = Color.red;
        CustomDebug.DrawWireCapsule(transform.position + capsuleCollider.radius * Vector3.up, transform.position + (HeightRange.y - capsuleCollider.radius) * Vector3.up, capsuleCollider.radius);
#endif
    }

    public override void OnEnter()
    {
        timer = 0f;

        randomDuration = Random.Range(DurationRange.x, DurationRange.y);
        randomHeight = Random.Range(HeightRange.x, HeightRange.y);
    
        UpdateCapsuleColliderHitbox(randomHeight);
        GeyserVFX.transform.localScale = new Vector3(1, randomHeight, 1);

        GeyserVFX.Play();
    }

    public override void OnExit()
    {
        GeyserVFX.Stop();
        geyseredEntities.Clear();
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > randomDuration)
        {
            geyser.ChangeState(geyser.GeyserCooldownState);
            return;
        }

        foreach (Entity entity in geyseredEntities.Keys.ToList())
        {
            if (geyseredEntities.TryGetValue(entity, out float remainingTime))
            {
                if (geyseredEntities[entity] <= 0)
                {
                    int damage = Random.Range(DamageRange.x, DamageRange.y);
                    entity.TakeDamage(damage, entity.GetColliderCenterPosition(), gameObject);

                    geyseredEntities[entity] = DamageTickDuration;
                }
                else
                {
                    geyseredEntities[entity] -= Time.deltaTime;
                }
            }
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

    public override void OnOnTriggerStay(Collider other)
    {
        Entity hitEntity = other.gameObject.GetComponent<Entity>();
        if (hitEntity == null) return;

        if (!geyseredEntities.ContainsKey(hitEntity))
        {
            geyseredEntities.Add(hitEntity, 0);
        }
    }

    public override void OnOnTriggerExit(Collider other)
    {
        Entity hitEntity = other.gameObject.GetComponent<Entity>();
        if (hitEntity == null) return;

        if (geyseredEntities.ContainsKey(hitEntity))
        {
            geyseredEntities.Remove(hitEntity);
        }
    }
}

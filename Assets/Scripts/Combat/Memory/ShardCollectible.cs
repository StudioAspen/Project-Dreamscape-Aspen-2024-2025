using DG.Tweening;
using System;
using UnityEngine;

public class ShardCollectible : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float bobAmplitude = 0.5f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private MeshRenderer meshRenderer;
    private Color shardColor = Color.blue;
    private Type entityType;
    private PlayerAbilityStateSO memoryAbility;
    private int shardCount;

    private bool isCollectible = true;

    /// <summary>
    /// Initializes the shard to store the entity type, color, and ability.
    /// </summary>
    public void Init(Type entityType, Color color, PlayerAbilityStateSO memoryAbility, int count)
    {
        this.entityType = entityType;
        this.shardColor = color;
        this.memoryAbility = memoryAbility;
        this.shardCount = count;
    }

    private void Start()
    {
        meshRenderer.material.SetColor("_main", shardColor);
        meshRenderer.material.SetColor("_highlight", shardColor);
        meshRenderer.material.SetColor("_Shine", shardColor);

        transform.localScale = (shardCount/10f) * Vector3.one;

        transform.DOMoveY(transform.position.y + bobAmplitude, bobSpeed).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCollectible) return;

        if(other.gameObject.TryGetComponent(out MemorySystem memorySystem))
        {
            memorySystem.AddShards(entityType, shardCount, shardColor, memoryAbility);
            isCollectible = false;

            PlayDestroyAnimation();
        }
    }

    /// <summary>
    /// Plays the destroy animation of the shard and then destroys itself
    /// </summary>
    private void PlayDestroyAnimation()
    {
        // Insert destroy animation logic here

        Destroy(gameObject);
    }
}

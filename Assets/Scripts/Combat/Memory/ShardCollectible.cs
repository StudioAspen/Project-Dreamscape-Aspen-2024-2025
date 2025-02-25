using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

public class ShardCollectible : MonoBehaviour
{
    private Player player;

    [Header("Config")]
    [SerializeField] private float moveSpeed = 10f;
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
        // Gets the closest player
        player = FindObjectsByType<Player>(FindObjectsSortMode.None).OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).ToList()[0];

        meshRenderer.material.SetColor("_main", shardColor);
        meshRenderer.material.SetColor("_highlight", shardColor);
        meshRenderer.material.SetColor("_Shine", shardColor);

        transform.localScale = (shardCount / 20f) * Vector3.one;
    }

    private void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(transform.position, player.GetColliderCenterPosition(), moveSpeed * Time.deltaTime);
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

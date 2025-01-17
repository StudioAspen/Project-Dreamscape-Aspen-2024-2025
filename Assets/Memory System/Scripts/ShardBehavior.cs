using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardBehavior : MonoBehaviour
{

    public Shard shard;
    public float moveSpeed = 5f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, player.position) < 0.5f)
            {
                MemorySystemInterface.Instance.AddShard(shard);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Notify the MemorySystem to add this shard
            MemorySystemInterface memorySystem = other.GetComponent<MemorySystemInterface>();
            if (memorySystem != null)
            {
                memorySystem.AddShard(shard); // Update the shard count and UI
            }

            // Destroy the shard object
            Destroy(gameObject);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardBehavior : MonoBehaviour
{
    public Shard shard; // Reference to the shard data
    public float moveSpeed = 5f; // Speed at which the shard moves towards the player

    private Transform player; // Reference to the player's transform

    void Start()
    {
        // Find the player object by tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Move the shard towards the player
        if (player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
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
                memorySystem.CollectCrystal(shard); // Update the shard count and UI
            }

            // Destroy the shard object
            Destroy(gameObject);
        }
    }
}


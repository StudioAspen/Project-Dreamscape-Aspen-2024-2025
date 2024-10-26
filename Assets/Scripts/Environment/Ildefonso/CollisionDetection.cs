using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player stepped on an island
        if (other.CompareTag("Island"))
        {
            IslandManager islandManager = other.GetComponent<IslandManager>();
            if (islandManager != null)
            {
                Debug.Log("Player stepped on an island at position: " + islandManager.GridPosition);
                // Additional logic when the player steps on an island
                islandManager.IsVisited = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MemoryBar : MonoBehaviour
{
    public GameObject segmentPrefab; // Prefab for bar segments
    public int maxShardsPerLevel = 10; // Shards needed to fill a level

    private int currentLevel = 0;
    private int totalShards = 0;

    public void UpdateBar(Dictionary<Shard, int> shardCounts)
    {
        // Clear existing segments
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Rebuild the bar based on shard counts
        float totalFill = 0f;

        foreach (var entry in shardCounts)
        {
            Shard shard = entry.Key;
            int count = entry.Value;

            float fillAmount = (float)count / maxShardsPerLevel;

            // Create a new segment for this shard
            GameObject newSegment = Instantiate(segmentPrefab, transform);
            Image segmentImage = newSegment.GetComponent<Image>();
            segmentImage.color = shard.color;
            segmentImage.fillAmount = fillAmount;

            // Adjust segment position for circular layout
            newSegment.transform.Rotate(Vector3.forward, totalFill * 360);

            totalFill += fillAmount;
        }
    }

    public bool IsBarFull()
    {
        return totalShards >= maxShardsPerLevel * (currentLevel + 1);
    }

    public void LevelUp()
    {
        currentLevel++;
        Debug.Log($"Bar leveled up! Current level: {currentLevel}");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MemoryBar : MonoBehaviour
{
    public static MemoryBar Instance;

    public RectTransform barContainer; // Parent object for the bar segments
    public GameObject barSegmentPrefab; // Prefab for individual bar segments
    public Color neutralColor = Color.gray; // Neutral color for the empty portion of the bar

    private List<Image> barSegments = new List<Image>(); // List to track bar segments

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize the bar with the neutral color
        InitializeMemoryBar();
    }

    private void InitializeMemoryBar()
    {
        // Create a single segment representing the neutral color
        CreateBarSegment(1f, neutralColor);
    }

    public void UpdateMemoryBar(Dictionary<string, int> shardCounts, Dictionary<string, Color> shardColors, string dominantShardType, int currentLevel)
    {
        Debug.Log("Updating Memory Bar...");

        // Check for null references
        if (shardCounts == null)
        {
            Debug.LogError("shardCounts is null!");
            return;
        }
        if (shardColors == null)
        {
            Debug.LogError("shardColors is null!");
            return;
        }
        if (barSegmentPrefab == null)
        {
            Debug.LogError("barSegmentPrefab is null!");
            return;
        }
        if (barContainer == null)
        {
            Debug.LogError("barContainer is null!");
            return;
        }

        // Clear existing bar segments (except the neutral one)
        for (int i = barSegments.Count - 1; i >= 0; i--)
        {
            if (barSegments[i].color != neutralColor) // Only destroy non-neutral segments
            {
                Destroy(barSegments[i].gameObject);
                barSegments.RemoveAt(i);
            }
        }

        // Calculate total shards collected
        int totalShards = 0;
        foreach (var count in shardCounts.Values)
        {
            totalShards += count;
        }

        Debug.Log($"Total Shards Collected: {totalShards}");

        // Calculate the proportion of the bar that should be filled
        int levelThreshold = MemorySystemInterface.Instance.levelThresholds[currentLevel - 1];
        float filledProportion = (float)totalShards / levelThreshold;

        Debug.Log($"Filled Proportion: {filledProportion}");

        // Create segments for each shard type
        float offset = 0f; // Track the starting position of each segment
        foreach (var pair in shardCounts)
        {
            string shardType = pair.Key;
            int shardCount = pair.Value;

            // Calculate the proportion of this shard type
            float proportion = (float)shardCount / levelThreshold;

            Debug.Log($"Creating Segment: Type = {shardType}, Proportion = {proportion}, Color = {shardColors[shardType]}");

            // Create a new bar segment
            CreateBarSegment(proportion, shardColors[shardType], offset);

            // Update the offset for the next segment
            offset += proportion;
        }

        // Add the neutral color for the remaining portion of the bar
        if (filledProportion < 1f)
        {
            Debug.Log($"Creating Neutral Segment: Proportion = {1f - filledProportion}, Color = {neutralColor}");
            CreateBarSegment(1f - filledProportion, neutralColor, offset);
        }
    }

    private void CreateBarSegment(float proportion, Color color, float offset = 0f)
    {
        Debug.Log($"Creating Bar Segment: Proportion = {proportion}, Color = {color}, Offset = {offset}");

        // Create a new bar segment
        GameObject segment = Instantiate(barSegmentPrefab, barContainer);
        Image segmentImage = segment.GetComponent<Image>();

        // Set the segment's color
        segmentImage.color = color;

        // Set the segment's size and position
        RectTransform segmentRect = segment.GetComponent<RectTransform>();
        segmentRect.anchorMin = new Vector2(offset, 0f);
        segmentRect.anchorMax = new Vector2(offset + proportion, 1f);
        segmentRect.offsetMin = Vector2.zero;
        segmentRect.offsetMax = Vector2.zero;

        // Add the segment to the list
        barSegments.Add(segmentImage);
    }
}

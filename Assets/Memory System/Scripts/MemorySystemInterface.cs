using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MemorySystemInterface : MonoBehaviour
{
    public static MemorySystemInterface Instance;

    public int[] levelThresholds = { 10, 15, 20 }; // Thresholds for each level
    public int currentLevel = 0;

    // Dictionary to track crystal counts and colors
    public Dictionary<string, int> crystalCounts = new Dictionary<string, int>();
    public Dictionary<string, Color> crystalColors = new Dictionary<string, Color>();

    public string dominantCrystalType; // Tracks the most collected crystal type

    public void CollectCrystal(Shard crystal)
    {
        string type = crystal.GetShardType();
        Color color = crystal.GetShardColor();

        // Update crystal count
        if (crystalCounts.ContainsKey(type))
        {
            crystalCounts[type]++;
        }
        else
        {
            crystalCounts[type] = 1;
            crystalColors[type] = color; // Store the color if it's a new crystal type
        }

        LogShardCounts();

        // Update the UI bar
        if (MemoryBar.Instance != null)
        {
            MemoryBar.Instance.UpdateMemoryBar(crystalCounts, crystalColors, dominantCrystalType, currentLevel);
        }
        else
        {
            Debug.LogError("MemoryBar.Instance is null!");
        }

        // Debug: Log the collected shard and current counts
        Debug.Log($"Collected Shard: Type = {type}, Color = {color}");

        // Update the UI bar
        if (MemoryBar.Instance != null)
        {
            MemoryBar.Instance.UpdateMemoryBar(crystalCounts, crystalColors, dominantCrystalType, currentLevel);
        }
        else
        {
            Debug.LogError("MemoryBar.Instance is null!");
        }

        // Check if the player has reached the next level threshold
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int totalCrystals = GetTotalCrystals();

        if (currentLevel < levelThresholds.Length && totalCrystals >= levelThresholds[currentLevel])
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        // Determine the dominant crystal type
        dominantCrystalType = GetDominantCrystalType();

        // Debug: Log the new level and dominant shard type
        Debug.Log($"Level Up! New Level = {currentLevel + 1}, Dominant Shard = {dominantCrystalType}");

        // Grant the player the ability for the dominant crystal type
        GrantAbility(dominantCrystalType, currentLevel + 1);

        // Move to the next level
        currentLevel++;

        // Update the UI bar
        MemoryBar.Instance.UpdateMemoryBar(crystalCounts, crystalColors, dominantCrystalType, currentLevel);
    }

    private string GetDominantCrystalType()
    {
        string dominantType = "";
        int maxCount = 0;

        foreach (var pair in crystalCounts)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                dominantType = pair.Key;
            }
        }

        return dominantType;
    }

    private int GetTotalCrystals()
    {
        int total = 0;
        foreach (var count in crystalCounts.Values)
        {
            total += count;
        }
        return total;
    }

    private void GrantAbility(string type, int level)
    {
        // Implement logic to grant the player the appropriate ability
        Debug.Log($"Granted {type} Level {level} Ability!");
    }

    private void LogShardCounts()
    {
        // Debug: Log the current counts of all shards
        string logMessage = "Shard Counts:\n";
        foreach (var pair in crystalCounts)
        {
            logMessage += $"{pair.Key}: {pair.Value}\n";
        }
        Debug.Log(logMessage);
    }
}

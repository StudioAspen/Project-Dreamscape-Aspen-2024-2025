using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MemorySystemInterface : MonoBehaviour
{
    public MemoryBar memoryBar; // Reference to the memory bar UI
    public int maxShardsPerLevel = 10; // Shards needed to fill a level

    private Dictionary<Shard, int> shardCounts = new Dictionary<Shard, int>();
    private int barLevel = 0;

    public void AddShard(Shard shard)
    {
        if (shardCounts.ContainsKey(shard))
        {
            shardCounts[shard]++;
        }
        else
        {
            shardCounts[shard] = 1;
        }

        // Update the memory bar
        memoryBar.UpdateBar(shardCounts);

        // Check if the bar is full and level up
        if (memoryBar.IsBarFull())
        {
            LevelUpBar();
        }
    }

    private void LevelUpBar()
    {
        memoryBar.LevelUp();
        barLevel++;
        Debug.Log("Memory bar leveled up!");
    }


    public int GetBarLevel()
    {
        return barLevel;
    }

    public Shard GetDominantShard()
    {
        return shardCounts.OrderByDescending(pair => pair.Value).FirstOrDefault().Key;
    }

    public void ActivateAbility()
    {
        Shard dominantShard = GetDominantShard();
        if (dominantShard != null)
        {
            var ability = dominantShard.GetAbility(barLevel);
            ability?.skillAction.Invoke();
            Debug.Log($"Activated {dominantShard.shardName} Level {barLevel} Ability!");
        }
    }

    private int GetTotalShards()
    {
        return shardCounts.Values.Sum();
    }

    private int GetThresholdForNextLevel()
    {
        return (barLevel + 1) * 10; // Example: Level 1 requires 10 shards, Level 2 requires 20 shards, etc.
    }
}

using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemorySystem : MonoBehaviour
{
    private Player player;

    [Serializable]
    public class ShardDictionaryData
    {
        public int Count;
        public Color Color;
        public PlayerAbilityStateSO MemoryAbility;

        public ShardDictionaryData(int count, Color color, PlayerAbilityStateSO memoryAbility)
        {
            Count = count;
            Color = color;
            MemoryAbility = memoryAbility;
        }
    }

    [field: SerializeField] public SerializedDictionary<string, ShardDictionaryData> ShardDictionary { get; private set; } = new();

    [field: Header("Config")]
    [field: SerializeField] public int MaxShardsPerLevel { get; private set; } = 100; // Shards needed to fill a level

    /// <summary>
    /// Action that is invoked when a new shard type is added.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>string shardHolderType</c>: The shard holder type that was added</description></item>
    /// </list>
    /// </remarks>
    public Action<string> OnNewShardTypeAdded = delegate { };
    /// <summary>
    /// Action that is invoked when a shard is added.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>string shardHolderType</c>: The shard holder type that was had it's shards added</description></item>
    /// </list>
    /// </remarks>
    public Action<string> OnShardAdded = delegate { };
    /// <summary>
    /// Action that is invoked when the memory bar reaches level 3 (MAX).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>string shardHolderType</c>: The current largest shard holder type.</description></item>
    /// </list>
    /// </remarks>
    public Action<string> OnMemoryBarFull = delegate { };
    /// <summary>
    /// Action that is invoked when the memory ability is activated.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>string shardHolderType</c>: The ability's shard holder type.</description></item>
    /// </list>
    /// </remarks>
    public Action<string> OnMemoryAbilityActivated = delegate { };

    private void Start()
    {
        player = GetComponent<Player>();
    }

    /// <summary>
    /// Tries to activate a memory ability based on the shard counts
    /// </summary>
    public void TryActivateMemoryAbility()
    {
        if (GetMemoryLevel() < 3)
        {
            Debug.Log("Memory meter is not full");
            return;
        }

        string largestShardType = GetLargestShardType();

        if(largestShardType == "")
        {
            Debug.LogWarning("Largest shard data null");
            return;
        }

        // Try to activate ability
        if(player.PlayerAbilityState.TryChangeAbilityState(ShardDictionary[largestShardType].MemoryAbility, false))
        {
            OnMemoryAbilityActivated.Invoke(largestShardType);
            ShardDictionary.Clear();
        }
    }

    /// <summary>
    /// Adds shards to the shards dictionary by performing checks.
    /// </summary>
    /// <param name="type">The type of the enemy to add.</param>
    /// <param name="count">The number of shards to add.</param>
    public void AddShards(Type type, int count, Color color, PlayerAbilityStateSO memoryAbility)
    {
        if(count < 0)
        {
            Debug.LogWarning($"Cant add negative shard count of {count}");
            return;
        }

        if(memoryAbility == null)
        {
            Debug.LogWarning($"Cant add {count} shards with null or invalid memory ability");
            return;
        }

        if (GetTotalShards() >= GetMaxShards())
        {
            Debug.Log("Memory bar full, can't add anymore shards!");
            return;
        }

        string typeName = type.ToString();

        if (ShardDictionary.ContainsKey(typeName))
        {
            ShardDictionary[typeName].Count += count;
            ShardDictionary[typeName].Color = color;
            ShardDictionary[typeName].MemoryAbility = memoryAbility;
        }
        else
        {
            ShardDictionary.Add(typeName, new ShardDictionaryData(count, color, memoryAbility));
            OnNewShardTypeAdded.Invoke(typeName);
        }

        int totalShards = GetTotalShards();
        int maxShards = GetMaxShards();

        // Subtract the overfill
        if (totalShards >= maxShards)
        {
            ShardDictionary[typeName].Count -= (totalShards - maxShards);
            OnMemoryBarFull.Invoke(GetLargestShardType());
        }

        OnShardAdded.Invoke(typeName);
    }

    /// <summary>
    /// Gets the shard type with the most shard count.
    /// If there is a tie, it gets the first one it finds.
    /// </summary>
    private string GetLargestShardType()
    {
        int largestCount = 0;
        string largestShardType = "";

        foreach(var shardEntry in ShardDictionary)
        {
            if(shardEntry.Value.Count > largestCount)
            {
                largestCount = shardEntry.Value.Count;
                largestShardType = shardEntry.Key;
            }
        }

        return largestShardType;
    }

    /// <summary>
    /// Gets the max shards the memory bar can hold.
    /// This is MaxShardsPerLevel * 3.
    /// </summary>
    public int GetMaxShards()
    {
        return MaxShardsPerLevel * 3;
    }

    /// <summary>
    /// Gets the current total number of shards in the memory bar.
    /// </summary>
    public int GetTotalShards()
    {
        int totalShards = 0;
        foreach (ShardDictionaryData shardData in ShardDictionary.Values)
        {
            totalShards += shardData.Count;
        }

        return totalShards;
    }

    /// <summary>
    /// Gets the current level of the memory bar.
    /// Level is calculated like this: (total shards)/(max shards per level).
    /// 3 is the highest level.
    /// </summary>
    public int GetMemoryLevel()
    {
        return GetTotalShards() / MaxShardsPerLevel;
    }
}

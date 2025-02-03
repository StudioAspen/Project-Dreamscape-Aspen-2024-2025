using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class MemorySystem : MonoBehaviour
{
    private Player player;

    [Serializable]
    public class ShardDictionaryData
    {
        public int Level;
        public int Count;
        public Color Color;
        public PlayerAbilityStateSO MemoryAbility;

        public ShardDictionaryData(int count, Color color, PlayerAbilityStateSO memoryAbility)
        {
            Level = 0;
            Count = count;
            Color = color;
            MemoryAbility = memoryAbility;
        }
    }

    [SerializeField] private SerializedDictionary<string, ShardDictionaryData> shardDictionary = new();

    [field: Header("Config")]
    [field: SerializeField] public int MaxShardsPerLevel { get; private set; } = 10; // Shards needed to fill a level

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnDestroy()
    {

    }

    /// <summary>
    /// Tries to activate a memory ability based on the shard counts
    /// </summary>
    public void TryActivateMemoryAbility()
    {
        string largestShardType = GetLargestShardType();

        if(largestShardType == "")
        {
            Debug.LogWarning("Largest shard data null");
            return;
        }

        player.PlayerAbilityState.ChangeAbilityState(shardDictionary[largestShardType].MemoryAbility, false);
        shardDictionary[largestShardType].Count = 0;
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

        string typeName = type.ToString();

        if (!shardDictionary.ContainsKey(typeName))
        {
            shardDictionary.Add(typeName, new ShardDictionaryData(count, color, memoryAbility));
            return;
        }

        shardDictionary[typeName].Count += count;
        shardDictionary[typeName].Color = color;
        shardDictionary[typeName].MemoryAbility = memoryAbility;
    }

    private string GetLargestShardType()
    {
        int largestCount = 0;
        string largestShardType = "";

        foreach(var shardEntry in shardDictionary)
        {
            if(shardEntry.Value.Count > largestCount)
            {
                largestCount = shardEntry.Value.Count;
                largestShardType = shardEntry.Key;
            }
        }

        return largestShardType;
    }
}

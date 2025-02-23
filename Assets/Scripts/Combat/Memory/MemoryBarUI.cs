using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MemoryBarUI : MonoBehaviour
{
    private MemorySystem memorySystem;

    [Header("UI")]
    [SerializeField] private RectTransform memoryBarTransform;
    private Dictionary<string, RectTransform> shardBarTransforms = new();

    private void Awake()
    {
        Player.OnPlayerInstantiated += Player_OnPlayerInstantiated;
    }

    private void OnDestroy()
    {
        Player.OnPlayerInstantiated -= Player_OnPlayerInstantiated;

        if(memorySystem != null)
        {
            memorySystem.OnNewShardTypeAdded -= MemorySystem_OnNewShardTypeAdded;
            memorySystem.OnShardAdded -= MemorySystem_OnShardAdded;
            memorySystem.OnLevelOneReached -= MemorySystem_OnLevelOneReached;
            memorySystem.OnLevelTwoReached -= MemorySystem_OnLevelTwoReached;
            memorySystem.OnMemoryBarFull -= MemorySystem_OnMemoryBarFull;
            memorySystem.OnMemoryAbilityActivated -= MemorySystem_OnMemoryAbilityActivated;
        }
    }

    private void Player_OnPlayerInstantiated(Player player)
    {
        Player.OnPlayerInstantiated -= Player_OnPlayerInstantiated;

        memorySystem = player.GetComponent<MemorySystem>();

        if (memorySystem == null) return;
        //Debug.Log("Found player memory system!");

        memorySystem.OnNewShardTypeAdded += MemorySystem_OnNewShardTypeAdded;
        memorySystem.OnShardAdded += MemorySystem_OnShardAdded;
        memorySystem.OnLevelOneReached += MemorySystem_OnLevelOneReached;
        memorySystem.OnLevelTwoReached += MemorySystem_OnLevelTwoReached;
        memorySystem.OnMemoryBarFull += MemorySystem_OnMemoryBarFull;
        memorySystem.OnMemoryAbilityActivated += MemorySystem_OnMemoryAbilityActivated;
    }

    private void MemorySystem_OnNewShardTypeAdded(string shardHolderType)
    {
        shardBarTransforms.Add(shardHolderType, CreateNewShardBar(shardHolderType));
    }

    private void MemorySystem_OnShardAdded(string shardHolderType)
    {
        
    }

    private void MemorySystem_OnLevelOneReached(string largestShardHolderType)
    {
        //Debug.Log($"Memory bar is level 1 now with {largestShardHolderType} as the dominant shard");
    }

    private void MemorySystem_OnLevelTwoReached(string largestShardHolderType)
    {
        //Debug.Log($"Memory bar is level 2 now with {largestShardHolderType} as the dominant shard");
    }

    private void MemorySystem_OnMemoryBarFull(string largestShardHolderType)
    {
       //Debug.Log($"Memory bar full, changing all bar colors to {memorySystem.ShardDictionary[largestShardHolderType].Color}");

        // Change all shard bar colors to the largest holder's shard color
        foreach (RectTransform shardBarTransform in shardBarTransforms.Values)
        {
            shardBarTransform.GetComponent<Image>().color = memorySystem.ShardDictionary[largestShardHolderType].Color;
        }
    }

    private void MemorySystem_OnMemoryAbilityActivated(string activatedShardHolderType)
    {
        //Debug.Log($"Memory ability for {activatedShardHolderType} activated, removing all shard bars");

        foreach (RectTransform shardBarTransform in shardBarTransforms.Values)
        {
            Destroy(shardBarTransform.gameObject);
        }
        shardBarTransforms.Clear();
    }

    private void Update()
    {
        if (memorySystem == null) return;

        float xOffset = 0f; // Start on left
        for(int i = 0; i < shardBarTransforms.Count; i++)
        {
            RectTransform shardBarTransform = shardBarTransforms.ElementAt(i).Value;
            string shardType = shardBarTransforms.ElementAt(i).Key;
            int shardCount = memorySystem.ShardDictionary[shardType].Count;

            // Set width based on shard count
            shardBarTransform.sizeDelta = new Vector2(
                memoryBarTransform.sizeDelta.x * (shardCount / (float)memorySystem.GetMaxShards()),
                memoryBarTransform.sizeDelta.y
            );

            // Position relative to the left side
            shardBarTransform.anchoredPosition = new Vector2(xOffset, 0f);

            // Move xOffset for the next shard
            xOffset += shardBarTransform.sizeDelta.x;
        }
    }

    /// <summary>
    /// Creates a new UI bar at runtime.
    /// </summary>
    /// <param name="shardType">The shard holder type.</param>
    private RectTransform CreateNewShardBar(string shardType)
    {
        GameObject newUIObject = new GameObject($"{shardType}ShardBar", typeof(RectTransform));
        newUIObject.transform.SetParent(memoryBarTransform, false);

        RectTransform shardBarTransform = newUIObject.GetComponent<RectTransform>();

        // Set pivot and anchor to left-middle
        shardBarTransform.pivot = new Vector2(0f, 0.5f);
        shardBarTransform.anchorMin = new Vector2(0f, 0.5f);
        shardBarTransform.anchorMax = new Vector2(0f, 0.5f);

        // Set size
        shardBarTransform.sizeDelta = new Vector2(0f, memoryBarTransform.sizeDelta.y);

        Image shardBarImage = newUIObject.AddComponent<Image>();
        shardBarImage.color = memorySystem.ShardDictionary[shardType].Color;

        return shardBarTransform;
    }
}

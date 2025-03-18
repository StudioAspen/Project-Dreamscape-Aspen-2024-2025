using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player player;
    private MomentumSystem momentumSystem;
    private ChainingSystem chainingSystem;
    private LevelSystem levelSystem;
    private AspectsManager aspectsManager;
    private MemorySystem memorySystem;

    [Header("Combat")]
    [SerializeField] private TMP_Text momentumText;
    [SerializeField] private TMP_Text chainText;

    [Header("Health")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;

    [Header("Experience")]
    [SerializeField] private Slider expBar;
    [SerializeField] private TMP_Text levelText;

    [Header("Aspects")]
    [SerializeField] private Image[] aspectsIcons = new Image[2];
    [SerializeField] private Sprite defaultAspectsIconSprite;

    [Header("Memory")]
    [SerializeField] private RectTransform memoryBarTransform;
    [SerializeField] private TMP_Text memoryText;
    private Dictionary<string, RectTransform> shardBarTransforms = new();

    // Awake is safe here because UI Scene loads last
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        momentumSystem = player.GetComponent<MomentumSystem>();
        chainingSystem = player.GetComponent<ChainingSystem>();
        levelSystem = player.GetComponent<LevelSystem>();
        aspectsManager = player.GetComponent<AspectsManager>();
        memorySystem = player.GetComponent<MemorySystem>();

        aspectsManager.OnAspectTreeAdded += AspectsManager_OnAspectTreeAdded;

        memorySystem.OnNewShardTypeAdded += MemorySystem_OnNewShardTypeAdded;
        memorySystem.OnShardAdded += MemorySystem_OnShardAdded;
        memorySystem.OnMemoryBarFull += MemorySystem_OnMemoryBarFull;
        memorySystem.OnMemoryAbilityActivated += MemorySystem_OnMemoryAbilityActivated;
    }

    private void Start()
    {
        UpdateAspectsIcons();
    }

    private void Update()
    {
        UpdateCombatUI();
        UpdateExpBar();
        UpdateHealthBar();
    }

    private void OnDestroy()
    {
        aspectsManager.OnAspectTreeAdded -= AspectsManager_OnAspectTreeAdded;

        memorySystem.OnNewShardTypeAdded -= MemorySystem_OnNewShardTypeAdded;
        memorySystem.OnShardAdded -= MemorySystem_OnShardAdded;
        memorySystem.OnMemoryBarFull -= MemorySystem_OnMemoryBarFull;
        memorySystem.OnMemoryAbilityActivated -= MemorySystem_OnMemoryAbilityActivated;
    }

    private void AspectsManager_OnAspectTreeAdded(AspectTree newTree)
    {
        UpdateAspectsIcons();
    }

    private void MemorySystem_OnNewShardTypeAdded(string shardHolderType)
    {
        shardBarTransforms.Add(shardHolderType, CreateNewShardBar(shardHolderType));
    }

    private void MemorySystem_OnShardAdded(string shardHolderType)
    {
        memoryText.text = $"{memorySystem.GetTotalShards()}/{memorySystem.GetMaxShards()}";

        float xOffset = 0f; // Start on left
        for (int i = 0; i < shardBarTransforms.Count; i++)
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

    private void UpdateHealthBar()
    {
        int playerHealth = Mathf.Clamp(player.CurrentHealth, 0, player.MaxHealth.GetIntValue());
        float healthFraction = playerHealth / (float)player.MaxHealth.GetIntValue();
        healthBar.value = healthFraction;

        healthText.text = $"{playerHealth}/{player.MaxHealth.GetIntValue()}";
    }

    private void UpdateExpBar()
    {
        float targetExpValue = levelSystem.CurrentEXP / (float)levelSystem.MaxEXP;
        expBar.value = Mathf.Lerp(expBar.value, targetExpValue, 10f * Time.unscaledDeltaTime);
        levelText.text = $"{levelSystem.Level}";
        //Debug.Log($"Current: {levelSystem.CurrentEXP}, Max: {levelSystem.MaxEXP}, Bar value: {expBar.value}, Target value: {targetExpValue}");
    }

    private void UpdateCombatUI()
    {
        // TODO: Need to make these invisible when at 0 and cool popup animation when they change

        momentumText.text = $"MOMENTUM: {momentumSystem.Momentum}";
        chainText.text = $"CHAIN: {chainingSystem.ChainCount}";
    }

    private void UpdateAspectsIcons()
    {
        for(int i = 0; i < aspectsManager.EquippedAspectTrees.Length; i++)
        {
            AspectTree tree = aspectsManager.EquippedAspectTrees[i];
            if(tree == null)
            {
                aspectsIcons[i].sprite = defaultAspectsIconSprite;
                continue;
            }

            aspectsIcons[i].sprite = tree.AspectSprite == null ? defaultAspectsIconSprite : tree.AspectSprite;
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

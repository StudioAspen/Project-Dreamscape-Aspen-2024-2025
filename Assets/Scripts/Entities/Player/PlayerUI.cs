using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player player;
    private AspectsManager aspectsManager;
    private MemorySystem memorySystem;
    private GameManager gameManager; //state changes
    private PlayerControls playerControls; //minimap keybind

    [Header("Health")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthText;

    [Header("Aspects")]
    [SerializeField] private Image[] aspectsIcons = new Image[3];
    [SerializeField] private Sprite defaultAspectsIconSprite;

    [Header("Memory")]
    [SerializeField] private RectTransform memoryBarTransform;
    [SerializeField] private TMP_Text memoryText;
    private Dictionary<string, RectTransform> shardBarTransforms = new();

    [Header("Minimap")]
    [SerializeField] private RectTransform minimapTransform; //Component used for transforming 2d ui element.
    private Vector2 normalSize;
    private Vector2 normalPosition;
    private Vector2 centeredPosition;

    private bool isMaximized = false; //maximized is when component is activated.
    [SerializeField] Vector2 maximizedSize = new Vector2(300, 300);
    [SerializeField] int maximizedZoom = 80;
    [Range(0.0f, 1.0f)][SerializeField] private float minimap_opacity = 0.75f;
    private float originalZoom;

    [SerializeField] private Camera minimapCamera; //Top down camera that follows player (keeps them at center).
    private CinemachineVirtualCamera virtualCamera;
    private Cinemachine3rdPersonFollow thirdPersonFollow;

    [SerializeField] private RawImage mapTexture; //Image of floor for minimap.
    private Mask mask;
    private RawImage image;
    private Transform border;

   



    // Awake is safe here because UI Scene loads last
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        aspectsManager = player.GetComponent<AspectsManager>();
        memorySystem = player.GetComponent<MemorySystem>();
        gameManager = FindAnyObjectByType<GameManager>();

        player.OnEntityTakeDamage += Player_OnEntityTakeDamage;
        player.OnEntityHeal += Player_OnEntityHeal;

        aspectsManager.OnAspectTreeAdded += AspectsManager_OnAspectTreeAdded;

        memorySystem.OnNewShardTypeAdded += MemorySystem_OnNewShardTypeAdded;
        memorySystem.OnShardAdded += MemorySystem_OnShardAdded;
        memorySystem.OnMemoryBarFull += MemorySystem_OnMemoryBarFull;
        memorySystem.OnMemoryAbilityActivated += MemorySystem_OnMemoryAbilityActivated;

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged; //subscribe to event
    }

    private void Start()
    {
        UpdateHealthBar(player.CurrentHealth);
        UpdateAspectsIcons();

        // Cache Components
        normalSize = minimapTransform.sizeDelta;
        normalPosition = minimapTransform.anchoredPosition;
        originalZoom = minimapCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().VerticalArmLength;

        RectTransform canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>(); //*Unsure how system wants to be implemented but using Main Canvas Component instead of a New Child Canvas*
        centeredPosition = new Vector2(-(canvasRT.rect.width / 2), canvasRT.rect.height / 2);

        virtualCamera = minimapCamera.GetComponent<CinemachineVirtualCamera>();
        thirdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        mask = GetComponentInChildren<Mask>();
        image = mask.GetComponentInChildren<RawImage>();
        border = transform.Find("Minimap/Minimap Background");

        playerControls = FindAnyObjectByType<GameInputManager>().PlayerControls;
        playerControls.Gameplay.ToggleMinimap.performed += PlayerControls_OnToggleMinimapPerformed; //subscribe to event

    }


    private void OnDestroy()
    {
        player.OnEntityTakeDamage += Player_OnEntityTakeDamage;
        player.OnEntityHeal += Player_OnEntityHeal;

        aspectsManager.OnAspectTreeAdded -= AspectsManager_OnAspectTreeAdded;

        memorySystem.OnNewShardTypeAdded -= MemorySystem_OnNewShardTypeAdded;
        memorySystem.OnShardAdded -= MemorySystem_OnShardAdded;
        memorySystem.OnMemoryBarFull -= MemorySystem_OnMemoryBarFull;
        memorySystem.OnMemoryAbilityActivated -= MemorySystem_OnMemoryAbilityActivated;

        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged; //unsubscribe to event
        playerControls.Gameplay.ToggleMinimap.performed -= PlayerControls_OnToggleMinimapPerformed; //unsubscribe to event
    }

    private void Player_OnEntityHeal(Entity entity, int healAmount)
    {
        UpdateHealthBar(player.CurrentHealth);
    }

    private void Player_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        UpdateHealthBar(player.CurrentHealth);
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

    private void UpdateHealthBar(int newCurrentHealth)
    {
        float healthFraction = newCurrentHealth / player.MaxHealth.GetFloatValue();
        healthFraction = Mathf.Clamp(healthFraction, 0f, 1f);
        healthBar.value = healthFraction;

        healthText.text = $"{newCurrentHealth}/{player.MaxHealth.GetFloatValue()}";
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


    //MINIMAP
    private void PlayerControls_OnToggleMinimapPerformed(InputAction.CallbackContext context)
    {
        ToggleMinimap();
    }

    public void ToggleMinimap()
    {
        // Disable the Toggle when the game isn't running
        if ((gameManager.CurrentState != GameState.PLAYING) && !isMaximized)
        {
            return;
        }

        // Toggle Mask and Border
        if (border != null)
        {
           border.gameObject.SetActive(isMaximized);
        }

        if (mask != null)
        {
            mask.enabled = !mask.enabled;
            mask.GetComponent<Image>().enabled = !mask.GetComponent<Image>().enabled;
        }

        // Toggle Solid Color Background of Minimap
        if (minimapCamera.TryGetComponent(out UniversalAdditionalCameraData cameraData))
        {
            cameraData.renderPostProcessing = !cameraData.renderPostProcessing;
        }

        // Map is Maximized --> Minimize map
        if (isMaximized)
        {
            // Make Minimap smaller and move into corner
            minimapTransform.sizeDelta = normalSize;
            minimapTransform.anchoredPosition = normalPosition;
            thirdPersonFollow.VerticalArmLength = originalZoom;

            Color newColor = image.color;
            newColor.a = 1.0f;
            image.color = newColor;
        }
        // Map is Minimized --> Maximize Map
        else
        {
            if (image != null)
            {
                Color newColor = image.color;
                newColor.a = minimap_opacity;
                image.color = newColor;
            }

            // Center Map and Increaes Size
            minimapTransform.sizeDelta = maximizedSize;
            minimapTransform.anchoredPosition = centeredPosition;

            thirdPersonFollow.VerticalArmLength = maximizedZoom;
        }

        isMaximized = !isMaximized;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        // If the game isn't running, hide minimap
        if ((newState != GameState.PLAYING))
        {
            if (isMaximized)
            {
                ToggleMinimap();
            }
        }
    }
}

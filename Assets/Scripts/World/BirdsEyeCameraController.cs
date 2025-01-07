using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BirdsEyeCameraController : MonoBehaviour
{
    private GameManager gameManager;
    private WorldManager worldManager;
    private ProgressionManager progressionManager;

    [Header("References")]
    [SerializeField] private GameObject landPlacementUIObject;
    [SerializeField] private GameObject landEmpowermentUIObject;

    [Header("Settings")]
    [SerializeField] private float minMoveSpeed = 30f;
    [SerializeField] private float maxMoveSpeed = 100f;
    [SerializeField] private float zoomSpeed = 500f;
    [SerializeField] private float minZoom = 25f;
    [SerializeField] private float maxZoom = 100f;
    private float currentMoveSpeed;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        worldManager = FindObjectOfType<WorldManager>();
        progressionManager = FindObjectOfType<ProgressionManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        currentMoveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, (transform.position.y - minZoom)/(maxZoom - minZoom));
    }

    private void Update()
    {
        HandleMoveInput();
        HandleZoomInput();

        HandleEmpowerInput();
        HandleWeakenInput();
        HandleContinueInput();
        HandleResetProgressionChangesInput();

        HandlePlaceLandInput();

        HandleGhostLand();
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        Disable();
        landPlacementUIObject.SetActive(false);
        landEmpowermentUIObject.SetActive(false);

        if(newState == GameState.LAND_PLACEMENT)
        {
            Enable();
            landPlacementUIObject.SetActive(true);
        }
        
        if (newState == GameState.LAND_EMPOWERMENT)
        {
            Enable();
            landEmpowermentUIObject.SetActive(true);
        }    
    }

    private void HandleMoveInput()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        transform.Translate(currentMoveSpeed * Time.unscaledDeltaTime * moveDirection, Space.World);
    }

    private void HandleZoomInput()
    {
        float zoomDelta = Input.mouseScrollDelta.y;

        Vector3 zoomDirection = new Vector3(0, -zoomDelta, 0).normalized;

        transform.Translate(zoomSpeed * Time.unscaledDeltaTime * zoomDirection, Space.World);

        float yPosition = transform.position.y;
        yPosition = Mathf.Clamp(yPosition, minZoom, maxZoom);
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);

        currentMoveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, (transform.position.y - minZoom) / (maxZoom - minZoom));
    }

    private void HandlePlaceLandInput()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(gameManager.CurrentState != GameState.LAND_PLACEMENT) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            worldManager.TrySpawnLandAtGhost();
        }
    }

    private void HandleEmpowerInput()
    {
        if (gameManager.CurrentState != GameState.LAND_EMPOWERMENT) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            progressionManager.TryEmpowerLandAtGhost();
        }
    }

    private void HandleWeakenInput()
    {
        if (gameManager.CurrentState != GameState.LAND_EMPOWERMENT) return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            progressionManager.TryWeakenLandAtGhost();
        }
    }

    private void HandleContinueInput()
    {
        if (gameManager.CurrentState != GameState.LAND_EMPOWERMENT) return;
        if (!progressionManager.CanProceedFromEmpowerment()) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            progressionManager.ContinueToEventSelection();
        }
    }

    private void HandleResetProgressionChangesInput()
    {
        if (gameManager.CurrentState != GameState.LAND_EMPOWERMENT) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            progressionManager.RefundProgressionChanges();
        }
    }

    private void HandleGhostLand()
    {
        worldManager.SetGhostLandPosition(transform.position);
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        worldManager.EnableLandLevelTexts();
        worldManager.EnableGhostLand();
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        worldManager.DisableLandLevelTexts();
        worldManager.DisableGhostLand();
    }
}

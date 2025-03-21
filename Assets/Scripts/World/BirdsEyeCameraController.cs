using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BirdsEyeCameraController : MonoBehaviour
{
    private PlayerControls playerControls;
    private GameManager gameManager;
    private WorldManager worldManager;
    private ProgressionManager progressionManager;

    [Header("Settings")]
    [SerializeField] private float minMoveSpeed = 30f;
    [SerializeField] private float maxMoveSpeed = 100f;
    [SerializeField] private float zoomSpeed = 500f;
    [SerializeField] private float minZoom = 25f;
    [SerializeField] private float maxZoom = 100f;
    private Vector3 moveDirection;
    private float currentMoveSpeed;
    private float zoomDelta;

    private void Start()
    {
        playerControls = FindObjectOfType<GameInputManager>().PlayerControls;
        gameManager = FindObjectOfType<GameManager>();
        worldManager = FindObjectOfType<WorldManager>();
        progressionManager = FindObjectOfType<ProgressionManager>();

        playerControls.LandPlacement.Movement.performed += PlayerControls_OnMovementPerformed;
        playerControls.LandPlacement.Movement.canceled += PlayerControls_OnMovementCanceled;
        playerControls.LandPlacement.Zoom.performed += PlayerControls_OnZoomPerformed;
        playerControls.LandPlacement.Zoom.canceled += PlayerControls_OnZoomCanceled;
        playerControls.LandPlacement.Select.performed += PlayerControls_LandPlacement_OnSelectedPerformed;

        playerControls.LandEmpowerment.Movement.performed += PlayerControls_OnMovementPerformed;
        playerControls.LandEmpowerment.Movement.canceled += PlayerControls_OnMovementCanceled;
        playerControls.LandEmpowerment.Zoom.performed += PlayerControls_OnZoomPerformed;
        playerControls.LandEmpowerment.Empower.performed += PlayerControls_LandEmpowerment_OnEmpowerPerformed;
        playerControls.LandEmpowerment.Weaken.performed += PlayerControls_LandEmpowerment_OnWeakenPerformed;
        playerControls.LandEmpowerment.Reset.performed += PlayerControls_LandEmpowerment_OnResetPerformed;
        playerControls.LandEmpowerment.Continue.performed += PlayerControls_LandEmpowerment_OnContinuePerformed;

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        currentMoveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, (transform.position.y - minZoom) / (maxZoom - minZoom));

        GameManager_OnGameStateChanged(gameManager.CurrentState); // Manually call this to set the initial state of the camera. Race case with gameManager.
    }

    private void OnDestroy()
    {
        playerControls.LandPlacement.Movement.performed -= PlayerControls_OnMovementPerformed;
        playerControls.LandPlacement.Zoom.performed -= PlayerControls_OnZoomPerformed;
        playerControls.LandPlacement.Select.performed -= PlayerControls_LandPlacement_OnSelectedPerformed;

        playerControls.LandEmpowerment.Movement.performed -= PlayerControls_OnMovementPerformed;
        playerControls.LandEmpowerment.Zoom.performed -= PlayerControls_OnZoomPerformed;
        playerControls.LandEmpowerment.Empower.performed -= PlayerControls_LandEmpowerment_OnEmpowerPerformed;
        playerControls.LandEmpowerment.Weaken.performed -= PlayerControls_LandEmpowerment_OnWeakenPerformed;
        playerControls.LandEmpowerment.Reset.performed -= PlayerControls_LandEmpowerment_OnResetPerformed;
        playerControls.LandEmpowerment.Continue.performed -= PlayerControls_LandEmpowerment_OnContinuePerformed;

        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();

        HandleGhostLand();
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        Disable();

        if(newState == GameState.LAND_PLACEMENT || newState == GameState.LAND_EMPOWERMENT)
        {
            Enable();
        }
    }

    private void PlayerControls_OnMovementPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }

    private void PlayerControls_OnMovementCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveDirection = Vector3.zero;
    }

    private void HandleMovement()
    {
        transform.Translate(currentMoveSpeed * Time.unscaledDeltaTime * moveDirection, Space.World);
    }

    private void PlayerControls_OnZoomPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        zoomDelta = context.ReadValue<Vector2>().y;
    }

    private void PlayerControls_OnZoomCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        zoomDelta = 0;
    }

    private void HandleZoom()
    {
        Vector3 zoomDirection = new Vector3(0, -zoomDelta, 0);

        transform.Translate(zoomSpeed * Time.unscaledDeltaTime * zoomDirection, Space.World);

        float yPosition = transform.position.y;
        yPosition = Mathf.Clamp(yPosition, minZoom, maxZoom);
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);

        currentMoveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, (transform.position.y - minZoom) / (maxZoom - minZoom));
    }

    private void PlayerControls_LandPlacement_OnSelectedPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (gameManager.CurrentState != GameState.LAND_PLACEMENT) return;

        worldManager.TrySpawnLandAtGhost();
    }

    private void PlayerControls_LandEmpowerment_OnEmpowerPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (gameManager.CurrentState != GameState.LAND_EMPOWERMENT) return;

        progressionManager.TryEmpowerLandAtGhost();
    }

    private void PlayerControls_LandEmpowerment_OnWeakenPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (gameManager.CurrentState != GameState.LAND_EMPOWERMENT) return;

        progressionManager.TryWeakenLandAtGhost();
    }

    private void PlayerControls_LandEmpowerment_OnResetPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (gameManager.CurrentState != GameState.LAND_EMPOWERMENT) return;

        progressionManager.RefundProgressionChanges();
    }

    private void PlayerControls_LandEmpowerment_OnContinuePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (gameManager.CurrentState != GameState.LAND_EMPOWERMENT) return;
        if (!progressionManager.CanProceedFromEmpowerment()) return;

        progressionManager.ContinueToEventSelection();
    }

    private void HandleGhostLand()
    {
        worldManager.SetGhostLandPosition(transform.position);
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        worldManager.EnableLandLevelTexts();
        worldManager.ToggleLandLevelStyle(worldManager.LandLevelStyleIsSimple); // Debug delete later
        worldManager.EnableGhostLand();
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        worldManager.DisableLandLevelTexts();
        worldManager.DisableGhostLand();
    }
}

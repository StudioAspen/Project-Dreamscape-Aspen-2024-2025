using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BirdsEyeCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float zoomSpeed = 500f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 100f;
    private float zoom;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleMoveInput();
        HandleZoomInput();
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if(newState != GameState.LAND_PLACEMENT)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
    }

    private void HandleMoveInput()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        transform.Translate(moveSpeed * Time.unscaledDeltaTime * moveDirection, Space.World);
    }

    private void HandleZoomInput()
    {
        float zoomDelta = Input.mouseScrollDelta.y;

        Vector3 zoomDirection = new Vector3(0, -zoomDelta, 0).normalized;

        transform.Translate(zoomSpeed * Time.unscaledDeltaTime * zoomDirection, Space.World);

        float yPosition = transform.position.y;
        yPosition = Mathf.Clamp(yPosition, minZoom, maxZoom);
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
    }

    private SelectionSphere GetMouseLookSelectionSphere()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        bool didHit = Physics.Raycast(mouseRay, out hit, Mathf.Infinity, LayerMask.GetMask("SelectionSphere"));

        if (!didHit) return null;

        return hit.transform.GetComponent<SelectionSphere>();
    }
}

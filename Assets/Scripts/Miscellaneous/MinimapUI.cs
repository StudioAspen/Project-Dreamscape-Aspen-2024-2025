using UnityEngine;
using UnityEngine.InputSystem;

public class MinimapUI : MonoBehaviour
{
    private PlayerControls playerControls;

    [Header("References")]
    [SerializeField] private GameObject shrunkMinimapObject;

    [Header("Expanded Config")]
    [SerializeField] private float expandedOpacity = 0.75f;

    private bool isExpanded;

    private void Awake()
    {
        playerControls = FindObjectOfType<GameInputManager>().PlayerControls;
        playerControls.Gameplay.ToggleMinimap.performed += PlayerControls_OnToggleMinimapPerformed;
    }

    private void OnDestroy()
    {
        playerControls.Gameplay.ToggleMinimap.performed -= PlayerControls_OnToggleMinimapPerformed;
    }

    private void OnDisable()
    {
        ShrinkMinimap();
    }

    private void PlayerControls_OnToggleMinimapPerformed(InputAction.CallbackContext context)
    {
        if (isExpanded) ShrinkMinimap();
        else ExpandMinimap();
    }

    private void ExpandMinimap()
    {
        isExpanded = true;
    }

    private void ShrinkMinimap()
    {
        isExpanded = false;
    }
}

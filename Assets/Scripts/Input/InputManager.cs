using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputManager : MonoBehaviour
{
    public PlayerControls PlayerControls { get; private set; }

    public enum ControlScheme
    {
        KEYBOARD_MOUSE,
        GAMEPAD
    }
    public ControlScheme CurrentControlScheme { get; private set; } = ControlScheme.KEYBOARD_MOUSE;
    public Action<ControlScheme> OnControlSchemeChanged = delegate { };

    private void Awake()
    {
        PlayerControls = new PlayerControls();
        PlayerControls.Enable();
        InputSystem.onEvent += InputSystem_OnEvent;

        OnAwake();
    }

    private protected virtual void OnAwake()
    {

    }

    private void Start()
    {
        OnStart();
    }

    private protected virtual void OnStart()
    {

    }

    private void OnDestroy()
    {
        PlayerControls.Disable();
        InputSystem.onEvent -= InputSystem_OnEvent;

        PlayerControls.Dispose();
        PlayerControls = null;

        OnOnDestroy();
    }

    private protected virtual void OnOnDestroy()
    {

    }

    private void InputSystem_OnEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // Detect the type of input source being used
        if (device is Gamepad)
        {
            SetControlScheme(ControlScheme.GAMEPAD);
        }
        else if (device is Keyboard || device is Mouse)
        {
            SetControlScheme(ControlScheme.KEYBOARD_MOUSE);
        }
    }

    /// <summary>
    /// Sets the control scheme variable.
    /// </summary>
    /// <param name="newControlScheme">The new control scheme to set.</param>
    private void SetControlScheme(ControlScheme newControlScheme)
    {
        if (newControlScheme != CurrentControlScheme)
        {
            // Add your logic for switching input UI or behavior
            CurrentControlScheme = newControlScheme;

            OnControlSchemeChanged?.Invoke(newControlScheme);
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        if (CurrentControlScheme == ControlScheme.GAMEPAD) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnableControls(bool isEnabled)
    {
        if(isEnabled) PlayerControls.Enable();
        else PlayerControls.Disable();
    }
}



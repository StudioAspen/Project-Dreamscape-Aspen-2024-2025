using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public PlayerControls PlayerControls { get; private set; }

    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;

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

    public static void StartRebind(PlayerControls playerControls, string actionName, int bindingIndex, TextMeshProUGUI statusText, bool excludeMouse)
    {
        InputAction action = playerControls.asset.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex) //In The Case You Put In The Incorrect InputAction Asset.
        {
            Debug.Log("NO BINDING AVAILABLE!");
            return;
        }

        //Composite Binding -> Action With Multiple Paths For [1] Action:   Example -->     Walk: W,A,S,D [Composite]     &   Jump: Space [!Composite]
        if (action.bindings[bindingIndex].isComposite)
        {
            //Ensure We Start At The Proper Path:
            var firstPartIndex = bindingIndex + 1; //Note:      Index[0] = WASD,     Index[1] = W,       Index[2] = S,       Index[3] = A,       Index[4] = D

            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, firstPartIndex, statusText, true, excludeMouse);
            }
        }
        //Non-Composite Binding
        else
        {
            DoRebind(action, bindingIndex, statusText, false, excludeMouse);
        }
    }


    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TextMeshProUGUI statusText, bool allCompositeParts, bool excludeMouse)
    {
        if (actionToRebind == null || bindingIndex < 0) return;

        //Feedback Text When Changing Bindings:
        statusText.text = $"Press a {actionToRebind.expectedControlType}";


        //Rebind Process:
        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);


        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose(); //Dispose In Order To Avoid Memory Leak

            //Composite Binding: Ensures Composite Bindings Can Be Adjusted In [1 input]
            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;

                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                {
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
                }
            }

            SaveBindingOverride(actionToRebind);

            //If "rebindComplete" -> Was Subscribed too:
            rebindComplete?.Invoke();
        });


        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose(); //Dispose In Order To Avoid Memory Leak

            //If "rebindCanceled" -> Was Subscribed too:
            rebindCanceled?.Invoke();
        });

        /*
        rebind.WithCancelingThrough("<Keyboard>/escape");   //IN THE CASE YOU WOULD WANT TO HAVE A BINDING TO CANCEL THE REBINDING PROCESS
        */

        if (excludeMouse)
        {
            rebind.WithControlsExcluding("Mouse");
        }

        //If "rebindStarted" -> Was Subscribed too:
        rebindStarted?.Invoke(actionToRebind, bindingIndex);

        rebind.Start(); //Starts The Rebinding Process
    }

    public static string GetBindingName(PlayerControls playerControls, string actionName, int bindingIndex)
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        InputAction action = playerControls.asset.FindAction(actionName);

        return action.GetBindingDisplayString(bindingIndex);
    }

    private static void SaveBindingOverride(InputAction action)
    {
        //Loop Through [ALL BINDINGS] -> Then Save
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(PlayerControls playerControls, string actionName)
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        InputAction action = playerControls.asset.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
            {
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
            }
        }
    }

    public static void ResetBinding(PlayerControls playerControls, string actionName, int bindingIndex)
    {
        InputAction action = playerControls.asset.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("NO ACTION AVAILABLE..");
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            int partIndex = bindingIndex + 1; // First composite part (W)

            while (partIndex < action.bindings.Count && action.bindings[partIndex].isPartOfComposite)
            {
                action.RemoveBindingOverride(partIndex);
                PlayerPrefs.DeleteKey(action.actionMap + action.name + partIndex);
                partIndex++; // Move to next composite part (A, S, D)
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
            PlayerPrefs.DeleteKey(action.actionMap + action.name + bindingIndex);
        }

        PlayerPrefs.Save();
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RebindUI : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField] private InputActionReference inputActionReference; //Drag Component (Scriptable Object) into Inspector For Specified Input Action.

    [SerializeField] private bool excludeMouse = true; //Toggle in the Case to Disable Rebinding to Mouse Inputs. 

    [Range(0, 1)]
    [SerializeField] private int selectedBinding; //In Our Case we only need a Range from (0,1) Given: MouseBinding = 0 , GamepadBinding = 1.

    [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions; //Built-in Enum to Format Binding Name For Readability.

   
    [Header("Binding Info - DO NOT EDIT VIEW ONLY")]
    [SerializeField] private InputBinding inputBinding; //Used To Read In The Inspector To Know what Our Current Binding Is.

    private int bindingIndex;
    private string actionName;

    [Header("UI Fields")]
    [SerializeField] private TextMeshProUGUI actionText;
    [Space]
    [SerializeField] private Button rebindButton;
    [SerializeField] private TextMeshProUGUI rebindText;
    [Space]
    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI resetText;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
    }

    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => ResetBinding());

        if (inputActionReference != null)
        {
            InputManager.LoadBindingOverride(inputManager.PlayerControls, actionName);
            GetBindingInfo();
            UpdateUI();
        }

        InputManager.rebindComplete += UpdateUI;
        InputManager.rebindCanceled += UpdateUI;
    }

    private void OnDisable()
    {
        InputManager.rebindComplete -= UpdateUI;
        InputManager.rebindCanceled -= UpdateUI;
    }


    //OnValidate -> Called When Something is Changed In The Inspector
    private void OnValidate()
    {
        if (inputActionReference == null) return;

        GetBindingInfo();
        UpdateUI();
        
    }

    private void GetBindingInfo()
    {
        //Setting actionName[String] To Be The Same As The Specified Action:
        if (inputActionReference.action != null)
        {
            actionName = inputActionReference.action.name;
        }

        //Implemented In The Case We Have a Wider Range of Selected Bindings then there are Actual Inputs Available:
        if (inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }


    private void UpdateUI()
    {
        //Setting the actionText[TMPro] to the predefined actionName[String]:
        if (actionText != null)
        {
            actionText.text = actionName;
        }


        if (rebindText != null)
        {
            if (Application.isPlaying)
            {
                rebindText.text = InputManager.GetBindingName(inputManager.PlayerControls, actionName, bindingIndex);
            }
            else
            {
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
            }
        }
    }


    private void DoRebind()
    {
        if (inputManager == null) return;

        InputManager.StartRebind(inputManager.PlayerControls, actionName, bindingIndex, rebindText, excludeMouse);
    }


    private void ResetBinding()
    {
        InputManager.ResetBinding(inputManager.PlayerControls, actionName, bindingIndex);
        UpdateUI();
    }
}

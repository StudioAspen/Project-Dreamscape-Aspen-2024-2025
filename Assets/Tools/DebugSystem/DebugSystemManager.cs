using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GetMikyled.DebugSystem.FreeLookCamera;

namespace GetMikyled.DebugSystem
{
	//-/////////////////////////////////////////////////////////////////////////////////////////////////
	///
	public class DebugSystemManager : MonoBehaviour
	{
		[SerializeField] private DeveloperConsole developerConsole;
		[SerializeField] private DebugFreeLookCamera freeLookCamera;

		private bool debugModeEnabled;
		private bool uiEnabled = true;
		private bool playerControlsEnabled = true;
		
		//-/////////////////////////////////////////////////////////////////////////////////////////////////
		///
		private void Update()
		{
			// If 'F5' key pressed -> Enable/Disable debug mode
			if (Input.GetKeyUp(KeyCode.F5))
			{
				SetDebugModeEnabled(!debugModeEnabled);
			}
			
			if (debugModeEnabled)
			{
				// Check for key press to active different debug tools
				if (Input.GetKeyUp(KeyCode.F1))
				{
					// Press 'F1' Key -> Enable / Disable UI
					UIManager uiManager = GameObject.FindObjectOfType<UIManager>(true);
					uiManager.gameObject.SetActive(!uiEnabled);

					uiEnabled = !uiEnabled;
				}
				else if (Input.GetKeyUp(KeyCode.F2))
				{
					// Press 'F2' Key -> Enable / Disable Free Look Camera
					freeLookCamera.SetActive(!freeLookCamera.isActive);
				}
				else if (Input.GetKeyUp(KeyCode.F3))
				{
					// Press 'F3' Key -> Enable / Disable Player Controls
					GameInputManager gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
					gameInputManager.EnableControls(!playerControlsEnabled);
					
					playerControlsEnabled = !playerControlsEnabled;
					
					// Press 'F3' Key -> Enable / Disable Camera Free Look Controls
					freeLookCamera.SetFreeLookControlsEnabled(!freeLookCamera.freeLookControlsEnabled);
				}
			}
		}

		//-/////////////////////////////////////////////////////////////////////////////////////////////////
		///
		private void SetDebugModeEnabled(bool argEnabled)
		{
			debugModeEnabled = argEnabled;

			// Log enabled status to the console
			if (argEnabled)
			{
				Debug.Log("Debug mode has been enabled");
			}
			else
			{
				Debug.Log("Debug mode has been disabled");
			}
		}
	}

}
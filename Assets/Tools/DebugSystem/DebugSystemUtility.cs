using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GetMikyled.DebugSystem
{
	//-/////////////////////////////////////////////////////////////////////////////////////////////////
	///
	public class DebugSystemUtility
	{
		//-/////////////////////////////////////////////////////////////////////////////////////////////////
		///
		/// Subscribes the Initialization of the DebugSystem
		/// 
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void SubscribeDeveloperConsoleInitialization()
		{
			SceneManager.sceneLoaded += InitializeDebugSystem;
		}
    
		static void InitializeDebugSystem(Scene scene, LoadSceneMode mode)
		{
			if (GameObject.Find("DebugSystem") == null)
			{
				Object.Instantiate(Resources.Load<GameObject>("DebugSystem"));
			}
			if (GameObject.FindObjectOfType(typeof(EventSystem)) == null)
			{
				new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
			}
		}
	}

}
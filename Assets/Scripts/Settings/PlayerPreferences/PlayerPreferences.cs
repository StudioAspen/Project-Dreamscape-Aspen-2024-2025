using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* Right now settings are not saved after exiting the game */
public class PlayerPreferences : MonoBehaviour
{
   public static PlayerPreferences Instance { get; private set; }
   public PlayerPreferencesData PlayerPreferencesData { get; private set; }
   
   private int numScreenResolutions;
   private Resolution[] resolutions;
   
   public event Action<float> OnCameraSensitivityChanged;

   private void Awake() 
   {
        if (Instance == null) 
        {
           Instance = this;
           
        } else {
           Destroy(gameObject);
        }

        InitializeResolutionsArray();
        
        // Attempt to load in player data
        (PlayerPreferencesData loadedPreferences, bool preferencesSaveFileFound) = TryLoadPlayerPreferences();
        PlayerPreferencesData = loadedPreferences;

        if (!preferencesSaveFileFound) {
           TrySavePlayerPreferences(); // Saves default player preferences into a JSON for future access
        }
        
        InitializeUserSettings();
    }

   /// <summary>
   /// Tries to load player preferences from JSON.
   /// If no JSON preferences save file found, creates a new JSON preferences save file.
   /// </summary>
   /// <returns>
   /// A tuple, containing:
   /// 1) The loaded player preference data, or just default player preferences if save file not found.
   /// 2) Whether a save file was found or not.
   /// </returns>
   private (PlayerPreferencesData, bool) TryLoadPlayerPreferences() {
      // InitializeResolutionArray must be called first before calling this method!
      PlayerPreferencesData loadedData = SaveLoadManager.LoadPlayerPreferences();
      if (loadedData == null) {
         return (GetDefaultPlayerPreferencesData(), false);
      }
      return (loadedData, true);
   }

   public void TrySavePlayerPreferences() {
      SaveLoadManager.SavePlayerPreferences(PlayerPreferencesData);
   }

   private PlayerPreferencesData GetDefaultPlayerPreferencesData() {
      return new PlayerPreferencesData {
         CameraSensitivity = .25f,
         MasterVolume = 1f,
         IsVSync = false,
         QualityLevel = 3,
         MaxFramerate = 60,
         CurrentScreenResolutionIndex = resolutions.Length - 1,
         FullScreenMode = FullScreenMode.FullScreenWindow,
      };
   }

   private void InitializeResolutionsArray() {
      // Initialize screen resolutions array (depends on user's display), filtering out non-unique or non-16:9 resolutions
      resolutions = Screen.resolutions
         .GroupBy(res => (res.width, res.height))
         .Select(group => group.First())
         .Where(res => Mathf.Approximately((float)res.width / res.height, 16f / 9f))
         .ToArray();

      numScreenResolutions = resolutions.Length;
      Debug.Log("Resolutions array length: " + numScreenResolutions);
   }
   
   private void InitializeUserSettings() {
      // InitializeResolutionsArray must be called first before calling this method!
      SetResolution(PlayerPreferencesData.CurrentScreenResolutionIndex);
      SetFullScreenMode(PlayerPreferencesData.FullScreenMode);
      SetMaximumFramerate(PlayerPreferencesData.MaxFramerate);
      SetVSync(PlayerPreferencesData.IsVSync);
      SetMasterVolume(PlayerPreferencesData.MasterVolume);
      SetCameraSensitivity(PlayerPreferencesData.CameraSensitivity);
      SetQualityLevel(PlayerPreferencesData.QualityLevel);
   }

   private void SetResolution(int screenResolutionIndex) {
      Resolution newResolution = resolutions[screenResolutionIndex];
      Screen.SetResolution(newResolution.width, newResolution.height, PlayerPreferencesData.FullScreenMode);
      SetMaximumFramerate(PlayerPreferencesData.MaxFramerate);
   }

   private void SetFullScreenMode(FullScreenMode newFullScreenMode) {
      PlayerPreferencesData.FullScreenMode = newFullScreenMode;
      Screen.fullScreenMode = newFullScreenMode;
   }
   
   private void SetMaximumFramerate(int newMax) {
      PlayerPreferencesData.MaxFramerate = newMax;
      Application.targetFrameRate = newMax;
      print($"Application target frame is now {Application.targetFrameRate}");
   }
   
   public void SetVSync(bool newValue) 
   {
      PlayerPreferencesData.IsVSync = newValue;
      QualitySettings.vSyncCount = newValue ? 1 : 0;
   }

   public void SetMasterVolume(float newValue) 
   {
      PlayerPreferencesData.MasterVolume = newValue;
      // Change master volume in Audio Manager with this new value
   }

   public void SetCameraSensitivity(float newValue) 
   {
      PlayerPreferencesData.CameraSensitivity = newValue;
      OnCameraSensitivityChanged?.Invoke(newValue); // Fire it off to subscriber in PlayerCameraController.cs
   }

   public void SetQualityLevel(int newValue) 
   {
      PlayerPreferencesData.QualityLevel = newValue;
      QualitySettings.SetQualityLevel(newValue);
   }

   #region String Formatters

   public String GetQualityLevelDisplay() 
   {
      switch (PlayerPreferencesData.QualityLevel) 
      {
         case 0:
            return "Potato";
         case 1:
            return "Low";
         case 2:
            return "Medium";
         case 3:
            return "High";
      }
      throw new ArgumentOutOfRangeException(nameof(PlayerPreferencesData.QualityLevel), "Quality level must be between 0 and 3.");
   }

   public String GetScreenResolutionDisplay() {
      return $"{resolutions[PlayerPreferencesData.CurrentScreenResolutionIndex].width}x{resolutions[PlayerPreferencesData.CurrentScreenResolutionIndex].height}";
   }

   public String GetFullScreenModeDisplay() 
   {
      switch (PlayerPreferencesData.FullScreenMode) 
      {
         case FullScreenMode.Windowed:
            return "Windowed";
         case FullScreenMode.FullScreenWindow:
            return "Fullscreen";
         case FullScreenMode.MaximizedWindow:
            return "Maximized";
      }
      throw new Exception("Invalid FullScreenMode: " + PlayerPreferencesData.FullScreenMode);
   }

   #endregion
   

   /// <summary>
   /// Cycles to the next max framerate, which is decided in this function's switch statement.
   /// Cycles in this order: 30 FPS -> 60 FPS -> UNCAPPED FPS -> 30 FPS -> ... 
   /// </summary>
   /// <returns>The new max framerate that was cycled to.</returns>
   /// <exception cref="ArgumentException"></exception>
   public int CycleMaxFramerate() { // -1 means uncapped framerate
      int newFramerate;
      switch (PlayerPreferencesData.MaxFramerate) {
         case 30:
            newFramerate = 60;
         break;
         case 60:
            newFramerate = -1;
         break;
         case -1:
            newFramerate = 30;
         break;
         default:
            throw new ArgumentException("MaxFramerate value must either be -1, 30, or 60.");      
      }
      SetMaximumFramerate(newFramerate);
      return newFramerate;
   }

   /// <summary>
   /// Cycles to the next screen resolution in the Screens.resolutions array, wrapping back to the front if needed. Then, updates screen resolution to this next screen resolution.
   /// </summary>
   /// <returns> The next index in the resolutions array that was cycled to. </returns>
   public int CycleScreenResolution() {
      PlayerPreferencesData.CurrentScreenResolutionIndex = (PlayerPreferencesData.CurrentScreenResolutionIndex + 1) % resolutions.Length;
      SetResolution(PlayerPreferencesData.CurrentScreenResolutionIndex);
      return PlayerPreferencesData.CurrentScreenResolutionIndex;
   }



   /// <summary>
   /// Cycles to the next full screen mode. Then, updates full screen mode to this next full screen mode.
   /// Cycles in this order: Windowed -> FullScreenWindow -> Windowed -> ...
   /// </summary>
   /// <returns>The next full screen mode that was cycled to.</returns>
   public FullScreenMode CycleFullScreenMode() {
      FullScreenMode nextFullScreenMode;
      switch (PlayerPreferencesData.FullScreenMode) {
         case FullScreenMode.Windowed:
            nextFullScreenMode = FullScreenMode.FullScreenWindow;
         break;
         case FullScreenMode.FullScreenWindow:
            nextFullScreenMode = FullScreenMode.Windowed;
         break;
         // Maximized window doesn't behave as intended so leaving it out
         // case FullScreenMode.MaximizedWindow:
         //    nextFullScreenMode = FullScreenMode.Windowed;
         // break;
         default:
            throw new ArgumentException("Invalid FullScreenMode: " + PlayerPreferencesData.FullScreenMode);
      }
      SetFullScreenMode(nextFullScreenMode);
      return nextFullScreenMode;
   }
   
   
}

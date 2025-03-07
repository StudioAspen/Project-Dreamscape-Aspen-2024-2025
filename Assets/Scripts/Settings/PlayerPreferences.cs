using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Right now settings are not saved after exiting the game */
public class PlayerPreferences : MonoBehaviour
{
   public static PlayerPreferences Instance { get; private set; }

   [SerializeField] public float CameraSensitivity {get; private set; } = .25f;
   [SerializeField] public float MasterVolume { get; private set; } = 1f; // Not connected with Audio Manager yet
   [SerializeField] public bool IsVSync {get; private set; } = false;
   [SerializeField] public int QualityLevel { get; private set; } = 3;
   
   public event Action<float> OnCameraSensitivityChanged;

   private void Awake() {
      if (Instance == null) {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      } else {
         Destroy(gameObject);
      }
      
   }

   private void Start() {
      // Set VSync and Volume
      QualitySettings.vSyncCount = IsVSync ? 1 : 0;
      SetMasterVolume(MasterVolume);
      SetCameraSensitivity(CameraSensitivity);
      SetQualityLevel(QualityLevel);

      // Setup event connections
   }

   public void SetVSync(bool newValue) {
      IsVSync = newValue;
      QualitySettings.vSyncCount = newValue ? 1 : 0;
   }

   public void SetMasterVolume(float newValue) {
      MasterVolume = newValue;
      // Change master volume in Audio Manager with this new value
   }

   public void SetCameraSensitivity(float newValue) {
      CameraSensitivity = newValue;
      OnCameraSensitivityChanged?.Invoke(newValue); // Fire it off to subscriber in PlayerCameraController.cs
   }

   public void SetQualityLevel(int newValue) {
      QualityLevel = newValue;
      QualitySettings.SetQualityLevel(newValue);
   }

   public String GetQualityLevelDisplay() {
      switch (QualityLevel) {
         case 0:
            return "Potato";
         break;
         case 1:
            return "Low";
         break;
         case 2:
            return "Medium";
         break;
         case 3:
            return "High";
         break;
         case 4:
            return "Very High";
         break;
      }
      throw new ArgumentOutOfRangeException(nameof(QualityLevel), "Quality level must be between 0 and 4.");
   }



}

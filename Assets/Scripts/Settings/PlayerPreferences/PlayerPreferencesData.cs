using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerPreferencesData {
  public float CameraSensitivity;
  public float MasterVolume;
  public bool IsVSync;
  public int QualityLevel;
  public int MaxFramerate;
  public int CurrentScreenResolutionIndex;
  public FullScreenMode FullScreenMode;
}

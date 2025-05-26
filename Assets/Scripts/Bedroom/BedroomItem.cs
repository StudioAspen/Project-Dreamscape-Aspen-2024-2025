using System.Collections;
using UnityEngine;

public class BedroomItem : MonoBehaviour
{
    [field: SerializeField] public BedroomItemConfigSO Config { get; private set; }
    public bool IsActivated { get; private set; } = false;
    [field: SerializeField] public Transform CameraTargetTransform { get; private set; }

    public bool Activate()
    {
        if (IsActivated)
        {
            Debug.LogWarning($"Called Activate() on already activated Item: {Config.DisplayName}. GameObject: {gameObject.name}");
            return false;
        }

        IsActivated = true;
        Debug.Log($"Activated Item: {Config.DisplayName}. GameObject: {gameObject.name}");

        return true;
    }
}

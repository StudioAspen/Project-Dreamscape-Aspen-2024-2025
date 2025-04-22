using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    private Quaternion startRotation;

    private void Awake()
    {
        startRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = startRotation; // Force the icon to always face the camera
    }
}

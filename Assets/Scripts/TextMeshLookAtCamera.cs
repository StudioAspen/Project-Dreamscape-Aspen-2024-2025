using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshLookAtCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Awake()
    {
        mainCameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
    }
}

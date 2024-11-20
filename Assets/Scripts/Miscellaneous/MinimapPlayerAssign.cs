using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

public class MinimapPlayerAssign : MonoBehaviour
{
    [SerializeField, Scene] private Player _player;
    [SerializeField] private Camera m_Camera;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    void Awake()
    {
        m_Camera.GetComponent<CinemachineVirtualCamera>().Follow = _player.transform;
    }
}
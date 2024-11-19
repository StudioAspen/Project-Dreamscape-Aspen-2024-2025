using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;

public class MinimapPlayerAssign : MonoBehaviour
{
    [SerializeField, Scene] private Player _player;
    [SerializeField] private Camera m_Camera;
    void Awake()
    {
        m_Camera.GetComponent<CinemachineVirtualCamera>().Follow = _player.transform;
    }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayerAssign : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private Camera m_Camera;
    void Awake()
    {
        m_Camera.GetComponent<CinemachineVirtualCamera>().Follow = Player.transform;
    }

}

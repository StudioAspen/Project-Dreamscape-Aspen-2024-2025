using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayerAssign : MonoBehaviour
{
    private Player player;

    [SerializeField] private Camera m_Camera;

    void Start()
    {
        player = FindObjectOfType<Player>();

        m_Camera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
    }
}
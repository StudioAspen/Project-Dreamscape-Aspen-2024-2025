using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.VFX;

public class FloatingPlatform : Obstacle
{
    [field: Header("FlaotingPlatform: States")]
    [field: SerializeField] public FloatingPlatformIdleState FloatingPlatformIdleState { get; private set; }
    [field: SerializeField] public FloatingPlatformSteppedState FloatingPlatformSteppedState { get; private set; }
    [field: SerializeField] public FloatingPlatformSinkingState FloatingPlatformSinkingState { get; private set; }
    [field: SerializeField] public FloatingPlatformRisingState FloatingPlatformRisingState { get; private set; }
    [field: SerializeField] public GameObject Pos1 { get; private set; }
    public Vector3 pos1;
    [field: SerializeField] public GameObject Pos2 { get; private set; }
    public Vector3 pos2;
    public float Speed = 0.5f;
    public float Pos2Dur = 3;

    private protected override void OnAwake()
    {

    }

    private protected override void OnStart()
    {
        
    }

    private protected override void OnUpdate()
    {

    }
}

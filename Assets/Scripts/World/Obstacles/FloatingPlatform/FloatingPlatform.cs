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

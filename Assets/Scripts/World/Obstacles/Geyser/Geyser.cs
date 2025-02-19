using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.VFX;

public class Geyser : Obstacle
{
    [field: Header("Geyser: States")]
    [field: SerializeField] public GeyserIdleState GeyserIdleState { get; private set; }
    [field: SerializeField] public GeyserWarningState GeyserWarningState { get; private set; }
    [field: SerializeField] public GeyserEruptingState GeyserEruptingState { get; private set; }
    [field: SerializeField] public GeyserCooldownState GeyserCooldownState { get; private set; }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenusFlyTrap : Obstacle
{
    [field: Header("VenusFlyTrap: States")]
    [field: SerializeField] public VenusFlyTrapIdleState VenusFlyTrapIdleState { get; private set; }
    [field: SerializeField] public VenusFlyTrapWindupState VenusFlyTrapWindupState { get; private set; }
    [field: SerializeField] public VenusFlyTrapSnapState VenusFlyTrapSnapState { get; private set; }
    private protected override void OnAwake()
    {
        // Initialization logic if needed
    }

    private protected override void OnStart()
    {
        // Start logic if needed
    }

    private protected override void OnUpdate()
    {
        // Handles state updates
    }

 
}




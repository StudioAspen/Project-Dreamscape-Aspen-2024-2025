using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaPeel : Obstacle
{
    [field: Header("BananaPeel: States")]
    [field: SerializeField] public BananaPeelIdleState BananaPeelIdleState { get; private set; }
    [field: SerializeField] public BananaPeelSteppedState BananaPeelSteppedState { get; private set; }


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

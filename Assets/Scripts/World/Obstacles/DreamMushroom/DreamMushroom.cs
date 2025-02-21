using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamMushroom : Obstacle
{
    [field: Header("DreamMushroom : States")]
    [field: SerializeField] public DreamMushroomIdleState DreamMushroomIdleState { get; private set; }
    [field: SerializeField] public DreamMushroomBurstState DreamMushroomBurstState { get; private set; }

    [field: SerializeField] public DreamMushroomSplotchState DreamMushroomSplotchState { get; private set; }


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

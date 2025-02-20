using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamMushroom : Obstacle
{
    [field: Header("DreamMushroom : States")]
    [field: SerializeField] public DreamMushroomIdleState DreamMushroomIdleState { get; set; }
    [field: SerializeField] public DreamMushroomBurstState DreamMushroomBurstState { get; set; }
    [field: SerializeField] public DreamMushroomHitState DreamMushroomHitState { get; set; }
    [field: SerializeField] public DreamMushroomStepOnState DreamMushroomStepOnState { get; set; }

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

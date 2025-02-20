using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DreamMushroomBaseState : ObstacleBaseState
{
    private protected DreamMushroom dreamMushroom;

    private protected override void Init()
    {
        dreamMushroom = obstacle as DreamMushroom;
    }

}

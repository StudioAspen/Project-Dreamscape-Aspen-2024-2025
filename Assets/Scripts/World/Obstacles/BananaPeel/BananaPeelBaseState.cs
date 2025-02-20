using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class BananaPeelBaseState : ObstacleBaseState
{
    private protected BananaPeel bananapeel;

    private protected override void Init()
    {
        bananapeel = obstacle as BananaPeel;
    }
}

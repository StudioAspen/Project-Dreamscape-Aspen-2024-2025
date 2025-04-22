using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class FloatingPlatformBaseState : ObstacleBaseState
{
    private protected FloatingPlatform floatingPlatform;

    private protected override void Init()
    {
        floatingPlatform = obstacle as FloatingPlatform;
    }
}
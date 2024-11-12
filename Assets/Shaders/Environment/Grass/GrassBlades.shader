Shader "Dreamscape/Foliage/GrassBlades"
{
    Properties
    {
        _RootColor("Root Color", Color) = (0, 0.5, 0, 1)    // Color of the grass root
        _TipColor("Tip Color", Color) = (0, 1, 0, 1)        // Color of grass tip
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Off    // No culling so grass is double sided
            
            HLSLPROGRAM
            // Signals that the shader needs a compute buffer
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            // Lighting and shadows
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "GrassBlades.hlsl"
            
            ENDHLSL
        }
    }
}

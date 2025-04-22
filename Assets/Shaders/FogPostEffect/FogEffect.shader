Shader "Custom/FogEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        //_NoiseTex ("Noise Texture", 2D) = "white" {}
       // _FogNoiseScale ("Noise Scale", Float) = 1.0
        //_FogNoiseVelocity ("Noise Velocity", Float) = 1.0
       // _SkyBoxNoiseScale ("Sky Box Noise Scale", Float) = 1.0
       //_SkyBoxNoiseVelocity ("Sky Box Noise Velocity", Float) = 1.0
       // _PFogColor ("Primary Fog Color", Color) = (1,1,1,1)
       // _SFogColor("Secondary Fog Color", Color) = (1,1,1,1)
       // _SkyBoxFogColor("Skybox Fog Color", Color) = (1,1,1,1)
       // _FogDensity ("Fog Density", Float) = 0.1 
      //  _SkyBoxFogDensity ("Fog Density", Float) = 0.1 
       // _FogOffset ("Fog Offset", Float) = 1.0  
      //  _SecondaryFogOffset ("Secondary Offset", Float) = 1.0
      //  _GradientStrength("Gradient Strength", Float) = 0.7
      //  _FogScattering("Fog Scattering", Float) = 1.0
      //  _SkyBoxNoiseTransparency("Sky Box Noise Transparency", Float) = 1.0
      //  _RotateFogNoise ("Rotate Fog Noise", Float) = 1.0
      //  _RotateSkyBoxNoise ("Rotate Sky Box Noise", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Overlay" "Queue" = "Overlay" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1; // For volumetric fog
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _FogNoiseScale;
            float _FogNoiseVelocity;
            float _SkyBoxNoiseScale;
            float _SkyBoxNoiseVelocity;
            sampler2D _CameraDepthTexture;
            float4 _PFogColor;
            float4 _SFogColor;
            float4 _SkyBoxFogColor;
            float _FogDensity;
            float _SkyBoxFogDensity;
            float _PrimaryFogColorOffset;
            float _SecondaryFogColorOffset;
            float _FogOffset;
            float _GradientStrength;
            float _FogScattering;
            float _SkyBoxNoiseTransparency;
            float _RotateFogNoise;
            float _RotateSkyBoxNoise;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = v.vertex.xyz;
                return o;
            }

        float2 RotateUV(float2 uv, float angle) {
            float cosAngle = cos(angle);
            float sinAngle = sin(angle);
            
            // Rotation matrix
            float2 rotatedUV;
            rotatedUV.x = uv.x * cosAngle - uv.y * sinAngle;
            rotatedUV.y = uv.x * sinAngle + uv.y * cosAngle;
    
            return rotatedUV;
        }
        float4 frag (v2f i) : SV_Target
        {
            float2 rotatedUV1 = RotateUV(i.uv, _RotateFogNoise);
            float2 rotatedUV2 = RotateUV(i.uv, _RotateSkyBoxNoise);

            // Sample the main texture color
            float4 sceneColor = tex2D(_MainTex, i.uv);
    
            // Sample noise for volumetric fog
            float noiseValue = tex2D(_NoiseTex, rotatedUV1 * _FogNoiseScale + (_Time.y * _FogNoiseVelocity)).r; 

            // Sample and linearize the depth
            float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
            depth = Linear01Depth(depth);

            // Calculate view distance
            float viewDistance = depth * _ProjectionParams.z;

            // Calculate fog factor
            float fogFactor = (_FogDensity / sqrt(log(2))) * max(0.0f, viewDistance - _FogOffset);
            fogFactor = exp2(-fogFactor * fogFactor);

            // Add fog effect
            float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            fogFactor *= lerp(1, noiseValue, _FogScattering); 

            // Calculate distance factor for color interpolation
            float distanceFactor = pow(saturate((viewDistance - _PrimaryFogColorOffset) / _SecondaryFogColorOffset), _GradientStrength);

            // Interpolate between primary and secondary fog colors
            float4 finalFogColor = lerp(_PFogColor, _SFogColor, distanceFactor);
            if (_GradientStrength == 0.0f) {
                finalFogColor = _PFogColor;
            }

    
            if (depth >= 1) {
                // Apply noise to the gradient interpolation
                float skyboxNoise = tex2D(_NoiseTex, rotatedUV2 * _SkyBoxNoiseScale + (_Time.y * _SkyBoxNoiseVelocity)).r; // Sample noise for skybox blending
                float noiseInfluence = lerp(1.0, skyboxNoise, _SkyBoxNoiseTransparency); // Influence of noise on the skybox gradient
                finalFogColor = lerp(finalFogColor, _SkyBoxFogColor, noiseInfluence);
                finalFogColor = lerp(sceneColor, finalFogColor, _SkyBoxFogDensity);
             }

             // Final color blending
             float4 finalColor = lerp(finalFogColor, sceneColor, saturate(fogFactor));

             return finalColor;
         }

        ENDHLSL
      }
   }
    FallBack "Diffuse"
}
#ifndef GRASSBLADES_INCLUDED
#define GRASSBLADES_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "NMGGraphicsHelpers.hlsl"

// Describes vertex on the new generated mesh (By compute shader)
struct DrawVertex
{
    float3 positionWS; // Position in World Space
    float height; // The height of the vertex on the grass blade
};

// A triangle on the generated mesh
struct DrawTriangle
{
    // A normal in world space to use for the lighting algorithm.
    // Will be parallel to grass to give semi-translucent effect
    float3 lightingNormalWS;
    
    DrawVertex vertices[3]; // The three points on each triangle
};

// Buffer containing generated mesh
StructuredBuffer<DrawTriangle> _DrawTriangles;

struct VertexOutput
{
    float uv : TEXCOORD0;           // Height of the vertex on the blade
    float3 positionWS : TEXCOORD1;  // Position in WS
    float3 normalWS : TEXCOORD2;    // Normal vector in WS

    float4 positionCS : SV_POSITION; // Position in CS
};

float4 _RootColor;
float4 _tipColor;

VertexOutput Vertex(uint vertexID : SV_VertexID)
{
    // Initialize output
    VertexOutput vOutput = (VertexOutput)0;

    // Get the vertex from the buffer
    DrawTriangle tri = _DrawTriangles[vertexID / 3];
    DrawVertex input = tri.vertices[vertexID % 3];

    vOutput.positionWS = input.positionWS;
    vOutput.normalWS = tri.lightingNormalWS;
    vOutput.uv = input.height;
    vOutput.positionCS = TransformWorldToHClip(input.positionWS);

    return vOutput;
}

// Properties
float4 _BaseColor;
float4 _TipColor;

half4 Fragment(VertexOutput input) : SV_Target
{
    // Get data for lighting
    InputData lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;
    lightingInput.normalWS = input.normalWS;
    lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
    lightingInput.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS);

    // Lerp the colors between the root and the height
    float colorLerp = input.uv;
    float3 albedo = lerp(_BaseColor.rgb, _TipColor.rgb, colorLerp);

    // Perform URP simple lit algorithm
    SurfaceData surfaceInput = (SurfaceData)0;
    surfaceInput.albedo = albedo;
    surfaceInput.alpha = 1;
    surfaceInput.specular = 1;
    surfaceInput.smoothness = 0.5;
    surfaceInput.occlusion = 1;
    return UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
}

#endif
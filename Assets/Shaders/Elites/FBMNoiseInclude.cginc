float Random2D_float(float2 UV)
{
    return frac(sin(dot(UV, float2(12.9898, 78.233))) * 43758.5453);
}

float Random3D_float(float3 UV)
{
    return frac(sin(dot(UV, float3(12.9898, 78.233, 45.543))) * 43758.5453);
}

float Random4D_float(float4 UV)
{
    return frac(sin(dot(UV, float4(12.9898, 78.233, 45.543, 94.654))) * 43758.5453);
}

float Noise2D_float(float2 UV)
{
    float2 i = floor(UV);
    float2 f = frac(UV);

    float a = Random2D_float(i);
    float b = Random2D_float(i + float2(1, 0));
    float c = Random2D_float(i + float2(0, 1));
    float d = Random2D_float(i + float2(1, 1));

    float2 u = f * f * (3.0 - 2.0 * f);

    return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

float Noise3D_float(float3 UV)
{
    float3 i = floor(UV);
    float3 j = frac(UV);

    float a = Random3D_float(i);
    float b = Random3D_float(i + float3(1, 0, 0));
    float c = Random3D_float(i + float3(0, 1, 0));
    float d = Random3D_float(i + float3(1, 1, 0));
    float e = Random3D_float(i + float3(0, 0, 1));
    float f = Random3D_float(i + float3(1, 0, 1));
    float g = Random3D_float(i + float3(0, 1, 1));
    float h = Random3D_float(i + float3(1, 1, 1));

    float a2 = lerp(a, b, j.x);
    float b2 = lerp(c, d, j.x);
    float c2 = lerp(e, f, j.x);
    float d2 = lerp(g, h, j.x);
    
    float a3 = lerp(a2, b2, j.y);
    float b3 = lerp(c2, d2, j.y);

    return lerp(a3, b3, j.z);
}

float Noise4D_float(float4 UV)
{
    float4 y = floor(UV);
    float4 z = frac(UV);

    float a = Random4D_float(y);
    float b = Random4D_float(y + float4(1, 0, 0, 0));
    float c = Random4D_float(y + float4(0, 1, 0, 0));
    float d = Random4D_float(y + float4(1, 1, 0, 0));
    float e = Random4D_float(y + float4(0, 0, 1, 0));
    float f = Random4D_float(y + float4(1, 0, 1, 0));
    float g = Random4D_float(y + float4(0, 1, 1, 0));
    float h = Random4D_float(y + float4(1, 1, 1, 0));
    float i = Random4D_float(y + float4(0, 0, 0, 1));
    float j = Random4D_float(y + float4(1, 0, 0, 1));
    float k = Random4D_float(y + float4(0, 1, 0, 1));
    float l = Random4D_float(y + float4(1, 1, 0, 1));
    float m = Random4D_float(y + float4(0, 0, 1, 1));
    float n = Random4D_float(y + float4(1, 0, 1, 1));
    float o = Random4D_float(y + float4(0, 1, 1, 1));
    float p = Random4D_float(y + float4(1, 1, 1, 1));

    float a2 = lerp(a, b, z.x);
    float b2 = lerp(c, d, z.x);
    float c2 = lerp(e, f, z.x);
    float d2 = lerp(g, h, z.x);
    float e2 = lerp(i, j, z.x);
    float f2 = lerp(k, l, z.x);
    float g2 = lerp(m, n, z.x);
    float h2 = lerp(o, p, z.x);

    float a3 = lerp(a2, b2, z.y);
    float b3 = lerp(c2, d2, z.y);
    float c3 = lerp(e2, f2, z.y);
    float d3 = lerp(g2, h2, z.y);
    
    float e3 = lerp(a3, b3, z.z);
    float f3 = lerp(c3, d3, z.z);

    return lerp(e3, f3, z.w);
}

void FBM2D_float(float2 UV, float Octaves, float Scale, out float Out)
{
    float Value = 0;
    float Amplitude = 0.5;
    UV *= Scale;

    for (int i = 0; i < Octaves; i++)
    {
        Value += Noise2D_float(UV) * Amplitude;
        UV *= 2;
        Amplitude *= 0.5;
    }
    Out = Value;
}

void FBM3D_float(float3 UV, float Octaves, float Scale, out float Out)
{
    float Value = 0;
    float Amplitude = 0.5;
    UV *= Scale;

    for (int i = 0; i < Octaves; i++)
    {
        Value += Noise3D_float(UV) * Amplitude;
        UV *= 2;
        Amplitude *= 0.5;
    }
    Out = Value;
}

void FBM4D_float(float4 UV, float Octaves, float Scale, out float Out)
{
    float Value = 0;
    float Amplitude = 0.5;
    UV *= Scale;

    for (int i = 0; i < Octaves; i++)
    {
        Value += Noise4D_float(UV) * Amplitude;
        UV *= 2;
        Amplitude *= 0.5;
    }
    Out = Value;
}
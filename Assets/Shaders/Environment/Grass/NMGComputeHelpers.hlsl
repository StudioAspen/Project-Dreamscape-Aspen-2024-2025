// Prevent duplicates of this file
#ifndef NMG_COMPUTE_HELPERS
#define NMG_COMPUTE_HELPERS

// Calculate the normal vector of a plane in world space!
// Points given must be in world space
void GetTriangleNormalAndTSToWSMatrix(float3 a, float3 b, float3 c, out float3 normalWS, out float3x3 tangentToWorld)
{
    // Calculate basis for tangent space
    // The tangent is the main direction of a surface starting at a point
    // In this case it is the x-direction from points a -> b
    float3 tangentWS = normalize(b-a);
    // The normal (z-direciton) is perpendicular to the lines formed by the triangle's points
    normalWS = normalize(cross(tangentWS, c-a));
    // The bitangent (y-direction) is perpendicular to the tangent and normal
    float3 bitangentWS = normalize(cross(tangentWS, normalWS));
    // tangent -> world rotation matrix
    tangentToWorld = transpose(float3x3(tangentWS, bitangentWS, normalWS));
}

// Returns the center point of a triangle
float3 GetTriangleCenter(float3 a, float3 b, float3 c)
{
    return (a + b + c) / 3.0;
}
float2 GetTriangleCenter(float2 a, float2 b, float2 c) {
    return (a + b + c) / 3.0;
}

// Return a pseudorandom number. By Ronja Böhringer
float rand(float4 seed){
    float4 r4 = frac(seed * float4(0.1031, 0.1030, 0.0973, 0.1099));
    r4 += dot(r4, r4.wzxy + 33.33);
    return frac((r4.x + r4.y) * (r4.z + r4.w));
}

float rand(float3 pos, float offset)
{
    return rand(float4(pos, offset));
}

float randNegative1to1(float3 pos, float offset)
{
    // * 2 - 1 essentially remaps it from 0 to 1 -> -1 to 1
    return rand(pos, offset) * 2 - 1;
}

// A function to compute an rotation matrix which rotates a point
// by angle radians around the given axis
// By Keijiro Takahashi
float3x3 AngleAxis3x3(float angle, float3 axis) {
    float c, s;
    sincos(angle, s, c);

    float t = 1 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return float3x3(
        t * x * x + c, t * x * y - s * z, t * x * z + s * y,
        t * x * y + s * z, t * y * y + c, t * y * z - s * x,
        t * x * z - s * y, t * y * z + s * x, t * z * z + c
        );
}

#endif
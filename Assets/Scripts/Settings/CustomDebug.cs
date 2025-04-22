using System.Drawing;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CustomDebug
{
#if UNITY_EDITOR
    private static int GIZMOS_SEGMENTS = 16;

    /// <summary>
    /// Draws a wireframe cone shape.
    /// </summary>
    public static void DrawWireCone(Vector3 center, Vector3 direction, float halfAngle, float maxDistance)
    {
        // Normalize the direction of the cone
        direction = direction.normalized;

        // Calculate the radius of the cone's base at maxDistance
        float radius = maxDistance * Mathf.Tan(halfAngle * Mathf.Deg2Rad);

        // Tip of the cone at the maxDistance along the direction vector
        Vector3 tip = center + direction * maxDistance;

        // Draw the line from the center to the tip of the cone (the cone's axis)
        Gizmos.DrawLine(center, tip);

        // Draw the base circle at maxDistance along the direction vector
        float angleStep = 360f / GIZMOS_SEGMENTS;
        Vector3 lastPoint = tip + Vector3.up * radius;  // Start from a point on the circle

        // Loop to draw the circle around the base of the cone
        for (int i = 1; i <= GIZMOS_SEGMENTS; i++)
        {
            float angle = angleStep * i;
            // Generate a point on the circle in the XZ plane at maxDistance
            Vector3 nextPoint = tip + Quaternion.AngleAxis(angle, direction) * Vector3.up * radius;
            Gizmos.DrawLine(lastPoint, nextPoint);
            Gizmos.DrawLine(center, nextPoint);  // Draw lines from center to the base
            lastPoint = nextPoint;
        }
    }

    /// <summary>
    /// Draws a wireframe capsule shape using Handles.
    /// </summary>
    public static void DrawWireCapsule(Vector3 point1, Vector3 point2, float radius)
    {
        Handles.color = Gizmos.color;

        Vector3 upOffset = point2 - point1;
        Vector3 up = upOffset.Equals(default) ? Vector3.up : upOffset.normalized;
        Quaternion orientation = Quaternion.FromToRotation(Vector3.up, up);
        Vector3 forward = orientation * Vector3.forward;
        Vector3 right = orientation * Vector3.right;
        // z axis
        Handles.DrawWireArc(point2, forward, right, 180, radius);
        Handles.DrawWireArc(point1, forward, right, -180, radius);
        Handles.DrawLine(point1 + right * radius, point2 + right * radius);
        Handles.DrawLine(point1 - right * radius, point2 - right * radius);
        // x axis
        Handles.DrawWireArc(point2, right, forward, -180, radius);
        Handles.DrawWireArc(point1, right, forward, 180, radius);
        Handles.DrawLine(point1 + forward * radius, point2 + forward * radius);
        Handles.DrawLine(point1 - forward * radius, point2 - forward * radius);
        // y axis
        Handles.DrawWireDisc(point2, up, radius);
        Handles.DrawWireDisc(point1, up, radius);
    }

    /// <summary>
    /// Draws a wireframe circle shape using Handles.
    /// </summary>
    public static void DrawWireCircle(Vector3 center, float radius)
    {
        Handles.color = Gizmos.color;

        // Draw the wire circle using Handles
        Handles.DrawWireArc(center, Vector3.up, Vector3.right, 360f, radius);
    }
#endif

    /// <summary>
    /// Instantiates a temporary sphere GameObject with the specified center, radius, expire duration, and color.
    /// The sphere is created as a primitive sphere with a Collider set as a trigger.
    /// The sphere's material is set to be transparent using the Universal Render Pipeline/Unlit shader.
    /// The sphere is destroyed after the specified expire duration.
    /// </summary>
    /// <param name="center">The center position of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <param name="expireDuration">The duration in seconds before the sphere is destroyed.</param>
    /// <param name="color">The color of the sphere.</param>
    public static GameObject InstantiateTemporarySphere(Vector3 center, float radius, float expireDuration, UnityEngine.Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.name = "Temporary Sphere";
        sphere.transform.position = center;
        sphere.transform.localScale = radius * 2f * Vector3.one;

        Renderer sphereRenderer = sphere.GetComponent<Renderer>();
        sphereRenderer.material = GetTransparentMaterial();
        sphereRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        sphereRenderer.material.color = color;
        GameObject.Destroy(sphere, expireDuration);

        return sphere;
    }

    /// <summary>
    /// Generates a transparent material.
    /// </summary>
    /// <returns>The transparent material.</returns>
    public static Material GetTransparentMaterial()
    {
        Material transparentMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

        // Change Surface Type to Transparent
        transparentMaterial.SetFloat("_Surface", 1); // 1 = Transparent, 0 = Opaque

        // Enable required shader keywords
        transparentMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        transparentMaterial.DisableKeyword("_SURFACE_TYPE_OPAQUE");

        // Set rendering mode for transparency
        transparentMaterial.SetOverrideTag("RenderType", "Transparent");
        transparentMaterial.SetInt("_ZWrite", 0); // Disable ZWrite for transparency
        transparentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        transparentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        // Apply the changes to the material
        transparentMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        return transparentMaterial;
    }
}

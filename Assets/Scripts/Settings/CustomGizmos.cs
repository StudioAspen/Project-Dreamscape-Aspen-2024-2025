using System.Drawing;
using UnityEditor;
using UnityEngine;

public static class CustomGizmos
{
    private static int SEGMENTS = 16;

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
        float angleStep = 360f / SEGMENTS;
        Vector3 lastPoint = tip + Vector3.up * radius;  // Start from a point on the circle

        // Loop to draw the circle around the base of the cone
        for (int i = 1; i <= SEGMENTS; i++)
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
}

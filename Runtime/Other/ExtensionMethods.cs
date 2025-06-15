using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class _ExtensionMethods
{
    /// <summary>
    /// Casts the Rigidbody in a direction to check for collision using SweepTest.
    /// </summary>
    /// <param name="rb">The Rigidbody in question.</param>
    /// <param name="direction">The direction the Rigidbody is going.</param>
    /// <param name="distance">The distance the Rigidbody is set to travel.</param>
    /// <param name="buffer">A buffer that the Rigidbody is temporarily moved backwards by before the Sweep Test.</param>
    /// <param name="hit">The resulting Hit.</param>
    /// <returns>Whether anything was Hit.</returns>
    public static bool DirectionCast(this Rigidbody rb, Vector3 direction, float distance, float buffer, out RaycastHit hit)
    {
        rb.MovePosition(rb.position - direction * buffer);
        bool result = rb.SweepTest(direction.normalized, out hit, distance + buffer, QueryTriggerInteraction.Ignore);
        rb.MovePosition(rb.position + direction * buffer);
        hit.distance -= buffer;
        return result;
    }
    /// <summary>
    /// Casts the Rigidbody in a direction to check for collision using SweepTest. (Returns Multiple.)
    /// </summary>
    /// <param name="rb">The Rigidbody in question.</param>
    /// <param name="direction">The direction the Rigidbody is going.</param>
    /// <param name="distance">The distance the Rigidbody is set to travel.</param>
    /// <param name="buffer">A buffer that the Rigidbody is temporarily moved backwards by before the Sweep Test.</param>
    /// <param name="hit">The resulting Hits.</param>
    /// <returns>Whether anything was Hit.</returns>
    public static bool DirectionCastAll(this Rigidbody rb, Vector3 direction, float distance, float buffer, out RaycastHit[] hit)
    {
        rb.MovePosition(rb.position - direction * buffer);
        hit = rb.SweepTestAll(direction.normalized, distance + buffer, QueryTriggerInteraction.Ignore);
        rb.MovePosition(rb.position + direction * buffer);
        hit[0].distance -= buffer;
        return hit.Length > 0;
    }



    public static float Abs(this float value) => Mathf.Abs(value);
    public static Vector3 XY(this Vector3 v) => new(v.x, v.y, 0f);
    public static Vector3 XZ(this Vector3 v) => new(v.x, 0f, v.z);
    public static Vector3 YZ(this Vector3 v) => new(0f, v.y, v.z);

    public static Vector3 ProjectAndScale(this Vector3 value, Vector3 normal) => Vector3.ProjectOnPlane(value, normal).normalized * value.magnitude;


}

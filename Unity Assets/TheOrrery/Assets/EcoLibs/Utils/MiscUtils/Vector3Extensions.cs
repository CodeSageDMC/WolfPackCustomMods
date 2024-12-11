// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.Math;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public static class Vector3Extentions
{
    /// <summary> Shortcut for <see cref="Vector3.Distance(Vector3, Vector3)"/> </summary>
    public static float Distance(this Vector3 vec, Vector3 otherVec) => Vector3.Distance(otherVec, vec);

    /// <summary>
    /// Rounds vector to int, using default unity <see cref="Mathf.RoundToInt"/> method (to even or bank rounding).
    /// I.e. -0.5 -> 0; 0.5 -> 0; 1.5 -> 2.
    /// It means it may round either down or up for half number!
    /// Warning! Avoid using this method for block positions, because it may produce unpredictable results. Use RoundUp instead.
    /// </summary>
    public static Vector3i RoundToEven(this Vector3 vec) =>
        new Vector3i(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));

    /// <summary>
    /// Rounds vector to int, using default unity <see cref="Mathf.RoundToInt"/> method (to even or bank rounding).
    /// I.e. -0.5 -> 0; 0.5 -> 0; 1.5 -> 2.
    /// It means it may round either down or up for half number!
    /// Warning! Avoid using this method for block positions, because it may produce unpredictable results. Use RoundUp instead.
    /// </summary>
    public static Vector2i RoundToEven(this Vector2 vec) =>
        new Vector2i(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));

    /// <summary>
    /// Rounds vector to int, using round up method (always rounds half number to higher closest number).
    /// I.e. -0.5 -> 0; 0.5 -> 1; 1.5 -> 2.
    /// Used mostly for accurate block positions.
    /// </summary>
    public static Vector3i RoundUp(this Vector3 vec) =>
        new Vector3i(Eco.Shared.Mathf.RoundUpToInt(vec.x), Eco.Shared.Mathf.RoundUpToInt(vec.y), Eco.Shared.Mathf.RoundUpToInt(vec.z));

    /// <summary> Calculate square of distance (much cheaper than distance calculation). May be used for estimation when exact value doesn't matter. </summary>
    public static float SqrDistance(this Vector3 a, Vector3 b)
    {
        var dx = a.x - b.x;
        var dy = a.y - b.y;
        var dz = a.z - b.z;
        return dx * dx + dy * dy + dz * dz;
    }

    /// <summary>Clamps vector in range of two points </summary>
    public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
    {
        var x = Mathf.Clamp(value.x, min.x, max.x);
        var y = Mathf.Clamp(value.y, min.y, max.y);
        var z = Mathf.Clamp(value.z, min.z, max.z);
        return new Vector3(x, y, z);
    }
    
    public static Vector3 Scaled(this Vector3 vector, Vector3 scale) => Vector3.Scale(vector, scale);

    public static Vector3 Rotated(this Vector3 vector, Quaternion rotation) => rotation * vector;

    public static Vector3 Translated(this Vector3 vector, Vector3 translation) => translation + vector;

    /// <summary> A variant of <see cref="TransformPoint(Vector3, Vector3, Quaternion, Vector3)"/> </summary>
    public static Vector3 TransformPoint(this Vector3 point, Vector3 position, Vector3 rotation, Vector3 scale) =>
        TransformPoint(point, position, Quaternion.Euler(rotation), scale);

    /// <summary>
    /// Similar to <see cref="Transform.TransformPoint(Vector3)"/> but doesn't need a transform,
    /// just pass the expected <see cref="position"/>, <see cref="rotation"/> and <see cref="scale"/> of that transform.
    /// </summary>
    public static Vector3 TransformPoint(this Vector3 point, Vector3 position, Quaternion rotation, Vector3 scale) =>
        point.Scaled(scale).Rotated(rotation).Translated(position);
}
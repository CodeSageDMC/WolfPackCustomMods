// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public static class VectorUtil
{
    public static bool IsReal(this Vector3 vec) => vec.x.IsReal() && vec.y.IsReal() && vec.z.IsReal();
    public static bool IsReal(this Eco.Shared.Math.Vector2 vec) => vec.x.IsReal() && vec.y.IsReal();

    public static Vector3[] Convert(this IEnumerable<Vector3> list) => list.ToArray();

    public static Vector3 SetAxis(this Vector3 vec, Axis a, float val)
    {
        switch(a) 
        {  
            case Axis.X: vec.x = val; return vec;
            case Axis.Y: vec.y = val; return vec;
            case Axis.Z: vec.z = val; return vec;
        }
        throw new System.Exception("Invalid axis.");
    }

    public static Vector3 AddToAxis(this Vector3 vec, Axis a, float val)
    {
        switch (a)
        {
            case Axis.X: vec.x += val; return vec;
            case Axis.Y: vec.y += val; return vec;
            case Axis.Z: vec.z += val; return vec;
        }
        throw new System.Exception("Invalid axis.");
    }


    public static Vector3 ToVec(this Axis a)
    {
        switch (a)
        {
            case Axis.X: return Vector3.right;
            case Axis.Y: return Vector3.up;
            case Axis.Z: return Vector3.forward;
        }
        throw new System.Exception("Invalid axis.");
    }


    public static float GetAxis(this Vector3 vec, Axis a)
    {
        switch (a)
        {
            case Axis.X: return vec.x;
            case Axis.Y: return vec.y;
            case Axis.Z: return vec.z;
        }
        throw new System.Exception("Invalid axis");
    }

    public static float GetAxis(this Vector2 vec, Axis a)
    {
        switch (a)
        {
            case Axis.X: return vec.x;
            case Axis.Y: return vec.y;
        }
        throw new System.Exception("Invalid axis");
    }

    public static void SetAxis(this Vector2 vec, Axis a, float val)
    {
        switch (a)
        {
            case Axis.X: vec.x = val; break;
            case Axis.Y: vec.y = val; break;
        }
    }

    /// <summary>
    /// Same as Vector3.Lerp, but also checks for every component which one is lower and always uses lowest value as a range start value.
    /// If doesn't clamp <c>t</c> to [0; 1], so t should already be clamped.
    /// </summary>
    /// <param name="a">first vector</param>
    /// <param name="b">second vector</param>
    /// <param name="t"></param>
    /// <returns>Linearly interpolated value</returns>
    public static Vector3 NonDirectionalLerpUnclamped(Vector3 a, Vector3 b, float t)
    {
        var x = b.x > a.x ? a.x + (b.x - a.x) * t : b.x + (a.x - b.x) * t;
        var y = b.y > a.y ? a.y + (b.y - a.y) * t : b.y + (a.y - b.y) * t;
        var z = b.z > a.z ? a.z + (b.z - a.z) * t : b.z + (a.z - b.z) * t;
        return new Vector3(x, y, z);
    }

    /// <summary> Compares two <see cref="Vector3"/> values and returns true if they are similar by <paramref name="tolerance"/> </summary>
    public static bool RoughlyEquals(this Vector3 a, Vector3 b, float tolerance)    => Math.Abs(a.x - b.x) < tolerance && Math.Abs(a.y - b.y) < tolerance && Math.Abs(a.z - b.z) < tolerance;

    /// <summary> Uses <see cref="RoughlyEquals"/> with tolerance of <see cref="UnityEngine.Mathf.Epsilon"/> </summary>
    public static bool RoughlyEqualsEps(this Vector3 a, Vector3 b)                  => a.RoughlyEquals(b, UnityEngine.Mathf.Epsilon);

    ///<summary>Rotate a vector around the Y axis in clockwise direction.</summary>
    public static Vector3 RotateAroundYClockwise(this Vector3 vec, float degrees)
    {
        var radians = degrees * Mathf.Deg2Rad;
        var x       = vec.z * Mathf.Sin(radians) + vec.x * Mathf.Cos(radians);
        var y       = vec.y;
        var z       = vec.z * Mathf.Cos(radians) - vec.x * Mathf.Sin(radians);
        return new Vector3(x, y, z);
    }
    
    /// Taken from https://stackoverflow.com/questions/51905268/how-to-find-closest-point-on-line
    /// <summary> Given a point and a line, it returns the closest point on the line to the reference point</summary>
    public static Vector3 GetClosestPointOnInfiniteLine(Vector3 point, Vector3 line_start, Vector3 line_end) => line_start + Vector3.Project(point - line_start, line_end - line_start);

    /// <summary> Get the direction from <paramref name="self"/> position towards <paramref name="other"/>. </summary>
    public static Vector3 DirectionToOther(this Vector3 self, Vector3 other, bool ignoreHeight)
    {
        var toOther = (other - self).normalized;
        return ignoreHeight ? toOther.WithY(0f) : toOther;
    }
}

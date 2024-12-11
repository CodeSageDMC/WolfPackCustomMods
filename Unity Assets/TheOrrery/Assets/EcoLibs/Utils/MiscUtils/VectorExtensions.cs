// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.Math;

using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public static class VectorExtensions
{
    public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);
    public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);

    public static Vector2i XZi(this Vector3 v) => new Vector2i((int) v.x, (int) v.z);

    public static Vector3 X_Z(this Vector2 v, float y = 0f) => new Vector3(v.x, y, v.y);
    public static Vector3 XY_(this Vector2 v, float z = 0f) => new Vector3(v.x, v.y, z);

    public static Vector3 X_Z(this Vector2? v, float y = 0f) => new Vector3(v.Value.x, y, v.Value.y);
    public static Vector3 XY_(this Vector2? v, float z = 0f) => new Vector3(v.Value.x, v.Value.y, z);

    public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
    public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
    public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

    public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);
    public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);


    // Since there's no 'Vector3.MoveTowardsAngle', and it'd be confusing to operate on all 3 components separately with the same maxDelta, we need an extension for each one of its components.
    public static Vector3 MoveTowardsAngleX(this Vector3 from, float targetX, float maxDelta) => from.WithX(Mathf.MoveTowardsAngle(from.x, targetX, maxDelta));
    public static Vector3 MoveTowardsAngleY(this Vector3 from, float targetY, float maxDelta) => from.WithY(Mathf.MoveTowardsAngle(from.y, targetY, maxDelta));
    public static Vector3 MoveTowardsAngleZ(this Vector3 from, float targetZ, float maxDelta) => from.WithZ(Mathf.MoveTowardsAngle(from.z, targetZ, maxDelta));

    public static Vector3 Invert(this Vector3 v) => new Vector3(1f / v.x, 1f / v.y, 1f / v.z);
}

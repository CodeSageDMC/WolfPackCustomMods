// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public static class GizmosUtils
{
    /// <summary> Gizmo matrix to local transform space </summary>
    /// gizmo rotation, position and scale will replicate its transform
    public static void ToLocalMatrix(Transform trans, Vector3? scale = null) => Gizmos.matrix = Matrix4x4.TRS(trans.position, trans.rotation, scale ?? Vector3.one);

    /// <summary> Draw wire cube from origin, not centralized </summary>
    public static void DrawWireCubeUpFromOrigin(Vector3 size, float yOffset = 0f, Color? color = null)
    {
        if (color.HasValue) Gizmos.color = color.Value;
        Gizmos.DrawWireCube(Vector3.up * (yOffset + size.y * 0.5f), size);
    }
    /// <summary> Draws a wire cube at a given position given a Matrix. This allows it to be rotate or scaled.</summary>
    public static void DrawWireCubeWithMatrix(Vector3 position, Quaternion rotation, Vector3 size, Vector3? scale = null)
    {
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale ?? Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
    }

    /// <summary> Draws a cube mesh at a given position given a Matrix. This allows it to be rotate or scaled.</summary>
    public static void DrawCubeWithMatrix(Vector3 position, Quaternion rotation, Vector3 size, Vector3? scale = null)
    {
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale ?? Vector3.one);
        Gizmos.DrawCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
    }

    /// <summary> Does a <see cref="Physics.CheckBox"/> to check for any collisions, and sets it's correspondant <see cref="Gizmos.color"/> 
    /// then it calls a give drawFunction. For example <see cref="DrawCubeWithMatrix"/> or <see cref="DrawWireCubeWithMatrix"/>.
    /// </summary>
    public static void CheckCubeCollisionAndDraw(Vector3 position, Quaternion rotation, Vector3 size, Color noCollision, Color collisionColor, int layerMask, System.Action<Vector3, Quaternion, Vector3, Vector3?> drawFunc)
    {
        Gizmos.color = Physics.CheckBox(position, size * 0.5f, rotation, layerMask) ? collisionColor : noCollision;
        drawFunc?.Invoke(position, rotation, size, null);
    }
    /// <summary>Does a <see cref="Physics.Raycast"/> to check for any collisions, and sets it's correspondant <see cref="Gizmos.color"/>.</summary>
    public static void RaycastAndDraw(Ray ray, float distance, Color noCollision, Color collisionColor)
    {
        Gizmos.color = Physics.Raycast(ray, distance) ? collisionColor : noCollision;
        Gizmos.DrawRay(ray.origin, ray.direction * distance);
    }

    /// <summary>Does a <see cref="Physics.Raycast"/> to check for any collisions, and sets it's correspondant <see cref="Gizmos.color"/>.</summary>
    public static void RaycastAndDraw(Ray ray, Color noCollision, Color collisionColor)
    {
        Gizmos.color = Physics.Raycast(ray) ? collisionColor : noCollision;
        Gizmos.DrawRay(ray);
    }

    /// <summary>Does a <see cref="Physics.Raycast"/> to check for any collisions, and sets it's correspondant <see cref="Gizmos.color"/>.</summary>
    public static void RaycastAndDraw(Ray ray, float distance, int layerMask, Color noCollision, Color collisionColor)
    {
        Gizmos.color = Physics.Raycast(ray, distance, layerMask) ? collisionColor : noCollision;
        Gizmos.DrawRay(ray.origin, ray.direction * distance);
    }

    /// <summary>
    /// Draws a visual representation of <see cref="Physics.BoxCast(Vector3, Vector3, Vector3, Quaternion, float)"/> and sets the Gizmos Color depending
    /// on whether or not it hit something.
    /// </summary>
    public static void DrawBoxCast(Vector3 origin, Vector3 size, Vector3 direction, Quaternion orientation, float distance, int layerMask, Color noCollision, Color collisionColor)
    {
        Gizmos.color     = Physics.BoxCast(origin, size, direction, orientation, distance, layerMask) ? collisionColor : noCollision;
        var dir          = direction * distance;
        var cubeVextexes = GetCubeVertexes(origin, size, orientation);
        //draws rays for each vertex of the initial cube
        for (int i = 0; i < cubeVextexes.Length; i++) Gizmos.DrawRay(cubeVextexes[i], dir);
       
        //draws starter cube
        Gizmos.matrix = Matrix4x4.TRS(origin, orientation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size * 2f);
        //draws end cube
        Gizmos.matrix = Matrix4x4.TRS(origin + (direction * distance), orientation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size * 2f);
    }

    /// <summary>Returns an array of Vectors that represent a cube given an origin, size and rotation</summary>
    static Vector3[] GetCubeVertexes(Vector3 origin, Vector3 size, Quaternion orientation)
    {
        return new Vector3[]
        {
         origin + (orientation * ( size)),
         origin + (orientation * (-size)),
         origin + (orientation * (new Vector3( size.x,  size.y,-size.z))),
         origin + (orientation * (new Vector3( size.x, -size.y, size.z))),
         origin + (orientation * (new Vector3( size.x, -size.y,-size.z))),
         origin + (orientation * (new Vector3(-size.x,  size.y, size.z))),
         origin + (orientation * (new Vector3(-size.x,  size.y,-size.z))),
         origin + (orientation * (new Vector3(-size.x, -size.y, size.z)))
        };
    }

    /// <summary> Draws a representation of <see cref="Physics.SphereCast"/></summary>
    public static void DrawSphereCast(Ray ray, float radius, float distance)
    {
        // Sets Gizmos matrix to a rotated version according to the ray's direction
        var lookRotation = Quaternion.LookRotation(ray.direction);
        var matrix = Matrix4x4.Rotate(lookRotation);

        // Draws sphere at the start of the ray and the end of it
        Gizmos.matrix = Matrix4x4.TRS(ray.origin, lookRotation, Vector3.one);
        Gizmos.DrawWireSphere(Vector3.zero, radius);

        Gizmos.matrix = Matrix4x4.TRS(ray.origin + ray.direction * distance, lookRotation, Vector3.one);
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        
        Gizmos.matrix = Matrix4x4.identity;
        
        // Draws 9 rays around the sphereCast UP and RIGHT vectors, making it "cilinder"
        for (int i = -1; i <= 1; i++)    // -1, 0 , 1
        for (int u = -1; u <= 1; u++)    // -1, 0 , 1
        {
            var offset = (Vector3)(matrix * (new Vector3(i, u, 0)).normalized) * radius; // Gets offset from ray's origin where the new ray will be drawn.
            Gizmos.DrawRay(ray.origin + offset, ray.direction * distance);
        }
    }

    /// <summary> Draws a representation of <see cref="Physics.SphereCast"/></summary>
    public static void DrawSphereCast(Vector3 startPos, Vector3 endPos, float radius) => DrawSphereCast(new Ray(startPos, endPos - startPos), radius, Vector3.Distance(startPos, endPos));

    /// <summary> Draws a representation of <see cref="Physics.SphereCastAll"/> and it's hit positions</summary>
    public static void DrawSphereCastAll(Ray ray, float radius, float distance, int layerMask, QueryTriggerInteraction interactionType)
    {
        var hits = Physics.SphereCastAll(ray, radius, distance, layerMask, interactionType);          // calculates the raycast hits
        DrawSphereCast(ray, radius, distance);                                                        // Draws a representation of the sphereCast
        foreach (var hit in hits) Gizmos.DrawWireSphere(hit.point, 0.01f);                            // Draws sphere at the sphereCast hits if any
    }

    /// <summary> Draws a representation of <see cref="Physics.SphereCastAll"/> and it's hit positions</summary>
    public static void DrawSphereCastAll(Vector3 startPos, Vector3 endPos, float radius, float distance, int layerMask, QueryTriggerInteraction interactionType) => DrawSphereCastAll(new Ray(startPos, endPos - startPos), radius, Vector3.Distance(startPos, endPos), layerMask, interactionType);

    public static void DrawPlane(Vector3 center, float width, float height, Color color)
    {
        Gizmos.color = color;

        // Calculate the corners of the plane
        Vector3 topLeft     = center + new Vector3(-width / 2, 0, height / 2);
        Vector3 topRight    = center + new Vector3(width / 2, 0, height / 2);
        Vector3 bottomLeft  = center + new Vector3(-width / 2, 0, -height / 2);
        Vector3 bottomRight = center + new Vector3(width / 2, 0, -height / 2);

        // Draw the plane using lines
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Optionally fill in the plane with lines
        int numLines = 10;
        for (int i = 0; i <= numLines; i++)
        {
            float t = (float)i / numLines;
            Vector3 start = Vector3.Lerp(topLeft, topRight, t);
            Vector3 end = Vector3.Lerp(bottomLeft, bottomRight, t);
            Gizmos.DrawLine(start, end);
        }
    }
}

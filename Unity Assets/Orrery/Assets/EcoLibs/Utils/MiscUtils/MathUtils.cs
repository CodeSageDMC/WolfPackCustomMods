// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///<summary>
///Contains general mathematics functions.
///</summary>
public static class MathUtils
{
    public static float CalculateHyperbolicCosine(float x) => (Mathf.Exp(x) + Mathf.Exp(-x)) / 2f;

    /// <summary>Get distance from point to line defined by a ray. Fast implementation.</summary>
    /// Uses a math trick to fast get the distance. See https://www.youtube.com/watch?v=tYUtWYGUqgw for visual explanation.
    public static float DistanceFromPointToRay(Ray ray, Vector3 point) => Vector3.Cross(ray.direction, point - ray.origin).magnitude;

    /// <summary>Check if ray intersects the given sphere and calculated two intersection points d1 and d2, described in a form of a distance from ray origin.</summary>
    public static bool RaySphereIntersection(Ray ray, Vector3 sphereCenter, float sphereRadius, out float d1, out float d2)
    {
        //This is improved implementation of this method: http://kylehalladay.com/blog/tutorial/math/2013/12/24/Ray-Sphere-Intersection.html
        d1 = 0f;
        d2 = 0f;

        var distRaySphere = DistanceFromPointToRay(ray, sphereCenter);
        if (distRaySphere > sphereRadius) //if distance from ray to sphere > radius -> there's no intersection, nothing to calculate
            return false;

        var intersectionCenterDist = Mathf.Sqrt(sphereRadius * sphereRadius + distRaySphere * distRaySphere); //This is the 1/2 length of the line segment inside the sphere

        var sphereProjectedCenterDist = Vector3.Dot(sphereCenter - ray.origin, ray.direction); //Projection of the sphere center to ray. Uses the fact direction is normalized.

        //Now, just get near and far distances
        d1 = intersectionCenterDist - sphereProjectedCenterDist;
        d2 = intersectionCenterDist + sphereProjectedCenterDist;

        return true;
    }

    /// <summary> Gets closest transform object for target position check. Uses simple distance comparator. </summary>
    public static Transform GetClosestTransform(this IEnumerable<Transform> list, Vector3 targetPos, Transform defaultResult = null)
    {
        if (list == null || !list.Any()) return defaultResult;
        
        Transform closest = null;
        var lastDistance = Mathf.Infinity;
        foreach (var point in list)
        {
            var dist = Vector3.Distance(point.position, targetPos);
            if (dist < lastDistance)
            {
                lastDistance = dist;
                closest = point;
            }
        }

        if (closest == null) closest = defaultResult;
        return closest;
    }
    
    /// <summary> Percentage to float range convert formula: value = ((max - min) * percentage) + min </summary>
    public static float PercentageToRangeFloat(float percent, float min, float max) => ((max - min) * percent) + min;
    
    /// <summary> Percentage to int range convert formula: value = ((max - min) * percentage) + min </summary>
    public static int PercentageToRangeInt(float percent, int min, int max) => Mathf.RoundToInt(PercentageToRangeFloat(percent, min, max));

    /// <summary>
    /// Decompose quaternion on swing and twist rotation on given axis.
    /// Benefits:
    /// - Unlike FromToRotation does not lose the twist angle
    /// - Could limit swing and twist rotations (rotation clamping)
    /// 
    /// Original: https://github.com/TheAllenChou/unity-cj-lib/blob/master/Unity%20CJ%20Lib/Assets/CjLib/Script/Math/QuaternionUtil.cs
    /// </summary>
    public static void DecomposeSwingTwist(Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
    {
        var r = new Vector3(q.x, q.y, q.z); // (rotation axis) * cos(angle / 2)

        //singularity: rotation by 180 degree
        if (r.sqrMagnitude < Mathf.Epsilon)
        {
            var rotatedTwistAxis = q * twistAxis;
            var swingAxis = Vector3.Cross(twistAxis, rotatedTwistAxis);

            if (swingAxis.sqrMagnitude > Mathf.Epsilon)
            {
                var swingAngle = Vector3.Angle(twistAxis, rotatedTwistAxis);
                swing = Quaternion.AngleAxis(swingAngle, swingAxis);
            }
            else
            {
                //more singularity: rotation axis parallel to twist axis
                swing = Quaternion.identity; //no swing
            }

            //always twist 180 degree on singularity
            twist = Quaternion.AngleAxis(180f, twistAxis);
            return;
        }

        // formula & proof: 
        // http://www.euclideanspace.com/maths/geometry/rotations/for/decomposition/
        var p = Vector3.Project(r, twistAxis);
        twist = new Quaternion(p.x, p.y, p.z, q.w);
        twist = twist.normalized;
        swing = q * Quaternion.Inverse(twist);
    }
}

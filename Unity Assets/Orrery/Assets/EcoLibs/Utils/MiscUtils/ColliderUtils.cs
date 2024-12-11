// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;

public static class ColliderUtils
{
    private static Collider[] colliderTest = new Collider[20];

    static Collider BlocksCollider(Vector3 center, Vector3 halfExtents, Quaternion rot, int layerMask, HashSet<Collider> ignoreColliders)
    {
        var num = Physics.OverlapBoxNonAlloc(center, halfExtents, colliderTest, rot, layerMask);
        for (int i = 0; i < num; i++)
            if ((ignoreColliders == null || !ignoreColliders.Contains(colliderTest[i]) && !colliderTest[i].isTrigger))
                return colliderTest[i];
        return null;
    }

    public static bool TouchesCollider(this WorldRange range, int layerMask, HashSet<Collider> ignoreColliders, out Collider overlapCollider)
    {
        var center      = range.Center - new Vector3(.5f, .5f, .5f); //Offset it, because .5.5.5 is the center of a block, but we want the corner for the collider test.
        var halfExtents = range.Size * 0.5f;
        overlapCollider = BlocksCollider(center, halfExtents, Quaternion.identity, layerMask, ignoreColliders);
        return overlapCollider != null;
    }

    public static Vector3 RandomPointInside(this BoxCollider box)
    {
        var boxSize = box.size;
        return box.transform.TransformPoint(box.center + new Vector3((Random.value - 0.5f) * boxSize.x, (Random.value - 0.5f) * boxSize.y, (Random.value - 0.5f) * boxSize.z));
    }

    /// <summary>
    /// Calculates compound collider bounds which contains bounds of all active colliders within <paramref name="gameObject"/>.
    /// If no colliders then it will return default bounds (zero size at zero point).
    /// </summary>
    public static Bounds GetCompoundColliderBounds(this GameObject gameObject)
    {
        var list = TempLists.Rent<Collider>();
        try
        {
            gameObject.GetComponentsInChildren(list);
            return list.GetCompoundColliderBounds();
        }
        finally
        {
            TempLists.Return(list);
        }
    }
    
    /// <summary>
    /// Calculates compound collider bounds which contains bounds of all colliders in <see cref="list"/>.
    /// If no colliders then it will return default bounds (zero size at zero point).
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public static Bounds GetCompoundColliderBounds(this List<Collider> list)
    {
        using (var enumerator = list.GetEnumerator())
        {
            if (!enumerator.MoveNext())
                return default;

            var bounds = enumerator.Current.bounds;
            while (enumerator.MoveNext())
                bounds.Encapsulate(enumerator.Current.bounds);
            return bounds;
        }
    }

    /// <summary>
    /// Get start and end of a capsule collider without having to pass the instance. Useful when needing to check for multiple poses
    /// but don't want to actually move the collider.
    /// If <paramref name="fullHeight"/> is true, we will use the full height of the capsule; otherwise, we will subtract the radius from the spheres at the limits of the capsule 
    /// </summary>
    /// <remarks>Only works for capsules with default value of direction.</remarks>
    public static (Vector3 start, Vector3 end) GetCapsuleStartEnd(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 center, float radius, float height, bool fullHeight = false)
    {
        // if height doesn't greater than diameter, the capsule is actually just a sphere.
        if (height <= radius * 2)
        {
            var startEnd = center.TransformPoint(position, rotation, scale);
            return (startEnd, startEnd);
        }
		
        ///If the capsule is long enough to be a normal one:
        ///If <param name="fullHeight"/> is false, subtract height by radius twice to get the height of the cylinder inside the capsule.
        ///Otherwise, just get the height of the capsule.
        var halfHeight = fullHeight ? (height / 2f) : ((height - radius * 2) / 2);
        // start and end are the top and bottom centers of the circle on top and bottom of cylinder
        var start = center.WithY(center.y - halfHeight).TransformPoint(position, rotation, scale);
        var end   = center.WithY(center.y + halfHeight).TransformPoint(position, rotation, scale);
        return (start, end);
    }

    /// <summary>
    /// Get positions of capsule's start and end (center of the 2 spheres on capsule's 2 ends) in world space.
    /// Useful when using with <see cref="Physics.CheckCapsule(Vector3,Vector3,float)"/> or <see cref="Physics.CapsuleCast(Vector3,Vector3,float,Vector3)"/>.
    /// </summary>
    public static (Vector3 start, Vector3 end) GetCapsuleStartEnd(this CapsuleCollider capsule)
    {
        var transform      = capsule.transform;
        var direction      = capsule.GetDirectionVector();           //Find vector representing direction of the capsule in local space
        var startEndOffset = (capsule.height / 2f) + capsule.radius; //Calculate offset of start and end coordinates from center of the capsule

        //Calculate start and end (centers of the spheres) coordinates in local space
        var start = capsule.center - direction * startEndOffset;
        var end   = capsule.center + direction * startEndOffset;

        //Transform to world space
        start = transform.TransformPoint(start);
        end   = transform.TransformPoint(end);

        return (start, end);
    }

    /// <summary>Returns a vector representing direction of the capsule in local space.</summary>
    public static Vector3 GetDirectionVector(this CapsuleCollider capsule) => capsule.direction switch
    {
        0 => Vector3.right,
        1 => Vector3.up,
        2 => Vector3.forward,
        _ => throw new Exception($"Capsule collider has invalid value of direction: {capsule.direction}. Only valid values are 0, 1 and 2."),
    };

    /// <summary>Returns what collider a given collider touches, with an option to ignore colliders under a given gameobject.
    /// Ingores chunks.</summary>
    public static IEnumerable<(Transform TouchingObj, Vector3 Direction)> GetTouchingColliders(this Collider collider, Func<GameObject, bool> ignore, int layerMask)
    {
        Collider[] allColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation, layerMask);
        foreach (Collider otherCollider in allColliders)
        {
            if (otherCollider.gameObject.HasAncestor(collider.gameObject))  continue;
            if (ignore?.Invoke(otherCollider.gameObject) == true)           continue;

            if (Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation,
                                            otherCollider, otherCollider.transform.position, otherCollider.transform.rotation,
                                            out var direction, out var distance))
                yield return (otherCollider.transform, direction);
        }
    }
}

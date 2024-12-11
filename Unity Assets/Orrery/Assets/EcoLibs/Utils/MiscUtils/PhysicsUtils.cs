// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PhysicsUtils
{
    public static RaycastHit[]  ReusableHits        = new RaycastHit[10];
    public static Collider[]    ReusableColliders   = new Collider[10];

    public static bool ContactedBy(this Collision collision, Collider collider)
    {
        for (var i = 0; i < collision.contactCount; i++)
            if (collision.GetContact(i).HasCollider(collider))
                return true;
        return false;
    }

    public static bool CheckBoxIgnore(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layermask, Collider ignore)
    {
        var num = Physics.OverlapBoxNonAlloc(center, halfExtents, ReusableColliders, orientation, layermask);
        for (int i = 0; i < num; i++)
            if (ReusableColliders[i] != ignore)
                return true;
        return false;
    }

    public static bool RaycastIgnore(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, HashSet<Collider> ignoreColliders, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (ignoreColliders == null || ignoreColliders.Count == 0)
            return Physics.Raycast(origin, direction, out hit, maxDistance, layerMask, queryTriggerInteraction);

        var numHits = Physics.RaycastNonAlloc(origin, direction, ReusableHits, maxDistance, layerMask);
        for(int i = 0; i < numHits; i++)
        {
            foreach (var c in ignoreColliders)
                if (c == ReusableHits[i].collider)
                    goto ignored; // screw linq!
            hit = ReusableHits[i];
            return true;
            ignored: ;
        }
        hit = ReusableHits[0];
        return false;
    }

    public static int OverlapBox(BoxCollider collider, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return Physics.OverlapBoxNonAlloc(
            collider.transform.TransformPoint(collider.center),
            collider.size / 2f,
            results,
            collider.transform.rotation,
            layerMask,
            queryTriggerInteraction);
    }

    /// <summary>Uses <see cref="Physics.OverlapBoxNonAlloc"/> but can ignore colliders.</summary>
    public static int OverlapBoxIgnore(BoxCollider collider, Collider[] results, int layerMask, Collider[] ignoreColliders, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        var allHits = Physics.OverlapBoxNonAlloc(
            collider.transform.TransformPoint(collider.center),
            collider.size / 2f,
            ReusableColliders,
            collider.transform.rotation,
            layerMask,
            queryTriggerInteraction);

        var hits = 0;
        for (int i = 0; i < allHits; i++)
        {
            if (!ignoreColliders.Contains(ReusableColliders[i]) && i < results.Length) // skip ignored colliders
            {
                results[i] = ReusableColliders[i];
                hits++;
            }
        }

        return hits;
    }

    public static int OverlapCapsule(CapsuleCollider collider, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        var offset = new Vector3(0f, collider.height * 0.5f - collider.radius, 0f);
        return Physics.OverlapCapsuleNonAlloc(
            collider.transform.position + (collider.transform.rotation * (collider.center + offset)),
            collider.transform.position + (collider.transform.rotation * (collider.center - offset)),
            collider.radius,
            results,
            layerMask,
            queryTriggerInteraction);
    }

    public static int OverlapCollider(Collider collider, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        if (collider is BoxCollider)
            return OverlapBox(collider as BoxCollider, results, layerMask, queryTriggerInteraction);
        else if (collider is CapsuleCollider)
            return OverlapCapsule(collider as CapsuleCollider, results, layerMask, queryTriggerInteraction);

        Debug.LogError("OverlapCollider used on an unsupported collider type, add the type " + collider.GetType().Name);
        return 0;
    }
    
    /// <summary>
    /// Similar to <see cref="CheckCapsule(Vector3, Quaternion, Vector3, Vector3, float, float,QueryTriggerInteraction, int[]"/> but accept a <see cref="LayerMask"/> instead of layers list.
    /// </summary>
    /// <returns></returns>
    public static bool CheckCapsule(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 center, float radius, float height, int layerMask,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var (start, end) = ColliderUtils.GetCapsuleStartEnd(position, rotation, scale, center, radius, height);
        return Physics.CheckCapsule(start, end, radius, layerMask, queryTriggerInteraction);
    }

    /// <summary>Raycast multiple rays in shape of arc</summary>
    /// <param name="position">Ray origin position</param>
    /// <param name="direction">Ray direction</param>
    /// <param name="arcAngle">Angle of the arc</param>
    /// <param name="crossAxis">Cross axis of arc direction</param>
    /// <param name="hitInfo">Raycast hit info</param>
    /// <param name="maxDistance">Maximum raycast distance</param>
    /// <param name="layer">Raycast layer</param>
    /// <param name="rayCount">Raycast count in the arc</param>
    /// <param name="drawDebugRays">Draw debug rays in Unity Editor</param>
    /// <returns>True if it hits the collider</returns>
    public static bool RaycastArc(Vector3 position, Vector3 direction, float arcAngle, Vector3 crossAxis, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity, int layer = Physics.DefaultRaycastLayers, int rayCount = 3, bool drawDebugRays = false)
    {
        //Rotate direction vector on cross axis half way in both direction to calculate start and end vectors of the arc
        var start = Quaternion.AngleAxis(-arcAngle/2f, crossAxis.normalized) * direction;
        var end   = Quaternion.AngleAxis( arcAngle/2f, crossAxis.normalized) * direction;
        
        var current = start;
        for (int i = 0; i <= rayCount - 1; i++)
        {
            var segment = (float)i / (rayCount - 1); //Get the segment value
            var radian = segment * arcAngle * Mathf.Deg2Rad; //Convert segment degree into radian
            current = Vector3.RotateTowards(start, end, radian, 0f); //Get the new segment direction
#if UNITY_EDITOR
            if (drawDebugRays) Debug.DrawRay(position, current, Color.red); //Draw ray in Unity editor
#endif
            if (Physics.Raycast(position, current, out hitInfo, maxDistance, layer)) return true;
        }
        hitInfo = default;
        return false;
    }

    /// <summary>
    /// Returns true if collider can safely be moved to given layer.
    /// Checks for any potential new collisions that could occur from overlapping colliders that collider whould start colliding with if moved to that layer.
    /// Such collisions would have to be resolved by adding a lot of force which would lead to abnormal physics behavior, like rigidbody suddenly gaining huge velocity.
    /// </summary>
    /// <remarks>Only supports basic collider shapes (sphere, capsule, box) and uniform scale (x, y and z of scale vector must be equal).</remarks>
    public static bool CanSwitchToLayer(this Collider collider, int newLayer, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var oldLayer = collider.gameObject.layer;

        if (oldLayer == newLayer) return true; //It's always safe to stay on the same layer

        var transform = collider.transform;
        var scale     = transform.lossyScale.x;
        var hits      = 0; //Number of hits registered

        //Can't use non uniform scale because of spheres used in casts that would no longer be spheres
        if (!Mathf.Approximately(transform.lossyScale.y, scale) || !Mathf.Approximately(transform.lossyScale.z, scale)) throw new ArgumentException($"Only colliders using uniform scale are supported. Current scale: {transform.lossyScale}", nameof(collider));

        //Create layer mask representing only the layers new layer collides with and current layer doesn't
        var oldLayerMask = GetCollidingLayerMask(oldLayer);
        var newLayerMask = GetCollidingLayerMask(newLayer);
        var layerMask    = newLayerMask & (oldLayerMask ^ newLayerMask);

        if (layerMask == 0) return true; //If new layer doesn't collide with more layers than old one it's safe to switch
        
        //Get colliders that should be ignored, because they belong to the same rigidbody
        var ignored = collider.attachedRigidbody?.GetComponentsInChildren<Collider>() ?? Array.Empty<Collider>();


        switch (collider)
        {
            case CapsuleCollider capsule:
                (var start, var end) = capsule.GetCapsuleStartEnd();
                hits = Physics.OverlapCapsuleNonAlloc(start, end, capsule.radius * scale, ReusableColliders, layerMask, queryTriggerInteraction);
                break;
            case SphereCollider sphere:
                hits = Physics.OverlapSphereNonAlloc(transform.TransformPoint(sphere.center), sphere.radius * scale, ReusableColliders, layerMask, queryTriggerInteraction);
                break;
            case BoxCollider box:
                hits = Physics.OverlapBoxNonAlloc(transform.TransformPoint(box.center), box.size * scale, ReusableColliders, transform.rotation, layerMask, queryTriggerInteraction);
                break;
            default:
                throw new ArgumentException($"{collider.GetType().Name} is not supported. Only basic collider shapes (sphere, capsule, box) can be used with this method.", nameof(collider));
        }

        var tmp = ReusableColliders.Take(hits).Where(collider => !ignored.Contains(collider)).ToList();

        return tmp.Count <= 0;

        //return ReusableColliders.Take(hits).None(collider => !ignored.Contains(collider)); //Return true if there are any hits that don't come from the same rigidbody
    }

    /// <summary>Returns layer mask representing all layers given layer collides with.</summary>
    public static LayerMask GetCollidingLayerMask(int layer)
    {
        LayerMask mask = 0; //0 means it doesn't collide with any layers

        //Go through all layers and set matching bits in the mask to 1 if those layers collide with our layer
        for (var i = 0; i < 32; ++i)
            mask |= Physics.GetIgnoreLayerCollision(layer, i) ? 0 : 1 << i;

        return mask;
    }
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public class CollisionRelay : TrackableBehavior
{
    public event System.Action<Collision>   CollisionEnter = delegate {};
    public event System.Action<Collision>   CollisionStay = delegate {};
    public event System.Action<Collider>    TriggerEnter = delegate {};
    public event System.Action<Collider>    TriggerExit = delegate {};
    // target collider filters collisions if multiple colliders attached to the same rigidbody, but it doesn't work for trigger events
    private Collider targetCollider;

    public static bool TryAttach(Rigidbody destination, Collider collider, out CollisionRelay relay, out Rigidbody source)
    {
        source = collider.GetComponentInParent<Rigidbody>();
        if (source == destination)
        {
            relay = null;
            return false;
        }

        relay = ((Component)source ?? collider).gameObject.AddComponent<CollisionRelay>();
        relay.targetCollider = collider;
        return true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (this.IsMatchingCollision(collision))
            this.CollisionEnter(collision);
    }

    public void OnCollisionStay(Collision collision)
    {
        if (this.IsMatchingCollision(collision))
            this.CollisionStay(collision);
    }

    public void OnTriggerEnter(Collider other) => this.TriggerEnter(other);

    public void OnTriggerExit(Collider other) => this.TriggerExit(other);

    public bool IsMatchingCollision(Collision collision) => this.targetCollider == null || UnityGameObjectUtils.ContactedBy(collision, this.targetCollider);
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using Eco.Client.Pooling;
    using UnityEngine;

    [SupportsPooling]
    public class EnableAndDisableColliderDueToUnityBug : TrackableBehavior
    {
        public Collider c;

        public void Awake()
        {
            if (c == null)
                c = GetComponent<Collider>();
        }

        public void OnEnable()
        {
            Invoke(nameof(ToggleCollider), 0.01f);
        }

        public void ToggleCollider()
        {
            // Unity2018 bug, colliders aren't getting activated when resized (or something like that)
            c.enabled = false;
            c.enabled = true;
        }
    }
}

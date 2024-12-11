// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using UnityEngine;

    /// <summary>
    /// Special behaviour which makes <see cref="GameObject"/> transform the <see cref="UnityUtils.DelayedDestroyRoot"/>.
    /// This object should be inactive, so all objects scheduled for destroy will automatically be deactivated when re-attached to this root.
    /// </summary>
    public class DelayedDestroyRoot : TrackableBehavior
    {
        protected void Awake()
        {
            UnityGameObjectUtils.DelayedDestroyRoot = this.transform;
            this.gameObject.SetActive(false);
        }

        protected void OnDestroy() => UnityGameObjectUtils.DelayedDestroyRoot = null;
    }
}

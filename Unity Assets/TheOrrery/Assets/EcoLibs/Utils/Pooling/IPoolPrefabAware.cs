// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    /// <summary>
    /// Interface which may be implemented by behavior to handle even when the component used as <see cref="ObjectPool"/> prefab.
    /// In example it may be used to set shared state once for pooled object or ensure state same for prefab and instance for mismatch detection.
    /// </summary>
    public interface IPoolPrefabAware
    {
        /// <summary> This method called when object prefab owning the component added to the pool. </summary>
        void OnPoolPrefab();
    }
}
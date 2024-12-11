// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    /// <summary>
    /// Can be used with classes (components) which aware about Pool Return event.
    /// <see cref="OnPoolReturn"/> triggered every time when an instance returned to pool.
    /// This callback should be used to reset object state and release all resources.
    /// If you need callback when object rented from pool you should implement <see cref="IPoolRentAware"/> interface.
    /// </summary>
    public interface IPoolReturnAware
    {
        /// <summary> Callback for Pool Return event. </summary>
        void OnPoolReturn();
    }
}
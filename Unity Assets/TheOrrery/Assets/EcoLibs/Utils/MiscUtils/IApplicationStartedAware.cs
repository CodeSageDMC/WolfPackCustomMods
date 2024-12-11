// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.Runtime
{
    /// <summary> Interface for services which aware about <see cref="GameApplicationLifetime.ApplicationStarted"/> event. </summary>
    public interface IApplicationStartedAware
    {
        /// <summary> Callback which is called when application started. </summary>
        void OnApplicationStarted();
    }
}
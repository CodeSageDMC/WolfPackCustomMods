// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.Unity
{
    using System;
    using Eco.Client.Pooling;
    using UnityEngine;

    /// <summary> Implementation of <see cref="IGameObjectMismatchResolver"/> for Eco Engine game objects and components. </summary>
    public class EcoEngineGameObjectMismatchResolver : IGameObjectMismatchResolver
    {
        public static Type IgnoreType;
        public bool ShouldIgnoreExtraGameObject(GameObject gameObject) => false;
        public bool ShouldIgnoreChildren(GameObject gameObject) => false;
        public bool ShouldIgnoreExtraComponent(Component component) => component is PooledInstanceCleaner || component.GetType() == IgnoreType;
    }
}

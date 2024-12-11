// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.Unity
{
    using UnityEngine;

    /// <summary> Implementation of <see cref="IGameObjectMismatchResolver"/> which allows to skip some standard game object hierarchies from mismatch detection. </summary>
    public class UnityEngineGameObjectMismatchResolver : IGameObjectMismatchResolver
    {
        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreExtraGameObject"/>
        public bool ShouldIgnoreExtraGameObject(GameObject gameObject) => false;
        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreExtraComponent"/>
        public bool ShouldIgnoreExtraComponent(Component component) => false;
        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreChildren"/>
        public bool ShouldIgnoreChildren(GameObject gameObject) => gameObject.HasComponent<LODGroup>();
    }
}
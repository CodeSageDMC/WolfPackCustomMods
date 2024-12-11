// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.Unity
{
    using System;
    using UnityEngine;

    /// <summary> Simple predicate based pool issues resolver. You may set <see cref="Predicate{T}"/> for each potential kind of issue. </summary>
    public class SimpleGameObjectMismatchResolver : IGameObjectMismatchResolver
    {
        public Predicate<GameObject>             ResolveExtraGameObjectPredicate;
        public Predicate<GameObject>             ShouldIgnoreChildrenPredicate;

        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreExtraGameObject"/>
        public bool ShouldIgnoreExtraGameObject(GameObject gameObject) => this.ResolveExtraGameObjectPredicate?.Invoke(gameObject) ?? false;
        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreExtraComponent"/>
        public bool ShouldIgnoreExtraComponent(Component component) => false;
        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreChildren"/>
        public bool ShouldIgnoreChildren(GameObject gameObject) => this.ShouldIgnoreChildrenPredicate?.Invoke(gameObject) ?? false;
    }
}
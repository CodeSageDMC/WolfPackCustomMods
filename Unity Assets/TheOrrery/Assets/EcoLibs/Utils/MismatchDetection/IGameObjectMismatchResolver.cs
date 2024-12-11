// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.Unity
{
    using global::UnityEngine;

    /// <summary> Class responsible for resolving potential false-positive issues happened in <see cref="GameObjectMismatchDetector"/>. </summary>
    public interface IGameObjectMismatchResolver
    {
        /// <summary> Try to resolve issue with extra game object (one which doesn't present in prefab). </summary>
        bool ShouldIgnoreExtraGameObject(GameObject gameObject);

        /// <summary> Will ignore children for the <paramref name="gameObject"/> if this returns <c>true</c>. </summary>
        bool ShouldIgnoreChildren(GameObject gameObject);

        /// <summary> Will ignore extra component if this returns <c>true</c>. </summary>
        bool ShouldIgnoreExtraComponent(Component component);
    }
}
// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.TMPro
{
    using EcoEngine.MismatchDetection.Unity;
    using global::TMPro;
    using global::UnityEngine;

    /// <summary> Mismatch resolver for TMPro library. Let <see cref="GameObjectMismatchDetector"/> to ignore TMPro normal lifecycle produced objects and components which doesn't break object re-usability. </summary>
    public class TMProGameObjectMismatchResolver : IGameObjectMismatchResolver
    {
        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreExtraGameObject"/>
        public bool ShouldIgnoreExtraGameObject(GameObject go) => go.name == "Caret" && go.transform.TryGetAncestor(2, out var inputTransform) && inputTransform.HasComponent<TMP_InputField>();
        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreExtraComponent"/>
        public bool ShouldIgnoreExtraComponent(Component component) => false;
        /// <inheritdoc cref="IGameObjectMismatchResolver.ShouldIgnoreChildren"/>
        public bool ShouldIgnoreChildren(GameObject go) => go.HasComponent<TMP_Text>() || go.HasComponent<TMP_InputField>() || go.HasComponent<TMP_Dropdown>();
    }
}
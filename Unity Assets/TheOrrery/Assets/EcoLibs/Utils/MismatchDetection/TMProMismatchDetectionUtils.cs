// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.TMPro
{
    using System.Linq;
    using System.Reflection;
    using Eco.Shared.Utils;
    using UnityEngine;

    internal static class TMProMismatchDetectionUtils
    {
        /// <summary> Checks if <paramref name="targetComponent"/> should ignore text mismatch (a field referencing this component marked with <see cref="TMProTextMismatchIgnoreAttribute"/> in same game object or in any of parent game objects). </summary>
        internal static bool ShouldIgnoreTextMismatch(Component targetComponent)
        {
            return targetComponent.gameObject.AnyComponentInParent<MonoBehaviour>(true,
                comp => comp.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(
                    field => field.HasAttribute<TMProTextMismatchIgnoreAttribute>() && ReferenceEquals(field.GetValue(comp), targetComponent)));
        }
    }
}
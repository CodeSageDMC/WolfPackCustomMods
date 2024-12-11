// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.Unity
{
    using System.Collections.Generic;
    using System.Linq;
    using Eco.Shared.Localization;
    using Eco.Shared.Text;
    using global::UnityEngine;

    /// <summary>
    /// Class which checks mismatches between two game objects (instance and prefab). It isn't intended to be used for debugging and detecting difference between two objects which may signal about error (i.e. when an instance returned to pool in invalid state and may behave incorrectly when rented).
    /// Currently it supports following mismatches detection:
    /// <ul>
    /// <li>object instance mismatches by game object hierarchy from prefab (extra objects or missing objects);</li>
    /// <li>object instance mismatches by components list from prefab for any game object in hierarchy (extra components or missing components);</li>
    /// <li>object instance mismatches by component data from prefab for any component for any game object in hierarchy.</li>
    /// </ul>
    /// In some cases it is fine to have some mismatches between an instance and the prefab. In this case you may want to not see false positives reporting about issues which you know isn't issues.
    /// To solve this you can implement <see cref="IGameObjectMismatchResolver"/> in your component or add one of it's implementation to <see cref="DefaultMismatchResolver"/>.
    /// When an instance checked it will ask both <see cref="DefaultMismatchResolver"/> and all current hierarchy components to resolve any found mismatches.
    /// In example, TMP_InputField will create Cursor object in an instance when used which doesn't present in prefab. If we found extra game object in hierarchy named "Cursor" and it has parent "TMP_InputField" then we can safely ignore it.
    /// </summary>
    public class GameObjectMismatchDetector : MismatchDetectorBase<GameObject>
    {
        public IGameObjectMismatchResolver DefaultMismatchResolver { get; set; } = new CompositeGameObjectMismatchResolver();

        /// <inheritdoc cref="IMismatchDetector{T}.DetectMismatches(T,T,EcoEngine.MismatchDetection.MismatchDetectionContext)"/>
        public override InfoBuilder DetectMismatches(GameObject one, GameObject other, MismatchDetectionContext context)
        {
            var infoBuilder = new InfoBuilder();
            // detect components mismatches
            infoBuilder.AddSectionLocStr("Components", this.DetectComponentsMismatches(one.GetComponents<MonoBehaviour>(), other.GetComponents<MonoBehaviour>(), context));
            // detect children mismatches if they shouldn't be ignored
            if (!this.DefaultMismatchResolver.ShouldIgnoreChildren(one))
                infoBuilder.AddSectionLocStr("Children", this.DetectChildrenMismatches(this.GetSortedChildren(one), this.GetSortedChildren(other), context));
            return infoBuilder;
        }

        /// <summary> Adds info about mismatches between instance and prefab component arrays. </summary>
        private InfoBuilder DetectComponentsMismatches(MonoBehaviour[] instanceComponents, MonoBehaviour[] prefabComponents, MismatchDetectionContext context)
        {
            var infoBuilder   = new InfoBuilder();
            var instanceIndex = 0;
            var prefabIndex   = 0;
            // check all instance components for mismatches, we are skipping all mismatching prefab components, expecting same order
            for (; instanceIndex < instanceComponents.Length; ++instanceIndex)
            {
                var instanceComponent = instanceComponents[instanceIndex];
                // check if we have matching component
                if (prefabIndex < prefabComponents.Length && instanceComponent.GetType() == prefabComponents[prefabIndex].GetType())
                {
                    infoBuilder.AddSection(Localizer.NotLocalizedStr(instanceComponent.GetType().Name), context.DetectMismatches(instanceComponent, prefabComponents[prefabIndex]));
                    ++prefabIndex;
                }
                // issue detected, no matching component for the instance component
                else if (!this.DefaultMismatchResolver.ShouldIgnoreExtraComponent(instanceComponent))
                    infoBuilder.AppendLineLoc($"The {context.OneName} has a component missing in the {context.OtherName}: {instanceComponent.GetType().Name} (or at wrong position)");
            }

            // for any extra prefab components report a pool issue
            for (; prefabIndex < prefabComponents.Length; ++prefabIndex)
                infoBuilder.AppendLineLoc($"The {context.OneName} missing a component from the {context.OtherName}: {prefabComponents[prefabIndex].GetType().Name}");
            return infoBuilder;
        }

        /// <summary> Returns child transforms order by name. </summary>
        private IList<Transform> GetSortedChildren(GameObject gameObject) => gameObject.transform.Children().OrderBy(x => x.name).ToArray();

        /// <summary> Adds info about mismatches between instance and prefab game object arrays (children). </summary>
        private InfoBuilder DetectChildrenMismatches(in IList<Transform> instanceChildren, in IList<Transform> prefabChildren, MismatchDetectionContext context)
        {
            var infoBuilder   = new InfoBuilder();
            var instanceIndex = 0;
            var prefabIndex   = 0;
            // check all instance children objects for mismatches, we are skipping all mismatching prefab children, expecting same order
            for (; instanceIndex < instanceChildren.Count; ++instanceIndex)
            {
                var instance = instanceChildren[instanceIndex];
                // check if we have matching prefab
                if (prefabIndex < prefabChildren.Count && instance.name == prefabChildren[prefabIndex].name)
                {
                    infoBuilder.AddSection(Localizer.NotLocalizedStr(instance.name), this.DetectMismatches(instance.gameObject, prefabChildren[prefabIndex].gameObject, context));
                    ++prefabIndex;
                }
                // issue detected, no matching prefab for the instance child
                else if (!this.DefaultMismatchResolver.ShouldIgnoreExtraGameObject(instance.gameObject))
                    infoBuilder.AppendLineLoc($"The {context.OneName} has a child missing in the {context.OtherName}: {instance.name} (or at wrong position)");
            }

            // for any extra prefab children report a pool issue
            for (; prefabIndex < prefabChildren.Count; ++prefabIndex)
                infoBuilder.AppendLineLoc($"The {context.OneName} missing a child from the {context.OtherName}: {prefabChildren[prefabIndex].name}");
            return infoBuilder;
        }
    }
}
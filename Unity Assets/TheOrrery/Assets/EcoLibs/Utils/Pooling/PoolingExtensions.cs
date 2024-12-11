// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Eco.Shared.Pools;
    using Eco.Shared.Utils;
    using JetBrains.Annotations;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public static class PoolingExtensions
    {
        public static readonly HashSet<Type> PoolableTypes = new HashSet<Type>();

        static PoolingExtensions()
        {
            PoolableTypes.AddRange(new[]
            {
                // UnityEngine
                typeof(Transform),
                typeof(RectTransform),
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(SkinnedMeshRenderer),
                typeof(ParticleSystemRenderer),
                typeof(CanvasRenderer),
                typeof(MeshCollider),
                typeof(BoxCollider),
                typeof(SphereCollider),
                typeof(CapsuleCollider),
                typeof(LODGroup),
                typeof(Rigidbody),
                typeof(Animator),
                typeof(ParticleSystem),
                typeof(Image),
                typeof(AudioSource),
                typeof(LayoutElement),
                typeof(Button),
                typeof(UnityEngine.UI.Text),
                typeof(ContentSizeFitter),
                typeof(VerticalLayoutGroup),
                typeof(HorizontalLayoutGroup),
                typeof(Toggle),
                typeof(RectMask2D),
                typeof(Selectable),
                typeof(Scrollbar),
                typeof(ScrollRect),
                typeof(Canvas),
                typeof(CanvasGroup),
                typeof(GridLayoutGroup),
                typeof(Tree),
                
                // TMPro
                typeof(TextMeshProUGUI),
                typeof(TMP_InputField),
                typeof(TMP_Dropdown),
            });

            PoolableTypes.AddRange(ReflectionCache.GetGameAssembliesTypes().Where(x =>
                !x.IsAbstract && (x.HasAttribute<SupportsPoolingAttribute>(false) || typeof(IPoolRentAware).IsAssignableFrom(x) || typeof(IPoolReturnAware).IsAssignableFrom(x))));
        }

        private static readonly Func<Component, bool> ComponentSupportsPoolingDelegate = SupportsPooling;

        [PublicAPI] public static void RegisterPoolableType(Type type) { PoolableTypes.Add(type); }

        public static bool SupportsPooling(this Component component) => component != null && PoolableTypes.Contains(component.GetType());

        /// <summary> Checks if <see cref="GameObject"/> supports pooling. It is only <c>true</c> if all <paramref name="gameObject"/> components and all it's children components <see cref="SupportsPooling(UnityEngine.Component)"/>. </summary>
        public static bool SupportsPooling(this GameObject gameObject) => gameObject.AllComponentsInChildren(ComponentSupportsPoolingDelegate, true);

        public static bool TryRentFrom(IObjectPool<GameObject> pool, out GameObject obj)
        {
            obj = pool.Get();
            if (ReferenceEquals(obj, null))
                return false;

            obj.ForEachComponentInChildren<IPoolRentAware>(c => c.OnPoolRent());
            return true;
        }

        /// <summary> Returns <paramref name="obj"/> to the <paramref name="pool"/>. If <paramref name="poolContainer"/> not null then will attach pooled object to <paramref name="poolContainer"/>. </summary>
        public static bool ReturnTo(this GameObject obj, IObjectPool<GameObject> pool, Transform poolContainer)
        {
            if (!pool.TryAdd(obj))
                return false;

            obj.transform.parent = poolContainer;
            obj.SetActive(false);
            obj.ForEachComponentInChildren<IPoolReturnAware>(c => c.OnPoolReturn());
            return true;
        }
    }
}

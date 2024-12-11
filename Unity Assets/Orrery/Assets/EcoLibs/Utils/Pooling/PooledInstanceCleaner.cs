// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> Special behavior which helps to perform cleanup on instances returned to pool (especially useful when temporary components added or standard components modified which can't implement <see cref="IPoolReturnAware"/> interface). </summary>
    public class PooledInstanceCleaner : TrackableBehavior, IPoolReturnAware
    {
        [NonSerialized] private readonly List<Component> tempComponents = new List<Component>();
        public event Action ReturnedToPool;

        /// <summary> Gets existing component of type <typeparamref name="T"/> or adds new temp component. </summary>
        public T GetOrAddTempComponent<T>() where T : Component => this.TryGetComponent<T>(out var component) ? component : this.AddTempComponent<T>();

        /// <summary> Adds new temp component (which will be destroyed when object returned to pool). </summary>
        public T AddTempComponent<T>() where T : Component
        {
            var component = this.gameObject.AddComponent<T>();
            this.tempComponents.Add(component);
            return component;
        }

        void IPoolReturnAware.OnPoolReturn()
        {
            foreach (var component in this.tempComponents)
                DestroyImmediate(component); // need to destroy immediate here in case object is pooled and rent on same frame
            this.tempComponents.Clear();
            this.ReturnedToPool?.Invoke();
            this.ReturnedToPool = null;
        }
    }

    /// <summary> Set of extension methods which makes adding temp components simpler. </summary>
    public static class TempComponentsExtensions
    {
        public static T GetOrAddTempComponent<T>(this GameObject gameObject) where T : Component => gameObject.GetOrAddComponent<PooledInstanceCleaner>().GetOrAddTempComponent<T>();
    }
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eco.Client.Utils
{
    /// <summary>
    /// Transform collection compatible with <see cref="IReadOnlyList{T}"/> and <see cref="IEnumerable{T}"/> interfaces.
    /// It has following benefits over standard Unity API:
    /// <ul>
    /// <li><see cref="UnityEngine.Transform"/> implements only <see cref="IEnumerable"/> interface, so it can't be used with LINQ and for easy conversion to array or list;</li>
    /// <li>it has custom value-type enumerator <see cref="TransformChildren.Enumerator"/> which won't make GC-allocation when used with <c>foreach</c>;</li>
    /// <li>it has well-known list interface for accessing children by index instead of pair of methods <see cref="Transform.childCount"/> and <see cref="Transform.GetChild"/>.</li>
    /// </ul>
    /// Can be obtained with <see cref="UnityUtils.Children"/> extension method.
    /// </summary>
    public readonly struct TransformChildren : IReadOnlyList<Transform>
    {
        public struct Enumerator : IEnumerator<Transform>
        {
            private readonly Transform transform;
            private          int       index;

            internal Enumerator(Transform transform)
            {
                this.index     = -1;
                this.transform = transform;
            }

            public bool MoveNext()
            {
                if (++this.index < this.transform.childCount)
                    return true;

                this.index = this.transform.childCount;
                return false;
            }
            public Transform Current => this.transform.GetChild(this.index);

            void IEnumerator.  Reset() { this.index = 0; }
            object IEnumerator.Current => this.Current;

            public void Dispose() { }
        }

        private readonly Transform transform;

        internal TransformChildren(Transform transform) { this.transform = transform; }

        public Enumerator                             GetEnumerator() => new Enumerator(this.transform);
        IEnumerator<Transform> IEnumerable<Transform>.GetEnumerator() => this.GetEnumerator();
        IEnumerator IEnumerable.                      GetEnumerator() => this.GetEnumerator();
        public int                                    Count           => this.transform.childCount;
        public Transform this[int index] => this.transform.GetChild(index);

        /// <summary> Destroys all children Immediate. </summary>
        public void DestroyAllImmediate()
        {
            for (var i = this.transform.childCount - 1; i >= 0; --i)
                Object.DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }
}
// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

namespace Eco.Client.Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>Enumerable for transform hierarchy including <see cref="root"/> and all it's children.</summary>
    public readonly struct TransformHierarchyEnumerable : IEnumerable<Transform>
    {
        readonly Transform root;

        public TransformHierarchyEnumerable(Transform root) => this.root = root;

        public Enumerator GetEnumerator() => new(this.root);

        IEnumerator<Transform> IEnumerable<Transform>.GetEnumerator() => this.GetEnumerator();
        IEnumerator                       IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>Enumerator of transform hierarchy (all transforms in hierarchy including root).</summary>
        public struct Enumerator : IEnumerator<Transform>
        {
            readonly Transform root;

            public Transform Current { get; private set; }

            public Enumerator(Transform root)
            {
                this.root    = root;
                this.Current = null!;
            }

            object IEnumerator.Current => this.Current;

            /// <summary>Moves to next element. Enumerates elements using Depth-First graph traversal algorithm.</summary>
            public bool MoveNext()
            {
                if (ReferenceEquals(this.Current, this.root)) // because root is always last node it means we already visited all other nodes in hierarchy
                    return false;

                if (this.Current is null) // for first operation we start with root and go to first leaf child as first node
                {
                    this.Current = this.root;
                    this.MoveToFirstLeaf();
                }
                else
                    this.MoveToNextSiblingLeafOrParent(); // otherwise go to next sibling (if it exists) or to parent

                return true;
            }

            /// <summary>Moves to next sibling first leaf or if there no more siblings then to parent.</summary>
            void MoveToNextSiblingLeafOrParent()
            {
                var nextSiblingIndex = this.Current.GetSiblingIndex() + 1;
                var parent           = this.Current.parent;
                if (nextSiblingIndex < parent.childCount)
                {
                    this.Current = parent.GetChild(nextSiblingIndex);
                    this.MoveToFirstLeaf();
                    return;
                }

                this.Current = parent;
            }

            /// <summary>Moves to first leaf (node without child nodes) for <see cref="Current"/> node.</summary>
            void MoveToFirstLeaf()
            {
                while (this.Current.childCount > 0)
                    this.Current = this.Current.GetChild(0);
            }

            public void Reset() => this.Current = null!;

            public void Dispose()
            {
            }
        }
    }
}

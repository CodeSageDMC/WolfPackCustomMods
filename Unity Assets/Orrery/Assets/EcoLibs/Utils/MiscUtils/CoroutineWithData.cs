// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections;
using UnityEngine;

namespace Eco.Client.Utils
{
    public class CoroutineWithData<T>
    {
        private IEnumerator target;
        public  T           Result    { get; private set; }
        public  Coroutine   Coroutine { get; private set; }

        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.target   = target;
            Coroutine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (this.target.MoveNext())
            {
                Result = (T)this.target.Current;
                yield return Result;
            }
        }
    }
}
// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.Subscription
{
    using System.Runtime.CompilerServices;
    using Eco.Shared.View;
    using UnityEngine;

    /// <summary>
    /// Extension of <see cref="IThreadUnsafeSubscriptions"/> interface for <see cref="MonoBehaviour"/>.
    /// It has additional flag <see cref="SubscriptionsReleased"/> which may be used to track if subscriptions was released for the instance.
    /// Usually subscriptions released with <see cref="MonoBehaviourSubscriptionsExtensions.ReleaseSubscriptions"/> extension method which also sets the flag.
    /// When the component subscriptions released with the the extension method then they also releases all child component subscriptions (including inactive).
    /// It helps to solve the issue when `OnDestroy` method never called on components of game object which never was active and so subscriptions never unsubscribed
    /// leading to exceptions when view notification received by destroyed game object (but not invoked OnDestroy callback).
    /// </summary>
    public interface IMonoBehaviourSubscriptions : IThreadUnsafeSubscriptions
    {
        bool SubscriptionsReleased { get; set; }
    }

    /// <summary> Extension methods for <see cref="IMonoBehaviourSubscriptions"/>. </summary>
    public static class MonoBehaviourSubscriptionsExtensions
    {
        public static void ReleaseSubscriptions(this GameObject gameObject)
        {
            gameObject.ForEachComponentInChildren<IMonoBehaviourSubscriptions>(true, comp =>
            {
                if (comp.SubscriptionsReleased) return;
                comp.UnsubscribeAll(true);
                comp.SubscriptionsReleased = true;
            });
        }

        /// <summary> Releases subscriptions on <paramref name="monoBehaviourSubscriptions"/> (if not yet released) and also recursively releases subscriptions on all child components. Read the motivation in <see cref="IMonoBehaviourSubscriptions"/> docs. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReleaseSubscriptions(this IMonoBehaviourSubscriptions monoBehaviourSubscriptions)
        {
            if (monoBehaviourSubscriptions.SubscriptionsReleased) return;
            ReleaseSubscriptions(((MonoBehaviour)monoBehaviourSubscriptions).gameObject);
        }
    }
}

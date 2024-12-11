// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Base Pool aware class which makes pool life-cycle management simpler.
    /// It implements following life-cycle hooks which may be overriden:
    /// - <see cref="AwakeReusableInstance"/> - called only once per instance lifecycle when <see cref="Awake"/> called by Unity or when object instantiated by Pool. It ensures you always have same life-cycle for pooled objects (rented or just instantiated).
    /// - <see cref="StartReusableInstance"/> - called only once per instance lifecycle when <see cref="Start"/> called by Unity
    /// - <see cref="InitInstance"/>          - first time called just after <see cref="AwakeReusableInstance"/> and then every time when an instance rented from pool.
    /// - <see cref="ReleaseInstance"/>       - called every time when an initialized instance returned to pool (should receive <see cref="InitInstance"/>, this callback skipped otherwise) or when an instance destroyed (called from <see cref="OnDestroy"/>), it has `destroy` argument which signals if the instance actually destroying or just released for pool.
    /// </summary>
    public abstract class PoolAwareBehavior : SubscribableBehavior, IPoolInstantiateAware, IPoolRentAware, IPoolReturnAware
    {
        [Flags]
        private enum Flags : byte
        {
            Awaken = 1,
            Started = 2
        }

        /// <summary> Flags holding component state (Started, Awaken). </summary>
        private Flags flags;

        /// <summary> Returns <c>true</c> if object was started (with Unity <see cref="Start"/> event). </summary>
        protected bool Started => (this.flags & Flags.Started) != 0;
        /// <summary> Returns <c>true</c> if object was awaken (with Unity <see cref="Awake"/> event). </summary>
        protected bool Awaken => (this.flags & Flags.Awaken) != 0;

        protected void Awake() => ((IPoolInstantiateAware)this).OnPoolInstantiate();

        protected void Start()
        {
            this.flags |= Flags.Started;
            this.StartReusableInstance();
        }

        protected override void OnDestroy() => this.ReleaseInstance(true);

        /// <summary> Callback from <see cref="Awake"/> which may be implemented to perform one-time initialization for an instance which then may be rented from pool multiple times. </summary>
        protected virtual void AwakeReusableInstance() { }
        /// <summary> Callback from <see cref="Start"/> which may be implemented to perform one-time initialization for an instance which then may be rented from pool multiple times. </summary>
        protected virtual void StartReusableInstance() { }
        /// <summary>
        /// Callback from <see cref="Start"/> or <see cref="IPoolRentAware.OnPoolRent"/> (only for started instance!).
        /// First-time invoked right after <see cref="StartReusableInstance"/>. Then (if instance was started) every time when object rented from pool.
        /// If object was added to pool before it was started then call to <see cref="InitInstance"/> will be delayed until component <see cref="Start"/>.
        /// </summary>
        protected virtual void InitInstance() { }
        /// <summary>
        /// Called every time when initialized (with <see cref="InitInstance"/>) component instance returned to pool. In this case <paramref name="destroy"/> have <c>false</c> value.
        /// If the component instance was returned to pool before <see cref="Awake"/> event triggered then this method won't be called, because <see cref="InitInstance"/> not invoked in this case.
        /// Because <see cref="OnDestroy"/> called only on awaken instances it is guaranteed that it also has <see cref="InitInstance"/> called before this call.
        /// </summary>
        protected virtual void ReleaseInstance(bool destroy) { }

        void IPoolInstantiateAware.OnPoolInstantiate()
        {
            // This may be called either from Pool or from Awake whatever happened first, but only need to proceed one time for non-awaken instance
            if (!this.Awaken)
            {
                this.flags |= Flags.Awaken;
                this.AwakeReusableInstance();
                this.InitInstance();
            }
        }

        /// <summary> Prevent accidental override. </summary>
        [UsedImplicitly] protected void OnPoolInstantiate() => ((IPoolInstantiateAware)this).OnPoolInstantiate();

        void IPoolRentAware.OnPoolRent()
        {
            // Only call InitInstance for awaken instances, otherwise delay until Awake
            if (this.Awaken)
                this.InitInstance();
        }

        /// <summary> Prevent accidental override. </summary>
        [UsedImplicitly] protected void OnPoolRent() => ((IPoolRentAware)this).OnPoolRent();

        void IPoolReturnAware.OnPoolReturn()
        {
            // only release instance when InitInstance was called (it means Awake was called)
            if (this.Awaken)
                this.ReleaseInstance(false);
        }

        /// <summary> Prevent accidental override. </summary>
        [UsedImplicitly] protected void OnPoolReturn() => ((IPoolReturnAware)this).OnPoolReturn();
    }
}

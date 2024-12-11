// Inspired by the code at http://catlikecoding.com/unity/tutorials/object-pools/

namespace Eco.Client.Pooling
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Eco.Shared.Pools.Collections;
    using Eco.Shared.Utils;
    using EcoEngine.MismatchDetection;
    using UnityEngine;

    // Unity doesn't allow us to override 'Destroy' method, so there is no method to 'save' object from being destroyed.
    // But it allows to save any child object from further destroy process. So we can put all chilren pooled nodes back to pool instead.
    // So every UI component which wants to use PooledObject should implement this interface in order to control lifecycle of owned pooled objects
    public interface IPooledObjectOwner
    {
        PoolableListWrapper<PooledObject> UsedPooledObjects { get; }
    }

    /// <summary> Adding PooledObject to a <see cref="GameObject"/> allows to use it with <see cref="ObjectPool"/>. You can find more details about pooling system in Pooling.md file (in namespace directory). </summary>
    [DisallowMultipleComponent]
    public class PooledObject : TrackableBehavior, IPoolRentAware, IPoolReturnAware, IMemberMismatchDetectionAware
    {
        public static PoolManager PoolManager;          //Must be assigned from the main codebase.
        public static Func<bool> ShouldTracePoolEvents; //Must be assigned by main codebase, for debug tuner injection.

#if UNITY_EDITOR
        private StackTrace lastStateChangeTrace;
#endif
        public PooledObject gameObjectPrefab;
        [Tooltip("Keeping too much pooled objects will affect performance as well. For example, when closing window it is faster to destroy objects than re-parent them to pool. Also sometimes big pool is not needed but uses memory.")]
        public int maxPooledCount = -1; 

        [NonSerialized] public IPooledObjectOwner owner;

        public PooledObjectState State { get; private set; } = PooledObjectState.Prefab;
        /// <summary> Checks if <see cref="PooledObject"/> currently is in pool. </summary>
        public bool IsPooled => this.State == PooledObjectState.Returned;
        /// <summary> Checks if <see cref="PooledObject"/> was just instantiated (not rented from pool). </summary>
        public bool IsNew => this.State == PooledObjectState.New;

        /// The pool used for an instance GameObject
        public ObjectPool Pool { get; internal set; }

        /// <summary> Binds <see cref="PooledObject"/> instance to pool. It is now associated with pool and enabled for returning and renting operations. Sets it to <see cref="PooledObjectState.New"/> state. </summary>
        internal void BindToPool(ObjectPool pool)
        {
            this.Pool = pool;
            this.State = PooledObjectState.New;
        }

        // check if pooled object is created (or peeked from pool) with IPooledObjectOwner provided
        // commented because prefab could be runtime for now. Add check after Alex' refactor
//         private void Awake()    => Assert.IsNotNull(owner);
//         private void OnEnable() => Assert.IsNotNull(owner);

        /// <summary> Gets or creates associated <see cref="ObjectPool"/>. Called on prefab. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPool GetOrCreatePool(bool reparentPrefab = false) => PoolManager.GetOrCreatePool(this, reparentPrefab);

        /// <summary> Should be called by pooling code when <see cref="PooledObject"/> actually rented from pool (not new instance). Called on instance. </summary>
        public virtual void OnPoolRent()
        {
            this.AssertInState(PooledObjectState.Returned); // only previously returned object may be rented
            this.SetState(PooledObjectState.Rented);
        }

        /// <summary> Should be called by pooling code when <see cref="PooledObject"/> returned to pool. Called on instance. </summary>
        public virtual void OnPoolReturn()
        {
            this.AssertNotInState(PooledObjectState.Prefab);   // can't return prefab, only instances allowed
            this.AssertNotInState(PooledObjectState.Returned); // can't return already returned object
            this.SetState(PooledObjectState.Returned);
        }

        void OnValidate()
        {
            if (!ReferenceEquals(this.Pool, null) && this.Pool == null) //Having link in Pool, but Unity don't detects it - it means that it references to deleted object. Clean it.
                this.Pool = null;
        }

        protected void OnDestroy()
        {
            // if prefab destroyed then bounded pool should be destroyed as well
            if (this.State == PooledObjectState.Prefab && this.Pool != null)
            {
                DestroyImmediate(this.Pool);
                this.Pool = null; //Fixes the issue when pooled objects contains deleted object in pool and prevents from loading
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetState(PooledObjectState state)
        {
            this.State = state;
#if UNITY_EDITOR
            this.lastStateChangeTrace = ShouldTracePoolEvents() ? new StackTrace() : null;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        private void AssertInState(PooledObjectState state)
        {
#if UNITY_EDITOR
            if (this.State != state)
                DebugUtils.Fail($"PooledObject is in wrong state: {this.State}, expected state: {state}.{(this.lastStateChangeTrace != null ? $"\nLast time state was changed from: {this.lastStateChangeTrace}" : string.Empty)}");
#endif
        }

        [Conditional("UNITY_EDITOR")]
        private void AssertNotInState(PooledObjectState state)
        {
#if UNITY_EDITOR
            if (this.State == state)
                DebugUtils.Fail($"PooledObject is already in state: {state}.{(this.lastStateChangeTrace != null ? $"\nLast time state was changed from: {this.lastStateChangeTrace}" : string.Empty)}");
#endif
        }

        /// <inheritdoc cref="IMemberMismatchDetectionAware.ShouldIgnoreMemberMismatch"/>
        public bool ShouldIgnoreMemberMismatch(MemberInfo member, object prefabValue, Exception prefabException) => member.Name == nameof(this.State) || member.Name == nameof(this.Pool);
    }
}

// Inspired by the code at http://catlikecoding.com/unity/tutorials/object-pools/

using Eco.Shared.View;

namespace Eco.Client.Pooling
{
    using System;
    using System.Linq;
    using Eco.Shared.Pools;
    using Eco.Shared.Text;
    using Eco.Shared.Utils;
    using EcoEngine.MismatchDetection;
    using UnityEngine;

    // Pool itself for PooledObjects, do not create it on your own, use static methods instead
    // You can find more details about pooling system in Pooling.md file (in namespace directory).
    public class ObjectPool : SubscribableBehavior
    {
        internal const  int InitialMaxPoolSize = 1024;
        internal static int DefaultMaxPoolSize = InitialMaxPoolSize;

        [EcoShowOnly] public PooledObject prefab;           // this prefab was used for the pool
        [EcoShowOnly] public PooledObject customizedPrefab; // this component will be used to create new instances, in most cases it is same as prefab, but if you have components implementing IPoolPrefabAware then it may create new instance (parented to ObjectPool) to avoid prefab modifications

        internal SubscribableEvent<Action<ObjectPool>> PoolDestroyed = new();

        private readonly ThreadUnsafeFixedSizePool<PooledObject> availableObjects = new ThreadUnsafeFixedSizePool<PooledObject>(DefaultMaxPoolSize);

        private int totalSize;
        public int Size => this.availableObjects.Count;
        public int TotalSize => this.totalSize;

        public int Count => this.Size;

        /// <summary> Changes max pool size. It won't reduce pool size if it already exceeds max size. </summary>
        public void SetMaxPoolSize(int maxPoolSize) => this.availableObjects.MaxSize = maxPoolSize;

        /// <summary>
        /// Either rent from pool or instantiates new game object from <see cref="prefab"/> and returns an instance of <see cref="PooledObject"/> component.
        /// It may be:
        /// <ul>
        /// <li>optionally associated with <paramref name="owner"/>;</li>
        /// <li>optionally attached to <paramref name="parent"/>;</li>
        /// <li>optionally set active (with <paramref name="setActive"/>).</li>
        /// </ul>
        /// </summary>
        public PooledObject Get(Transform parent = null, bool setActive = true, IPooledObjectOwner owner = null)
        {
            PooledObject pooledObject;
            GameObject go;
            if (this.availableObjects.Count > 0)
            {
                pooledObject = this.availableObjects.Get();
                go = pooledObject.gameObject;
                // optionally set parent
                if (parent) go.transform.SetParent(parent, false);
                this.RunRecursiveCallback<IPoolRentAware>(go, c => c.OnPoolRent());

                // optionally set active
                if (setActive) go.SetActive(true);
            }
            else
            {
                this.totalSize++;
                // if parent not provided then use current transform as parent, attach to provided parent otherwise
                pooledObject = this.InstantiateObject(!ReferenceEquals(parent, null) ? parent : this.transform);
                go           = pooledObject.gameObject;
                this.RunRecursiveCallback<IPoolInstantiateAware>(go, c => c.OnPoolInstantiate());
                go.SetActive(setActive);
            }

            // set owner and start tracking of pooled object
            if (owner != null)
            {
                pooledObject.owner = owner;
                DebugUtils.Assert(!owner.UsedPooledObjects.Contains(pooledObject), "Object already in pool");
                owner.UsedPooledObjects.Add(pooledObject);
            }

            return pooledObject;
        }

        /// <summary> Call <see cref="IPoolRentAware.OnPoolRent"/> callbacks for <paramref name="go"/>. </summary>
        private void RunRecursiveCallback<TComponent>(GameObject go, Action<TComponent> callback)
        {
            // call for each own component
            go.ForEachComponent(callback);
            go.TraverseChildren(callback, (child, cb)  =>
            {
                if (child.TryGetComponent<PooledObject>(out var obj) && obj.State == PooledObjectState.Prefab)
                    return false;

                child.ForEachComponent((Action<TComponent>)cb);
                return true;
            });
        }

        /// <inheritdoc cref="IObjectPool{T}.Get()"/>
        public bool TryAdd(PooledObject pooledObject)
        {
            if (pooledObject.IsPooled)
            {
                DebugUtils.Fail($"Trying to add {pooledObject.name} to the pool which already in a pool.");
                // we return true, because this object was actually already added to pool, so we won't execute logic (like Destroy) for objects which wasn't added to pool
                return true;
            }

            // If owner set then removes object from list of used pooled objects
            if (!ReferenceEquals(pooledObject.owner, null))
            {
                var removed = pooledObject.owner.UsedPooledObjects.Remove(pooledObject);
                DebugUtils.Assert(removed, "PooledObject expected to belong to owner, but it wasn't (not listed in UsedPooledObjects collection).");
                pooledObject.owner = null;
            }

            var go = pooledObject.gameObject;
            go.SetActive(false);
            this.RunRecursiveCallback<IPoolReturnAware>(go, c => c.OnPoolReturn());
            pooledObject.transform.SetParent(this.transform, false);
            if (!this.availableObjects.TryAdd(pooledObject))
            {
                Destroy(go);
                return false;
            }

            return true;
        }

        public void SetPrefab(PooledObject prefab)
        {
            if (!prefab.gameObject.SupportsPooling())
            {
                var componentList = string.Join(", ", prefab.GetComponentsInChildren<Component>(true).NonNull().Where(x => !x.SupportsPooling()).Select(x => $"{x.name} ({x.GetType().Name})"));
                Debug.LogWarning($"{prefab.gameObject.name} doesn't support pooling. It may cause problems in runtime. Either add [SupportsPooling] attribute to all it's and it's children components or implement pool supporting interface(s) (IPoolRentAware, IPoolReturnAware). For third-party components you can use PoolingExtensions.RegisterPoolableType. List of components missing pooling support: {componentList}");
            }

            // destroy customized original (if it was previously set and prefab changed)
            this.DestroyCustomizedOriginal();

            this.prefab = prefab;
            // Only create new instance for prefab which has custom prefab preparation logic (to avoid original prefab modifications)
            this.customizedPrefab = this.prefab.HasComponentInChildren<IPoolPrefabAware>(true) ? this.InstantiateCustomizedOriginal() : this.prefab;
        }

        // This method keeps instantiations until given pool size reached
        // Calling this method will cause significant performance hit, avoid using it while in-game, better place is scene init
        public void PopulatePool(int count)
        {
            for (var i = this.totalSize; i < count; i++)
            {
                var obj = this.InstantiateObject(this.transform);
                this.TryAdd(obj);
            }
        }

        /// <summary> Clears pool from all currently hold instances. </summary>
        public void Clear(bool resetOriginal = false)
        {
            while (this.availableObjects.Count > 0)
            {
                var obj = this.availableObjects.Get();
                Destroy(obj.gameObject);
            }

            // Recreate 'original' for pool when we are clearing to update prefabs from disk
            if (resetOriginal && this.HasCustomizedOriginal)
            {
                this.DestroyCustomizedOriginal();
                this.customizedPrefab = this.InstantiateCustomizedOriginal();
            }
        }

        /// <summary>
        /// Detects <see cref="ObjectPool"/> issues using <paramref name="context"/>.
        /// Currently it detects when prefab used for instance creation mismatches from an instance(s) in the pool.
        /// It may be sign of not-fully reset instance which may then cause incorrect behavior (double subscriptions, destroyed state etc).
        /// This method intended for debugging and not for regular usage.
        /// </summary>
        public InfoBuilder DetectPoolIssues(MismatchDetectionContext context)
        {
            var infoBuilder = new InfoBuilder();
            var index       = 0;
            foreach (var obj in this.availableObjects)
                infoBuilder.AddSectionLoc($"Instance [{index++}]: {obj.gameObject.name}", context.DetectMismatches(obj.gameObject, this.customizedPrefab.gameObject));

            return infoBuilder;
        }

        /// <summary> Instantiates object from <see cref="prefab"/>. Newly created object automatically bounded to the pool. </summary>
        private PooledObject InstantiateObject(Transform parent)
        {
            var obj = Instantiate(this.customizedPrefab, parent, false);
            obj.name = this.customizedPrefab.name; // remove "(Clone)"
            obj.BindToPool(this);
            return obj;
        }

        protected override void OnDestroy()
        {
            this.PoolDestroyed?.Invoke(this);
            this.availableObjects.Clear();
            base.OnDestroy();
        }

        /// <summary> Checks if <see cref="customizedPrefab"/> was customized (with <see cref="InstantiateCustomizedOriginal"/>). </summary>
        private bool HasCustomizedOriginal => this.customizedPrefab != null && this.customizedPrefab != this.prefab;

        /// <summary> Destroys customized <see cref="customizedPrefab"/> (if it was created from <see cref="prefab"/> with custom prefab setup phase). </summary>
        private void DestroyCustomizedOriginal() { if (this.HasCustomizedOriginal) Destroy(this.customizedPrefab.gameObject); }

        /// <summary>
        /// Instantiates customized <see cref="customizedPrefab"/> (for new pooled object instances) from <see cref="prefab"/>
        /// which prepared by <see cref="IPoolPrefabAware.OnPoolPrefab"/> callbacks for better performance.
        /// </summary>
        private PooledObject InstantiateCustomizedOriginal()
        {
            var original = Instantiate(this.prefab, this.transform);
            original.name = this.prefab.name; // remove (Clone)
            original.gameObject.SetActive(false);
            original.gameObject.ForEachComponentInChildren<IPoolPrefabAware>(true, c => c.OnPoolPrefab());
            return original;
        }
    }
}

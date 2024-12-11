// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    using System.Runtime.CompilerServices;
    using Eco.Shared.Pools;
    using UnityEngine;

    public static class PooledObjectOwnerExtensions
    {
        /// <summary> Returns all <see cref="IPooledObjectOwner.UsedPooledObjects"/> back to pool. It doesn't clear collection, because they will be removed from collection within <see cref="PoolOrDestroy(UnityEngine.GameObject)"/> call. </summary>
        public static void ReturnPooledObjects(this IPooledObjectOwner owner)
        {
            // "remove" all used objects from list and return them to pool
            var pooledObjects = owner.UsedPooledObjects.RentSnapshotAndClear();
            if (pooledObjects == null) return;

            for(var i = pooledObjects.Count - 1; i >= 0; i--)
            {
                var pooledObject = pooledObjects[i];
                // check if pooled objects was already destroyed (i.e. we called this from OnDestroy and pooled object was one of children)
                if (pooledObject != null)
                {
                    pooledObject.owner = null;
                    pooledObject.PoolOrDestroy();
                }
            }
            ListPool<PooledObject>.Shared.Return(pooledObjects);
        }

        // returns true if the object was pooled; false otherwise
        public static bool PoolOrDestroy(this GameObject gameObject)
        {
            // Recycle ourselves and detach from parent if we recycle.
            if (gameObject.TryGetComponent<PooledObject>(out var pooledObject))
                return pooledObject.PoolOrDestroy();

            gameObject.DetachAndDestroy();
            return false;
        }

        /// <summary> Same as <see cref="PoolOrDestroy(UnityEngine.GameObject)"/>, but checks if <paramref name="gameObject"/> is not null. </summary>
        public static bool PoolOrDestroyIfSet(this GameObject gameObject) => gameObject != null && gameObject.PoolOrDestroy();

        /// <summary> Same as <see cref="PoolOrDestroy(UnityEngine.GameObject)"/>, but checks if <paramref name="component"/> is not null. </summary>
        public static bool PoolOrDestroyIfSet(this Component component) => component != null && component.gameObject.PoolOrDestroy();

        /// <summary> Returns <paramref name="pooledObject"/> back to pool (if it has associated pool) or destroys it. It will return <c>true</c> if object was added to pool and <c>false</c> if it was destroyed. </summary>
        public static bool PoolOrDestroy(this PooledObject pooledObject)
        {
            pooledObject.gameObject.SetActive(false); //Set it inactive for optimizing internal unity cache for disabled objects (i.e. Graphics)

            //If pool set and alive (not Destroyed) then try return to pool
            if (pooledObject.Pool != null)
            {
                if (pooledObject.Pool.TryAdd(pooledObject))
                    return true;
            }

            //otherwise release owner reference and destroy game object
            pooledObject.owner?.UsedPooledObjects.Remove(pooledObject);
            pooledObject.gameObject.DetachAndDestroy(); //replace with assert?
            return false;
        }

        /// <summary>
        /// Instantiates <paramref name="prefab"/> or optionally if pooling supported gets instance from pool (see <see cref="RentFromPool{T}"/>).
        /// <paramref name="owner"/> used only for pooling and will automatically return all rented objects back to pool.
        /// Because this method works both with poolable and non-poolable objects it requires <paramref name="parent"/> transform to ensure instantiated object will be
        /// attached to <paramref name="parent"/> and destroyed together with parent to avoid orphan game objects.
        /// </summary>
        public static GameObject PoolInstantiate(this GameObject prefab, Transform parent, bool setActive = true, IPooledObjectOwner owner = null)
        {
            if (prefab.TryGetComponent<PooledObject>(out var pooledObject))
                return pooledObject.RentFromPool(parent, setActive, owner).gameObject;
            if (!setActive)
                prefab.SetActive(false);
            var instance = Object.Instantiate(prefab, parent, false);
            if (setActive)
                instance.SetActive(true);
            return instance;
        }

        /// <summary>
        /// Instantiates <paramref name="prefab"/> or optionally if pooling supported gets instance from pool (see <see cref="RentFromPool{T}"/>).
        /// <paramref name="owner"/> used only for pooling and will automatically return all rented objects back to pool.
        /// Because this method works both with poolable and non-poolable objects it requires <paramref name="parent"/> transform to ensure instantiated object will be
        /// attached to <paramref name="parent"/> and destroyed together with parent to avoid orphan game objects.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject PoolInstantiate(this IPooledObjectOwner owner, GameObject prefab, Transform parent, bool setActive = true) => prefab.PoolInstantiate(parent, setActive, owner);

        /// <summary> Gets instance of game object with <see cref="PoolInstantiate(UnityEngine.GameObject,UnityEngine.Transform,bool,IPooledObjectOwner)"/> for <paramref name="prefab"/>'s game object prefab and then returns same component for an instance. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PoolInstantiate<T>(this IPooledObjectOwner owner, T prefab, Transform parent, bool setActive = true) where T : Component => prefab.gameObject.PoolInstantiate(parent, setActive, owner).GetComponent<T>();

        // Create pooled object via component
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RentFromPool<T>(this IPooledObjectOwner owner, T prefab, Transform parent = null, bool setActive = true) where T : TrackableBehavior =>
            RentFromPool(owner, prefab.gameObject, parent, setActive).GetComponent<T>();

        /// <summary>
        /// Rents from pool game object (instance of <paramref name="prefab"/>). Optionally set <paramref name="parent"/> and may make an instance active.
        /// For UI this method requires <see cref="IPooledObjectOwner"/> to be specified to provide safe OnDestroy tracking in order to release all pooled objects before destruction
        /// use setActive = false, if complex init is required - it is much faster to fill disabled object that enabled.
        /// </summary>
        public static GameObject RentFromPool(this IPooledObjectOwner owner, GameObject prefab, Transform parent = null, bool setActive = true) => prefab.GetComponent<PooledObject>().RentFromPool(parent, setActive, owner).gameObject;

        /// <summary>
        /// Rents from pool game object (instance of <paramref name="prefab"/>) and returns it's <see cref="PooledObject"/> component.
        /// It may be:
        /// <ul>
        /// <li>optionally associated with <paramref name="owner"/>;</li>
        /// <li>optionally attached to <paramref name="parent"/>;</li>
        /// <li>optionally set active (with <paramref name="setActive"/>).</li>
        /// </ul>
        /// </summary>
        public static PooledObject RentFromPool(this PooledObject prefab, Transform parent = null, bool setActive = true, IPooledObjectOwner owner = null) => prefab.GetOrCreatePool().Get(parent, setActive, owner);
    }
}

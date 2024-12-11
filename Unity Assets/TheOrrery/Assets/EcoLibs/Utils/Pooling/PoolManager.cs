// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.View;

namespace Eco.Client.Pooling
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Eco.Shared.Localization;
    using Eco.Shared.Pools;
    using Eco.Shared.Text;
    using Eco.Shared.Utils;
    using EcoEngine.MismatchDetection;
    using EcoEngine.MismatchDetection.TMPro;
    using EcoEngine.MismatchDetection.Unity;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary> Class responsible for pool management (create, mapping to prefabs etc). </summary>
    public class PoolManager
    {
        readonly Dictionary<PooledObject, ObjectPool> allPools = new Dictionary<PooledObject, ObjectPool>();
        MismatchDetectionContext             mismatchDetectionContext;

        public void AddSkipForMismatch<T>() => this.mismatchDetectionContext.SkipMismatches<T>();

        Scene poolsScene; // pools scene
        
        public void Initialize()
        {
            UtilCache.RegisterConsoleCommand(this, nameof(this.DetectPoolIssues));
            UtilCache.SubscribePostDisconnectionEventPermanently(this.ClientEvents_OnDisconnectEvent);
 
            this.mismatchDetectionContext = this.CreateMismatchDetectionContext();
            this.poolsScene                =  SceneManager.CreateScene("Pools", new CreateSceneParameters(LocalPhysicsMode.None));
        }

        void ClientEvents_OnDisconnectEvent()
        {
            if (this.poolsScene.rootCount == 0) return;
            var rootGameObjects = ListPool<GameObject>.Shared.Rent(this.poolsScene.rootCount);
            this.poolsScene.GetRootGameObjects(rootGameObjects);
            foreach (var rootGameObject in rootGameObjects)
                UnityEngine.Object.Destroy(rootGameObject);
            ListPool<GameObject>.Shared.Return(rootGameObjects);
        }

        /// <summary> Creates and configures <see cref="MismatchDetectionContext"/> with default resolvers and detectors. </summary>
        MismatchDetectionContext CreateMismatchDetectionContext()
        {
            var gameObjectMismatchResolver = new CompositeGameObjectMismatchResolver();
            // add some known default resolvers for third-party libraries and Unity standard components
            gameObjectMismatchResolver.MismatchResolvers.AddRange(new IGameObjectMismatchResolver[]
            {
                new TMProGameObjectMismatchResolver(),
                new UnityEngineGameObjectMismatchResolver(),
                new EcoEngineGameObjectMismatchResolver()
            });

            var context = new MismatchDetectionContext { OneName = "instance", OtherName = "prefab" };
            context.AddDetector(new GameObjectMismatchDetector { DefaultMismatchResolver = gameObjectMismatchResolver });
            context.AddDetector(new MonoBehaviorMismatchDetector());
            context.AddDetector(new UnityEventMismatchDetector());
            context.AddDetector(new TMPTextMismatchDetector());
            context.AddDetector(new TMPInputFieldMismatchDetector());
            context.AddDetector(new TMPDropdownMismatchDetector());
            context.SkipMismatches<Transform>();
            context.SkipMismatches<RectOffset>();
            context.SkipMismatches<Animator>();
            context.SkipMismatches<Renderer>();
            context.SkipMismatches<Material>();
            context.SkipMismatches<Mesh>();
            context.SkipMismatches<Collider>();
            context.SkipMismatches<LODGroup>();
            context.SkipMismatches<CanvasGroup>();
            context.SkipMismatches<Canvas>();
            context.SkipMismatches<Image>();

            var attributesResolver = new AttributesMemberMismatchResolver();
            foreach (var poolableType in PoolingExtensions.PoolableTypes)
                attributesResolver.ScanType(poolableType);

            context.MemberMismatchResolvers.Add(new AssemblyAccessMemberMismatchResolver(this.GetType().Assembly));
            context.MemberMismatchResolvers.Add(attributesResolver);
            return context;
        }

        /// <summary> Detects and reports any pool issues (like returned pool instance mismatches prefab). </summary>
        public void DetectPoolIssues()
        {
            try
            {
                var infoBuilder = new InfoBuilder();
                foreach (var pool in this.allPools.Values)
                    infoBuilder.AddSection(Localizer.NotLocalizedStr(pool.name), pool.DetectPoolIssues(this.mismatchDetectionContext));
                if (!infoBuilder.IsEmpty)
                    Debug.LogWarning($"Following pool issues was detected:\n{infoBuilder}");
                else
                    Debug.Log("No pool issues detected.");
            }
            finally
            {
                this.mismatchDetectionContext.Reset();
            }
        }

        /// <summary> This method intended for debugging. In example you can force pool size == 1 to intense pool usage and enforce pool side-effects. Use negative value to reset to default value. </summary>
        public void ForceMaxPoolSize(int maxPoolSize)
        {
            // if maxPoolSize negative value and defaultMaxPoolSize was modified then reset to initialMaxPoolSize, do nothing otherwise
            if (maxPoolSize < 0)
            {
                if (ObjectPool.DefaultMaxPoolSize == ObjectPool.InitialMaxPoolSize)
                    return;
                maxPoolSize = ObjectPool.InitialMaxPoolSize;
            }

            ObjectPool.DefaultMaxPoolSize = maxPoolSize;
            foreach (var entry in this.allPools)
                entry.Value.SetMaxPoolSize(maxPoolSize);
        }

        /// <summary> Checks if provided <paramref name="prefab"/> has <see cref="PooledObject"/>. If it presents then either returns already associated pool or creates and associates new pool with <see cref="PooledObject"/>. </summary>
        public void PopulatePool(GameObject prefab, int count)
        {
            if (!prefab.TryGetComponent<PooledObject>(out var pooledObject))
                return;
            this.GetOrCreatePool(pooledObject).PopulatePool(count);
        }

        /// <summary> Either returns already associated pool for <paramref name="prefab"/> or creates and associates new pool with <paramref name="prefab"/>. If <paramref name="reparentPrefab"/> set then <paramref name="prefab"/> will be moved to pool root object. </summary>
        public ObjectPool GetOrCreatePool(PooledObject prefab, bool reparentPrefab = false)
        {
            //Pooled object can override game object that should be instantiated for a prefab (for example when prefab contains some additional objects for other purposes)
            if (prefab.gameObjectPrefab != null)
                prefab = prefab.gameObjectPrefab;

            if (this.TryGetPool(prefab, out var pool))
                return pool;

            var poolName = $"{prefab.name} Pool";
            var obj = new GameObject(poolName, typeof(ContainerObject), typeof(ObjectPool));
            this.AddToPoolContainer(obj);

            pool = obj.GetComponent<ObjectPool>();
            pool.Subscribe(pool.PoolDestroyed, this.OnPoolDestroyed);
            pool.SetPrefab(prefab);
            if (prefab.maxPooledCount > 0)
                pool.SetMaxPoolSize(prefab.maxPooledCount); //Override max pool size if it is specified in prefab

            if (reparentPrefab)
                prefab.transform.SetParent(obj.transform, false);

            this.allPools.Add(prefab, pool);
            prefab.Pool = pool;
            return pool;
        }

        /// <summary> Tries to get pool (if existing) for <paramref name="prefab"/>. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetPool(PooledObject prefab, out ObjectPool pool)
        {
            if (!ReferenceEquals(prefab.Pool, null))
                pool = prefab.Pool;
            else if (!this.allPools.TryGetValue(prefab, out pool))
                return false;
            return true;
        }

        /// <summary> Callback from <see cref="ObjectPool"/>. </summary>
        internal void OnPoolDestroyed(ObjectPool pool)
        {
            // detach prefab from pool
            pool.prefab.Pool = null;
            // when pool released it de-registered and destroyed
            this.allPools.Remove(pool.prefab);
        }

        // Finds pool associated with given prefab and clear it.
        // ! Only available objects would be cleared, does not affect owned objects !
        public void ClearPoolContainingPrefab(GameObject prefab, bool resetOriginal = false)
        {
            if (prefab == null) return;
            if (prefab.TryGetComponent<PooledObject>(out var pooledObject))
                this.ClearPoolContainingPrefab(pooledObject, resetOriginal);
        }

        // Finds pool associated with given prefab and clear it.
        // ! Only available objects would be cleared, does not affect owned objects !
        public void ClearPoolContainingPrefab(PooledObject prefab, bool resetOriginal = false)
        {
            if (prefab == null) return;
            DebugUtils.Assert(prefab.gameObjectPrefab == null, "This method has to be called for the actual prefab that is instantiated from the pool which in that case is not the root game object of the prefab.");
            if (!this.allPools.TryGetValue(prefab, out var existing))
                return;

            existing.Clear(resetOriginal);
        }

        void AddToPoolContainer(GameObject poolObj) => SceneManager.MoveGameObjectToScene(poolObj, this.poolsScene);
    }
}

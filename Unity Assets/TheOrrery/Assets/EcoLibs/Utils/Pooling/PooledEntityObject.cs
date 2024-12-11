// Inspired by the code at http://catlikecoding.com/unity/tutorials/object-pools/

namespace Eco.Client.Pooling
{
    using Unity.Entities;
    using UnityEngine;

    /// <summary>A PooledObject that can optionally contain an Entity, which is created before object is instantiated or rented from the pool and makes sure object is destroyed or returned to pool when entity is destroyed.</summary>
    public class PooledEntityObject : PooledObject
    {
        /// <summary>Name given to entities created from this object.</summary>
        /// <remarks>Works only in the editor.</remarks>
        public string EntityDebugName;

        public Entity Entity { get; set; }
        public GameObject entityPrefab;
    }
}

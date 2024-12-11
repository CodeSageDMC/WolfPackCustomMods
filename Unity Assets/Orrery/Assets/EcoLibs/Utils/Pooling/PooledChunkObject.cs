// Inspired by the code at http://catlikecoding.com/unity/tutorials/object-pools/

namespace Eco.Client.Pooling
{
    using Unity.Entities;
    using Unity.Transforms;
    using UnityEngine;

    public class PooledChunkObject : PooledEntityObject
    {
        /// <summary>When set to true all children of this game object are pooled or destroyed when it is pooled.</summary>
        /// <seealso cref="PooledObjectOwnerExtensions.PoolOrDestroy"/>
        public bool recursive = false;

        public override void OnPoolReturn()
        {
            //Resets local position and rotation which may be modified outside
            var trans = this.transform;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;

            //Recycle our children first
            if (this.recursive)
            {
                for (var i = this.gameObject.transform.childCount - 1; i >= 0; --i)
                {
                    var child = this.gameObject.transform.GetChild(i);
                    child.gameObject.PoolOrDestroy();
                }
            }

            base.OnPoolReturn();
        }
    }
}

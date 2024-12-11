// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using UnityEngine;

namespace Eco.Client.Utils
{
    /// <summary>
    /// Helper class that allows to control joints state. Sadly Unity has no built-in functionality to do this still.
    /// It works simple. Caches needed info (connected body, limits and spring) on start and then when you disable them -
    /// it just resets that to values that are not functional by internal unity system and it will disable it there.
    /// On enable it just loads working cached values
    /// </summary>
    public class JointStateController : MonoBehaviour
    {
        [Serializable]
        class JointCacheData
        {
            // Joint ref
            readonly HingeJoint Joint;
            
            // Cached data to restore when needed
            readonly Rigidbody ConnectedBodyCache;
            readonly bool UseSpringCache;
            readonly JointLimits JointLimitsCache;

            public JointCacheData(HingeJoint joint, Rigidbody connectedBody, JointLimits limits, bool useSpring)
            {
                this.ConnectedBodyCache = connectedBody;
                this.UseSpringCache = useSpring;
                this.JointLimitsCache = limits;
                this.Joint = joint;
            }
        
            public void Reset()
            {
                if (this.Joint.connectedBody != null) this.Joint.connectedBody.Sleep();
                this.Joint.limits = new JointLimits();
                this.Joint.connectedBody = null;
                this.Joint.useSpring = false;
            }

            public void LoadCache()
            {
                this.Joint.limits = this.JointLimitsCache;
                this.Joint.connectedBody = this.ConnectedBodyCache;
                this.Joint.useSpring = this.UseSpringCache;
                if (this.Joint.connectedBody != null) this.Joint.connectedBody.WakeUp();
            }
        }

        [SerializeField] bool DisableJointsOnStart;
        [SerializeField] HingeJoint[] JointsList;
        JointCacheData[] JointCache;
    
        void Awake()
        {
            this.JointCache = new JointCacheData[this.JointsList.Length];
            for (var i = 0; i < this.JointsList.Length; i++)
            {
                var joint = this.JointsList[i];
                this.JointCache[i] = new JointCacheData(joint, joint.connectedBody, joint.limits, joint.useSpring);
            }
        
            if (this.DisableJointsOnStart) this.DisableJoints();
        }

        // Can be called by animator to enable/disable physics for joints
        public void SetJointState(int state)
        {
            var isEnabled = state == 1;
            if (isEnabled) this.EnableJoints();
            else this.DisableJoints();
        }

        public void DisableJoints() { foreach (var jointData in this.JointCache) jointData.Reset(); }
    
        public void EnableJoints() { foreach (var jointData in this.JointCache) jointData.LoadCache(); }
    }
}

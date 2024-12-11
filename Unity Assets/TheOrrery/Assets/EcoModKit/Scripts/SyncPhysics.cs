// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

/// <summary> Describes which axis or combination, if any, to synchronize. </summary>
public enum AxisSyncMode : byte
{
    None        = 0,    // Do not sync.
    AxisX       = 1,    // Only x axis.
    AxisY       = 2,    // Only the y axis.
    AxisZ       = 3,    // Only the z axis.
    AxisXY      = 4,    // The x and y axis.
    AxisXZ      = 5,    // The x and z axis.
    AxisYZ      = 6,    // The y and z axis.
    AxisXYZ     = 7     // The x, y and z axis.
}

// Interpolates game object physics over time from network updates
public partial class SyncPhysics : TrackableBehavior
{

    [Header("Position")]
    [Tooltip("Generally avoids local physics in favor of derived velocities/positions.")]                                   public bool SyncPos;

    [Header("Velocity")]
    [Tooltip("Synchronizes the velocity of the object, in addition to position if that is selected.")]                      public bool SyncVelocity = true;
    [Tooltip("Synchronizes the velocity of the object using base velocity instead of rigidbody velocity if available.")]    public bool UseBaseVelocity = false; // BaseVelocity is the pure velocity without influence from external factors. (Such as a player being on top of a vehicle, the vehicle's velocity is not included in such velocity)
    [Tooltip("Limiter for the velocity of the object, using magnite")]                                                      public bool LimitVelocity = false;
    [Tooltip("Limiter for the velocity of the object, using magnite")]                                                      public float VelocityLimit = 9f;

    [Header("Rotation")]
    [Tooltip("Will the object synchronize rotation or not.")]                                                               public bool SyncRot;
    [Tooltip("Which axis, if any to synchronize on.")]                                                                      public AxisSyncMode RotSyncMode         = AxisSyncMode.AxisXYZ;

    [Header("General")]
    [Tooltip("If true, another script must decide when to issue an update.")]                                               public bool ManuallyUpdated             = false;
    [Tooltip("The distance to the nearest player, beyond which kinematic mode is engaged. Does not update live.")]          public float distanceToIgnorePhysics    = 25f;      // Kinematic mode stops the rigidbody from being affected by collisions, forces like gravity, and joints.
    [Tooltip("The maximum deviation between client/server before jumping to the target pos/rot.")]                          public float snapDistance               = 5f;       // If snapDistance is exceeded, interpolation is skipped and the object teleports to its target position/rotation.

    [Header("Debug")]
    [Tooltip("Does not remove older keyframes after they are used.")]                                                       public bool debugKeyframes              = false;

    [Header("Collisions")]
    [Tooltip("Limits the maximum velocity that the physics system will apply to this object when handling collisions.")]    public float maxDepenetationVelocity    = 20.0f;    // ~20.0f ensures large/heavy objects (like Old Growth Redwoods) are moved out of the way without exploding into the distance.
}

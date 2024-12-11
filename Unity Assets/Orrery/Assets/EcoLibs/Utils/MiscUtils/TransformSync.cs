// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public class TransformSync : TrackableBehavior
{
    public Transform target;
    public bool syncPosition;
    public bool syncRotation;
    public bool local;

    public Vector3Int PosSyncAxis = Vector3Int.one;

    void Update ()
    {
        if (this.local)
        {
            if (this.syncRotation) this.transform.localRotation = this.target.localRotation;
            if (this.syncPosition)
                this.transform.localPosition = this.GetAxisPosition(this.transform.localPosition, this.target.localPosition, this.PosSyncAxis);
            
        }
        else
        {
            if (this.syncRotation) this.transform.rotation = this.target.rotation;
            if (this.syncPosition)
                this.transform.position = this.GetAxisPosition(this.transform.position, this.target.position, this.PosSyncAxis);
        }
    }

    float GetAxisValues(float current, float next, int mult) => mult == 0 ? current : next * mult;

    Vector3 GetAxisPosition(Vector3 current, Vector3 next, Vector3Int axis)
    {
        var x = this.GetAxisValues(current.x, next.x, axis.x);
        var y = this.GetAxisValues(current.y, next.y, axis.y);
        var z = this.GetAxisValues(current.z, next.z, axis.z);
        return new Vector3(x, y, z);
    }
}

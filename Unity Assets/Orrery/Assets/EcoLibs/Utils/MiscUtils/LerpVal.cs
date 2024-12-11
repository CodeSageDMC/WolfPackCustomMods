// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared;
using Eco.Shared.Utils;
using System;

//A class for lerping with acceleration and velocity.
[Serializable]
public class LerpVal
{
    public float Accel = 1f;
    public float MaxVel = 1f;
    public float Velocity = 0f;

    public float Value = 0f;
    public float Target = 0f;

    float timeToReachTarget = .5f;

    void Update(float time)
    {
        float distToTarget = Target - Value;
        var desiredVel = distToTarget / timeToReachTarget;
        desiredVel = Mathf.Clamp(desiredVel, -MaxVel, MaxVel);

        Velocity = MathUtil.Approach(Velocity, desiredVel, Accel * time);
        Value += Velocity * time;
    }
}

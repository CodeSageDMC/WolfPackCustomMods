// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public struct Cached<T>
{
    const float DefaultValidDuration = 10f;
    const float ExtraExpireTime = 60;

    public Cached(T value)
    {
        this.ExpireTime = DefaultValidDuration + Time.time;
        this.value = value;
    }

    public Cached(T value, float validDuration)
    {
        this.ExpireTime = validDuration + Time.time;
        this.value = value;
    }


    public void ForceExpire() => this.ExpireTime = Time.time;

    /// <summary>We leave expired entries to serve as a 'best guess' until they are 'extra expired' at which point we remove them.</summary>
    public bool IsExtraExpired() => Time.time >= this.ExpireTime + ExtraExpireTime;

    public float ExpireTime;
    private T value;

    public bool Expired { get { return Time.time >= this.ExpireTime; } }
    public T Value      { get { return this.value; } set { this.ExpireTime = Time.time; this.value = value; } }
}

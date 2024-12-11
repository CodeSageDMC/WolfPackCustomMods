// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

//Util for attaching generic data to a monobehaviour through a new component
public class AttachedData : TrackableBehavior
{
    public Dictionary<object, object> Data = new Dictionary<object, object>();
}

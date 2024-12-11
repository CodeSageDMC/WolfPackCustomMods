// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public static class DestroyHelper
{
    public static void Destroy(GameObject o, bool forceDestroyImmediate = false)
    {
        //Remove it immediately from the parent, so subsequent child examinations won't find it.
        o.transform.SetParent(null);

#if UNITY_EDITOR
        if (Application.isPlaying && !forceDestroyImmediate) GameObject.Destroy(o);
        else                                                 GameObject.DestroyImmediate(o);
#else
        if (forceDestroyImmediate)                           GameObject.DestroyImmediate(o);
        else                                                 GameObject.Destroy(o);
#endif
    }
}


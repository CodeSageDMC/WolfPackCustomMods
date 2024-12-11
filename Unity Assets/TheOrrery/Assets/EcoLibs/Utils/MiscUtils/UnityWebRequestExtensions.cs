// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using UnityEngine.Networking;

    public static class UnityWebRequestExtensions
    {
        public static bool IsFailed(this UnityWebRequest request)
        {
#if UNITY_2020_2_OR_NEWER
            return request.result switch { UnityWebRequest.Result.InProgress => false, UnityWebRequest.Result.Success => false, _ => true };
#else
            return request.isNetworkError || request.isHttpError;
#endif
        }
    }
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using System.Collections.Generic;

    public static class ListExtensions
    {
        /// <summary>Ensures that capacity of list is greater or equal to passed capacity.</summary>
        public static void EnsureCapacity<T>(this List<T> list, int capacity)
        {
            if (list.Capacity < capacity) list.Capacity = capacity;
        }
    }
}

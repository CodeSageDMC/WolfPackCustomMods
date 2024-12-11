// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    using System;

    /// <summary> Marker attribute which marks class as supporting pooling without implementing any pool aware interface like <see cref="IPoolRentAware"/> or <see cref="IPoolReturnAware"/>. </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SupportsPoolingAttribute : Attribute { }
}

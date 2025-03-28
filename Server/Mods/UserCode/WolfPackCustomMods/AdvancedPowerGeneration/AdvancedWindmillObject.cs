﻿// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Mods.TechTree
{
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Shared.Math;
    using System;
    using System.Collections.Generic;



    public partial class AdvancedWindmillObject
    {
        static AdvancedWindmillObject()
        {
            WorldObject.AddOccupancy<AdvancedWindmillObject>(new List<BlockOccupancy>()
            {
                new BlockOccupancy(new Vector3i(-2, 0, 0)),
                new BlockOccupancy(new Vector3i(-1, 0, 0)),
                new BlockOccupancy(new Vector3i(0, 0, 0)),
                new BlockOccupancy(new Vector3i(1, 0, 0)),
                new BlockOccupancy(new Vector3i(2, 0, 0)),
                new BlockOccupancy(new Vector3i(-2, 0, 1)),
                new BlockOccupancy(new Vector3i(-1, 0, 1)),
                new BlockOccupancy(new Vector3i(0, 0, 1)),
                new BlockOccupancy(new Vector3i(1, 0, 1)),
                new BlockOccupancy(new Vector3i(2, 0, 1)),
                new BlockOccupancy(new Vector3i(-2, 0, 2)),
                new BlockOccupancy(new Vector3i(-1, 0, 2)),
                new BlockOccupancy(new Vector3i(0, 0, 2)),
                new BlockOccupancy(new Vector3i(1, 0, 2)),
                new BlockOccupancy(new Vector3i(2, 0, 2)),
                new BlockOccupancy(new Vector3i(-2, 0, 3)),
                new BlockOccupancy(new Vector3i(-1, 0, 3)),
                new BlockOccupancy(new Vector3i(0, 0, 3)),
                new BlockOccupancy(new Vector3i(1, 0, 3)),
                new BlockOccupancy(new Vector3i(2, 0, 3)),
                new BlockOccupancy(new Vector3i(-2, 0, 4)),
                new BlockOccupancy(new Vector3i(-1, 0, 4)),
                new BlockOccupancy(new Vector3i(0, 0, 4)),
                new BlockOccupancy(new Vector3i(1, 0, 4)),
                new BlockOccupancy(new Vector3i(2, 0, 4)),

                new BlockOccupancy(new Vector3i(-2, 1, 0)),
                new BlockOccupancy(new Vector3i(-1, 1, 0)),
                new BlockOccupancy(new Vector3i(0, 1, 0)),
                new BlockOccupancy(new Vector3i(1, 1, 0)),
                new BlockOccupancy(new Vector3i(2, 1, 0)),
                new BlockOccupancy(new Vector3i(-2, 1, 1)),
                new BlockOccupancy(new Vector3i(-1, 1, 1)),
                new BlockOccupancy(new Vector3i(0, 1, 1)),
                new BlockOccupancy(new Vector3i(1, 1, 1)),
                new BlockOccupancy(new Vector3i(2, 1, 1)),
                new BlockOccupancy(new Vector3i(-2, 1, 2)),
                new BlockOccupancy(new Vector3i(-1, 1, 2)),
                new BlockOccupancy(new Vector3i(0, 1, 2)),
                new BlockOccupancy(new Vector3i(1, 1, 2)),
                new BlockOccupancy(new Vector3i(2, 1, 2)),
                new BlockOccupancy(new Vector3i(-2, 1, 3)),
                new BlockOccupancy(new Vector3i(-1, 1, 3)),
                new BlockOccupancy(new Vector3i(0, 1, 3)),
                new BlockOccupancy(new Vector3i(1, 1, 3)),
                new BlockOccupancy(new Vector3i(2, 1, 3)),
                new BlockOccupancy(new Vector3i(-2, 1, 4)),
                new BlockOccupancy(new Vector3i(-1, 1, 4)),
                new BlockOccupancy(new Vector3i(0, 1, 4)),
                new BlockOccupancy(new Vector3i(1, 1, 4)),
                new BlockOccupancy(new Vector3i(2, 1, 4)),

                new BlockOccupancy(new Vector3i(-2, 2, 0)),
                new BlockOccupancy(new Vector3i(-1, 2, 0)),
                new BlockOccupancy(new Vector3i(0, 2, 0)),
                new BlockOccupancy(new Vector3i(1, 2, 0)),
                new BlockOccupancy(new Vector3i(2, 2, 0)),
                new BlockOccupancy(new Vector3i(-2, 2, 1)),
                new BlockOccupancy(new Vector3i(-1, 2, 1)),
                new BlockOccupancy(new Vector3i(0, 2, 1)),
                new BlockOccupancy(new Vector3i(1, 2, 1)),
                new BlockOccupancy(new Vector3i(2, 2, 1)),
                new BlockOccupancy(new Vector3i(-2, 2, 2)),
                new BlockOccupancy(new Vector3i(-1, 2, 2)),
                new BlockOccupancy(new Vector3i(0, 2, 2)),
                new BlockOccupancy(new Vector3i(1, 2, 2)),
                new BlockOccupancy(new Vector3i(2, 2, 2)),
                new BlockOccupancy(new Vector3i(-2, 2, 3)),
                new BlockOccupancy(new Vector3i(-1, 2, 3)),
                new BlockOccupancy(new Vector3i(0, 2, 3)),
                new BlockOccupancy(new Vector3i(1, 2, 3)),
                new BlockOccupancy(new Vector3i(2, 2, 3)),
                new BlockOccupancy(new Vector3i(-2, 2, 4)),
                new BlockOccupancy(new Vector3i(-1, 2, 4)),
                new BlockOccupancy(new Vector3i(0, 2, 4)),
                new BlockOccupancy(new Vector3i(1, 2, 4)),
                new BlockOccupancy(new Vector3i(2, 2, 4)),

                new BlockOccupancy(new Vector3i(-2, 3, 0)),
                new BlockOccupancy(new Vector3i(-1, 3, 0)),
                new BlockOccupancy(new Vector3i(0, 3, 0)),
                new BlockOccupancy(new Vector3i(1, 3, 0)),
                new BlockOccupancy(new Vector3i(2, 3, 0)),
                new BlockOccupancy(new Vector3i(-2, 3, 1)),
                new BlockOccupancy(new Vector3i(-1, 3, 1)),
                new BlockOccupancy(new Vector3i(0, 3, 1)),
                new BlockOccupancy(new Vector3i(1, 3, 1)),
                new BlockOccupancy(new Vector3i(2, 3, 1)),
                new BlockOccupancy(new Vector3i(-2, 3, 2)),
                new BlockOccupancy(new Vector3i(-1, 3, 2)),
                new BlockOccupancy(new Vector3i(0, 3, 2)),
                new BlockOccupancy(new Vector3i(1, 3, 2)),
                new BlockOccupancy(new Vector3i(2, 3, 2)),
                new BlockOccupancy(new Vector3i(-2, 3, 3)),
                new BlockOccupancy(new Vector3i(-1, 3, 3)),
                new BlockOccupancy(new Vector3i(0, 3, 3)),
                new BlockOccupancy(new Vector3i(1, 3, 3)),
                new BlockOccupancy(new Vector3i(2, 3, 3)),
                new BlockOccupancy(new Vector3i(-2, 3, 4)),
                new BlockOccupancy(new Vector3i(-1, 3, 4)),
                new BlockOccupancy(new Vector3i(0, 3, 4)),
                new BlockOccupancy(new Vector3i(1, 3, 4)),
                new BlockOccupancy(new Vector3i(2, 3, 4)),

                new BlockOccupancy(new Vector3i(-2, 4, 0)),
                new BlockOccupancy(new Vector3i(-1, 4, 0)),
                new BlockOccupancy(new Vector3i(0, 4, 0)),
                new BlockOccupancy(new Vector3i(1, 4, 0)),
                new BlockOccupancy(new Vector3i(2, 4, 0)),
                new BlockOccupancy(new Vector3i(-2, 4, 1)),
                new BlockOccupancy(new Vector3i(-1, 4, 1)),
                new BlockOccupancy(new Vector3i(0, 4, 1)),
                new BlockOccupancy(new Vector3i(1, 4, 1)),
                new BlockOccupancy(new Vector3i(2, 4, 1)),
                new BlockOccupancy(new Vector3i(-2, 4, 2)),
                new BlockOccupancy(new Vector3i(-1, 4, 2)),
                new BlockOccupancy(new Vector3i(0, 4, 2)),
                new BlockOccupancy(new Vector3i(1, 4, 2)),
                new BlockOccupancy(new Vector3i(2, 4, 2)),
                new BlockOccupancy(new Vector3i(-2, 4, 3)),
                new BlockOccupancy(new Vector3i(-1, 4, 3)),
                new BlockOccupancy(new Vector3i(0, 4, 3)),
                new BlockOccupancy(new Vector3i(1, 4, 3)),
                new BlockOccupancy(new Vector3i(2, 4, 3)),
                new BlockOccupancy(new Vector3i(-2, 4, 4)),
                new BlockOccupancy(new Vector3i(-1, 4, 4)),
                new BlockOccupancy(new Vector3i(0, 4, 4)),
                new BlockOccupancy(new Vector3i(1, 4, 4)),
                new BlockOccupancy(new Vector3i(2, 4, 4)),

                new BlockOccupancy(new Vector3i(-2, 5, 0)),
                new BlockOccupancy(new Vector3i(-1, 5, 0)),
                new BlockOccupancy(new Vector3i(0, 5, 0)),
                new BlockOccupancy(new Vector3i(1, 5, 0)),
                new BlockOccupancy(new Vector3i(2, 5, 0)),
                new BlockOccupancy(new Vector3i(-2, 5, 1)),
                new BlockOccupancy(new Vector3i(-1, 5, 1)),
                new BlockOccupancy(new Vector3i(0, 5, 1)),
                new BlockOccupancy(new Vector3i(1, 5, 1)),
                new BlockOccupancy(new Vector3i(2, 5, 1)),
                new BlockOccupancy(new Vector3i(-2, 5, 2)),
                new BlockOccupancy(new Vector3i(-1, 5, 2)),
                new BlockOccupancy(new Vector3i(0, 5, 2)),
                new BlockOccupancy(new Vector3i(1, 5, 2)),
                new BlockOccupancy(new Vector3i(2, 5, 2)),
                new BlockOccupancy(new Vector3i(-2, 5, 3)),
                new BlockOccupancy(new Vector3i(-1, 5, 3)),
                new BlockOccupancy(new Vector3i(0, 5, 3)),
                new BlockOccupancy(new Vector3i(1, 5, 3)),
                new BlockOccupancy(new Vector3i(2, 5, 3)),
                new BlockOccupancy(new Vector3i(-2, 5, 4)),
                new BlockOccupancy(new Vector3i(-1, 5, 4)),
                new BlockOccupancy(new Vector3i(0, 5, 4)),
                new BlockOccupancy(new Vector3i(1, 5, 4)),
                new BlockOccupancy(new Vector3i(2, 5, 4)),

                new BlockOccupancy(new Vector3i(-2, 6, 0)),
                new BlockOccupancy(new Vector3i(-1, 6, 0)),
                new BlockOccupancy(new Vector3i(0, 6, 0)),
                new BlockOccupancy(new Vector3i(1, 6, 0)),
                new BlockOccupancy(new Vector3i(2, 6, 0)),
                new BlockOccupancy(new Vector3i(-2, 6, 1)),
                new BlockOccupancy(new Vector3i(-1, 6, 1)),
                new BlockOccupancy(new Vector3i(0, 6, 1)),
                new BlockOccupancy(new Vector3i(1, 6, 1)),
                new BlockOccupancy(new Vector3i(2, 6, 1)),
                new BlockOccupancy(new Vector3i(-2, 6, 2)),
                new BlockOccupancy(new Vector3i(-1, 6, 2)),
                new BlockOccupancy(new Vector3i(0, 6, 2)),
                new BlockOccupancy(new Vector3i(1, 6, 2)),
                new BlockOccupancy(new Vector3i(2, 6, 2)),
                new BlockOccupancy(new Vector3i(-2, 6, 3)),
                new BlockOccupancy(new Vector3i(-1, 6, 3)),
                new BlockOccupancy(new Vector3i(0, 6, 3)),
                new BlockOccupancy(new Vector3i(1, 6, 3)),
                new BlockOccupancy(new Vector3i(2, 6, 3)),
                new BlockOccupancy(new Vector3i(-2, 6, 4)),
                new BlockOccupancy(new Vector3i(-1, 6, 4)),
                new BlockOccupancy(new Vector3i(0, 6, 4)),
                new BlockOccupancy(new Vector3i(1, 6, 4)),
                new BlockOccupancy(new Vector3i(2, 6, 4)),

                new BlockOccupancy(new Vector3i(-2, 7, 0)),
                new BlockOccupancy(new Vector3i(-1, 7, 0)),
                new BlockOccupancy(new Vector3i(0, 7, 0)),
                new BlockOccupancy(new Vector3i(1, 7, 0)),
                new BlockOccupancy(new Vector3i(2, 7, 0)),
                new BlockOccupancy(new Vector3i(-2, 7, 1)),
                new BlockOccupancy(new Vector3i(-1, 7, 1)),
                new BlockOccupancy(new Vector3i(0, 7, 1)),
                new BlockOccupancy(new Vector3i(1, 7, 1)),
                new BlockOccupancy(new Vector3i(2, 7, 1)),
                new BlockOccupancy(new Vector3i(-2, 7, 2)),
                new BlockOccupancy(new Vector3i(-1, 7, 2)),
                new BlockOccupancy(new Vector3i(0, 7, 2)),
                new BlockOccupancy(new Vector3i(1, 7, 2)),
                new BlockOccupancy(new Vector3i(2, 7, 2)),
                new BlockOccupancy(new Vector3i(-2, 7, 3)),
                new BlockOccupancy(new Vector3i(-1, 7, 3)),
                new BlockOccupancy(new Vector3i(0, 7, 3)),
                new BlockOccupancy(new Vector3i(1, 7, 3)),
                new BlockOccupancy(new Vector3i(2, 7, 3)),
                new BlockOccupancy(new Vector3i(-2, 7, 4)),
                new BlockOccupancy(new Vector3i(-1, 7, 4)),
                new BlockOccupancy(new Vector3i(0, 7, 4)),
                new BlockOccupancy(new Vector3i(1, 7, 4)),
                new BlockOccupancy(new Vector3i(2, 7, 4)),

                new BlockOccupancy(new Vector3i(-2, 8, 0)),
                new BlockOccupancy(new Vector3i(-1, 8, 0)),
                new BlockOccupancy(new Vector3i(0, 8, 0)),
                new BlockOccupancy(new Vector3i(1, 8, 0)),
                new BlockOccupancy(new Vector3i(2, 8, 0)),
                new BlockOccupancy(new Vector3i(-2, 8, 1)),
                new BlockOccupancy(new Vector3i(-1, 8, 1)),
                new BlockOccupancy(new Vector3i(0, 8, 1)),
                new BlockOccupancy(new Vector3i(1, 8, 1)),
                new BlockOccupancy(new Vector3i(2, 8, 1)),
                new BlockOccupancy(new Vector3i(-2, 8, 2)),
                new BlockOccupancy(new Vector3i(-1, 8, 2)),
                new BlockOccupancy(new Vector3i(0, 8, 2)),
                new BlockOccupancy(new Vector3i(1, 8, 2)),
                new BlockOccupancy(new Vector3i(2, 8, 2)),
                new BlockOccupancy(new Vector3i(-2, 8, 3)),
                new BlockOccupancy(new Vector3i(-1, 8, 3)),
                new BlockOccupancy(new Vector3i(0, 8, 3)),
                new BlockOccupancy(new Vector3i(1, 8, 3)),
                new BlockOccupancy(new Vector3i(2, 8, 3)),
                new BlockOccupancy(new Vector3i(-2, 8, 4)),
                new BlockOccupancy(new Vector3i(-1, 8, 4)),
                new BlockOccupancy(new Vector3i(0, 8, 4)),
                new BlockOccupancy(new Vector3i(1, 8, 4)),
                new BlockOccupancy(new Vector3i(2, 8, 4)),

                new BlockOccupancy(new Vector3i(-2, 9, 0)),
                new BlockOccupancy(new Vector3i(-1, 9, 0)),
                new BlockOccupancy(new Vector3i(0, 9, 0)),
                new BlockOccupancy(new Vector3i(1, 9, 0)),
                new BlockOccupancy(new Vector3i(2, 9, 0)),
                new BlockOccupancy(new Vector3i(-2, 9, 1)),
                new BlockOccupancy(new Vector3i(-1, 9, 1)),
                new BlockOccupancy(new Vector3i(0, 9, 1)),
                new BlockOccupancy(new Vector3i(1, 9, 1)),
                new BlockOccupancy(new Vector3i(2, 9, 1)),
                new BlockOccupancy(new Vector3i(-2, 9, 2)),
                new BlockOccupancy(new Vector3i(-1, 9, 2)),
                new BlockOccupancy(new Vector3i(0, 9, 2)),
                new BlockOccupancy(new Vector3i(1, 9, 2)),
                new BlockOccupancy(new Vector3i(2, 9, 2)),
                new BlockOccupancy(new Vector3i(-2, 9, 3)),
                new BlockOccupancy(new Vector3i(-1, 9, 3)),
                new BlockOccupancy(new Vector3i(0, 9, 3)),
                new BlockOccupancy(new Vector3i(1, 9, 3)),
                new BlockOccupancy(new Vector3i(2, 9, 3)),
                new BlockOccupancy(new Vector3i(-2, 9, 4)),
                new BlockOccupancy(new Vector3i(-1, 9, 4)),
                new BlockOccupancy(new Vector3i(0, 9, 4)),
                new BlockOccupancy(new Vector3i(1, 9, 4)),
                new BlockOccupancy(new Vector3i(2, 9, 4)),

            });

        }
    }
}

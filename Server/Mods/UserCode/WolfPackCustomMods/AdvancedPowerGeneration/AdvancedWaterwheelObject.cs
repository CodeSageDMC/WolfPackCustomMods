// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Mods.TechTree
{
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Shared.Math;
    using System;
    using System.Collections.Generic;


    public partial class AdvancedWaterwheelItem : WorldObjectItem<AdvancedWaterwheelObject>
    {
        public override Type[] Blockers { get { return AllowWaterPlacement; } }             // This allows the AdvancedWaterwheel to be placed inside of water.
    }

    public partial class AdvancedWaterwheelObject : WorldObject
    {
        static AdvancedWaterwheelObject()
        {



            WorldObject.AddOccupancy<AdvancedWaterwheelObject>(new List<BlockOccupancy>(){
            new BlockOccupancy(new Vector3i(0, -3, -3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -3, -2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -3, -1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -3, 0), typeof(WaterWorldObjectBlock)),                 // Bottom block.
            new BlockOccupancy(new Vector3i(0, -3, 1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -3, 2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -3, 3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -2, -3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -2, -2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -2, -1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -2, 0), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -2, 1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -2, 2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -2, 3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -1, -3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -1, -2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -1, -1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -1, 0), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -1, 1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -1, 2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, -1, 3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 0, -3), typeof(WaterWorldObjectBlock)),                 // Left block.
            new BlockOccupancy(new Vector3i(0, 0, -2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 0, -1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 0, 0), typeof(WaterWorldObjectBlock), Quaternion.Identity, BlockOccupancyType.CustomSurfaceAttachment),// Center block.
            new BlockOccupancy(new Vector3i(0, 0, 1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 0, 2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 0, 3), typeof(WaterWorldObjectBlock)),                  // Right block.
            new BlockOccupancy(new Vector3i(0, 1, -3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 1, -2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 1, -1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 1, 0), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 1, 1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 1, 2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 1, 3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 2, -3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 2, -2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 2, -1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 2, 0), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 2, 1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 2, 2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 2, 3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 3, -3), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 3, -2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 3, -1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 3, 0), typeof(WaterWorldObjectBlock)),                  // Top block.
            new BlockOccupancy(new Vector3i(0, 3, 1), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 3, 2), typeof(WaterWorldObjectBlock)),
            new BlockOccupancy(new Vector3i(0, 3, 3), typeof(WaterWorldObjectBlock)),
            });

        }
    }
}

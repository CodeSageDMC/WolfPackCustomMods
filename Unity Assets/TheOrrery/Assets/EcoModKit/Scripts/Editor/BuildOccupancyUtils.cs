// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoModKitEditor.Occupancy.Internal
{
#if UNITY_EDITOR
    using System.Text;
    using EcoModKit.Occupancy;
    using UnityEngine;
    internal static class BuildOccupancyUtils
    {
        /// <summary> Generates the code for the given custom surface requirement</summary>
        public static string GetPositionsRequirementCode(CustomPositionsRequirement surfaceRequirement)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("new List<PositionsRequirement>{");
            foreach (var sr in surfaceRequirement.positionsRequirements)
            {
                code.AppendLine("            new PositionsRequirement(new List<Vector3i>{");
                foreach (var pos in sr.positions)
                {
                    code.AppendLine($"            new Vector3i({pos.value.x}, {pos.value.y}, {pos.value.z}),");
                }
                //Get the requirement function if its one of predefined ones, or the placeholder for the custom one
                var requirementFunction = (int)sr.requirementType >= 0 ? SurfaceRequirements()[(int)sr.requirementType] : CustomSurfaceRequirement;

                code.AppendLine("            },");
                code.AppendLine($"            {requirementFunction},");
                code.AppendLine($"            Localizer.DoStr(\"{sr.partName}\"),");
                code.AppendLine($"            Localizer.DoStr(\"{sr.placementRequirement}\")");
                code.AppendLine("            ),");
            }
            code.AppendLine("}");

            return code.ToString();
        }

        //Default code snipets for surface requirements
        static string[] SurfaceRequirements() => new string[]
        {
        "(pos) => World.GetBlock(pos).Is<Solid>()",
        "(pos) => World.GetBlock(pos).IsWater()",
        "(pos) => World.GetBlock(pos).Is<Empty>()"
        };

        //If the surface requirement is custom, this will be used as a placeholder instead
        static string CustomSurfaceRequirement => "(pos) => true";

        public static void CopyToClipboard(string text)
        {
            TextEditor textEditor = new TextEditor();
            textEditor.text = text;
            textEditor.SelectAll();
            textEditor.Copy();
        }
    }
#endif
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BlockEditor
{
    static readonly string harvestEffectName = "Harvest";
    static readonly string pickupEffectName  = "Pickup";

    public static void DrawBlockEditor(Block block) 
    {
        try
        {
            GUILayout.BeginVertical(GUI.skin.box);
            block.Name = EditorGUILayout.TextField("Name", block.Name);
            block.Layer = EditorGUILayout.TextField("Layer", block.Layer);
            block.Tier = EditorGUILayout.IntField("Tier", block.Tier);
            block.Category = EditorGUILayout.TextField("Category", block.Category);

            block.Builder  = EditorGUILayout.ObjectField("Builder", block.Builder, typeof(ScriptableObject), false) as BlockBuilder;
            block.Material = EditorGUILayout.ObjectField("Material", block.Material, typeof(Material), false) as Material;
            block.LODTexture  = EditorGUILayout.ObjectField("LOD Texture", block.LODTexture, typeof(Texture2D), false) as Texture2D;
            OverrideMaterialTransparencyInspector(ref block.OverrideMainMaterialTransparency);

            if (block.Materials == null)
                block.Materials = new Material[0];
            if (block.OverrideSubMaterialsTransparency == null)
                block.OverrideSubMaterialsTransparency = new OverrideMaterialTransparency[0];
            int numSubMaterials = EditorGUILayout.IntField("Sub Material Count", block.Materials.Length);
            if (numSubMaterials != block.Materials.Length)
                Array.Resize(ref block.Materials, numSubMaterials);
            if (numSubMaterials != block.OverrideSubMaterialsTransparency.Length)
                Array.Resize(ref block.OverrideSubMaterialsTransparency, numSubMaterials);
            for (int subMaterial = 0; subMaterial < block.Materials.Length; subMaterial++)
            {
                block.Materials[subMaterial] = EditorGUILayout.ObjectField("Sub Material " + subMaterial, block.Materials[subMaterial], typeof(Material), false) as Material;
                OverrideMaterialTransparencyInspector(ref block.OverrideSubMaterialsTransparency[subMaterial]);
            }

            block.MinimapColor = EditorGUILayout.ColorField("Minimap Color", block.MinimapColor);

            block.IsDiggable         = EditorGUILayout.Toggle("Is Diggable", block.IsDiggable);
            block.Solid              = EditorGUILayout.Toggle("Solid", block.Solid);
            block.BuildCollider      = EditorGUILayout.Toggle("Build Collider?", block.BuildCollider);
            block.Rendered           = EditorGUILayout.Toggle("Rendered", block.Rendered);
            block.ShadowCastingMode  = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", block.ShadowCastingMode);
            block.WaterOccupancy     = EditorGUILayout.Toggle(new GUIContent("Allow Water Occupancy", "Prevents air pockets from forming underwater. Ie. Allows water to flow while underwater."), block.WaterOccupancy);
            block.WaterLoggable      = EditorGUILayout.Toggle(new GUIContent("Allow Water Logging", "Allows water to flow into this block. Should be paired with the interface or water loggable tech tree setting."), block.WaterLoggable);
            block.IsWater            = EditorGUILayout.Toggle("Water?", block.IsWater);
            block.IsEmpty            = EditorGUILayout.Toggle("Empty?", block.IsEmpty);
            block.IsLadder           = EditorGUILayout.Toggle("Ladder?", block.IsLadder);
            block.IsSlope            = EditorGUILayout.Toggle("Slope?", block.IsSlope);
            block.BlendingPriority   = EditorGUILayout.IntField("Blend Priority", block.BlendingPriority);
            block.PrefabHeightOffset = EditorGUILayout.FloatField("Prefab Height Offset", block.PrefabHeightOffset);
            block.ActualHeight       = EditorGUILayout.FloatField("Actual Height", block.ActualHeight);

            block.AudioCategory = EditorGUILayout.TextField("Audio Category", block.AudioCategory);
            block.MusicCategory = EditorGUILayout.TextField("Music Category", block.MusicCategory);

            //Harvest and pickup effects should be used interchangeably, so if one of them is missing we add it as copy of the existing one
            var hasPickupEffec   = block.EffectNames.Contains(pickupEffectName);
            var hasHarvestEffect = block.EffectNames.Contains(harvestEffectName);
            if (hasPickupEffec ^ hasHarvestEffect)
            {
                var existingEffectName = hasPickupEffec ? pickupEffectName  : harvestEffectName;
                var missingEffectName  = hasPickupEffec ? harvestEffectName : pickupEffectName;

                Array.Resize(ref block.EffectNames, block.EffectNames.Length + 1);   //Resize arrays to fit new effect
                Array.Resize(ref block.Effects,     block.Effects.Length + 1);
                block.EffectNames[block.EffectNames.Length - 1] = missingEffectName; //Add name of the missing effect
                block.Effects[block.Effects.Length - 1]         = block.Effects[Array.IndexOf(block.EffectNames, existingEffectName)]; //Copy effect game object reference from existing effect
            }

            GUILayout.Label("Effects");
            EditorGUI.indentLevel++;
            EditorGUIUtils.Draw2Arrays(
                ref block.EffectNames,
                ref block.Effects,
                DrawString,
                DrawGameObject);
            EditorGUI.indentLevel--;
        }
        finally
        {
            GUILayout.EndVertical();
        }
	}

    private static void OverrideMaterialTransparencyInspector(ref OverrideMaterialTransparency overrideMaterialTransparency)
    {
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();
        overrideMaterialTransparency = EditorGUILayout.Toggle(new GUIContent("Override"), overrideMaterialTransparency != OverrideMaterialTransparency.NotOverride)
            ? EditorGUILayout.Toggle(new GUIContent("Transparent"), overrideMaterialTransparency == OverrideMaterialTransparency.ForceTransparent)
                ? OverrideMaterialTransparency.ForceTransparent
                : OverrideMaterialTransparency.ForceOpaque
            : OverrideMaterialTransparency.NotOverride;
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }

    private static string DrawString(string val) { return EditorGUILayout.TextField(val); }

    private static GameObject DrawGameObject(GameObject val) { return EditorGUILayout.ObjectField(val, typeof(GameObject), false) as GameObject; }
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

/// <summary>
/// <para>Custom loader for overriding the standard TMP_Text font asset loader (via Resorces.Load<>)
/// Allows to load and create runtime font assets.</para>
/// <para>See the Eco wiki page available <a href="https://wiki.play.eco/en/Custom_Fonts">here</a> for more info.</para>
/// </summary>
public static class FontAssetLoader
{
    private static Dictionary<string, TMP_FontAsset> LoadedFontAssets       = new Dictionary<string, TMP_FontAsset>();  // All loaded font assets
    private static List<TMP_FontAsset>               CreatedFontAssets      = new List<TMP_FontAsset>();                // Runtime only font assets, need for cleanup

    private static bool fontAssetLoaderInitialized = false;

    /// <summary>Enables the FontAssetLoader for TMP_Text.</summary>
    public static void EnableFontAssetLoader()
    {
        if (!fontAssetLoaderInitialized)
        {
            TMP_Text.OnFontAssetRequest += LoadFontAsset;
            fontAssetLoaderInitialized = true;
        }
    }

    /// <summary>Checks if a font already exists in the loaded assets. IF its missing creates the asset from either a local file or a mod loaded font.</summary>
    public static void AddFont(Font font)
    {
        // check if we have the requested font
        if (!LoadedFontAssets.ContainsKey(font.name))
        {
            // first, try to load local asset if present. If not found create a new asset.
            var loaded = LoadFontAsset(0, font.name); 
            if (loaded == null) CreateTMPFontAsset(font.name, font); 
        }
    }

    /// <summary>Clears the loaded font asset files.</summary>
    public static void ClearFontAssets()
    {
        // These two dictionaries are private and only way to remove dynamic assets is to get get them with reflection
        // to perform revert of MaterialReferenceManager.AddFontAssetInternal actions

        var m_FontAssetReferenceLookup = typeof(MaterialReferenceManager).GetField("m_FontAssetReferenceLookup", BindingFlags.NonPublic | BindingFlags.Instance);
        var fontAssetReferenceLookupInternal = (Dictionary<int, TMP_FontAsset>)m_FontAssetReferenceLookup.GetValue(MaterialReferenceManager.instance);

        var m_FontMaterialReferenceLookup = typeof(MaterialReferenceManager).GetField("m_FontMaterialReferenceLookup", BindingFlags.NonPublic | BindingFlags.Instance);
        var fontMaterialReferenceLookupInternal = (Dictionary<int, Material>)m_FontMaterialReferenceLookup.GetValue(MaterialReferenceManager.instance);

        foreach (var fontAsset in CreatedFontAssets)
        {
            fontAssetReferenceLookupInternal.Remove(fontAsset.hashCode);
            fontMaterialReferenceLookupInternal.Remove(fontAsset.materialHashCode);

            Material.Destroy(fontAsset.material);
            TMP_FontAsset.Destroy(fontAsset);

            LoadedFontAssets.Remove(fontAsset.name);
        }
        CreatedFontAssets.Clear();
    }
    
    /// <summary>Attempts to load a font from the local game resource directory if its present. This method also caches the font for later use.</summary>
    private static TMP_FontAsset LoadFontAsset(int hash, string name)
    {
        if (LoadedFontAssets.TryGetValue(name, out var asset))
            return asset;

        asset = Resources.Load<TMP_FontAsset>(Path.Combine(TMP_Settings.defaultFontAssetPath, name)); // clone of TMP internal code
        if (asset != null) LoadedFontAssets[name] = asset;
        return asset;
    }

    /// <summary>Creates and registers a new <see cref="TMP_FontAsset"/> created from the provided Font instance.</summary>
    private static void CreateTMPFontAsset(string name, Font font)
    {
        var asset = TMP_FontAsset.CreateFontAsset(font);
        var material = Material.Instantiate(TMP_Settings.defaultFontAsset.material);
        material.mainTexture = asset.atlasTexture;
        asset.material = material;
        asset.name = name;

        LoadedFontAssets[name] = asset;
        CreatedFontAssets.Add(asset);
    }
}

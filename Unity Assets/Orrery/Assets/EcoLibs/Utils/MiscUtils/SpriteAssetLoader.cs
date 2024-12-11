// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.EcoLibs.Utils.MiscUtils;

// This loader will override standart TMP_Text sprite asset loader (via Resorces.Load<>)
// Allows to load and create runtime sprite assets for chat icons
// Contains couple TMP hacks
public static class SpriteAssetLoader
{
    private static Dictionary<string, TMP_SpriteAsset> LoadedSpriteAssets = new Dictionary<string, TMP_SpriteAsset>(); // All loaded sprite assets
    private static List<TMP_SpriteAsset> CreatedSpriteAssets = new List<TMP_SpriteAsset>();               // Runtime only sprite assets, need for cleanup

    private static bool spriteAssetLoaderInitialized = false;

    // will enable this loader for TMP_Text
    public static void EnableSpriteAssetLoader()
    {
        if (!spriteAssetLoaderInitialized)
        {
            TMP_Text.OnSpriteAssetRequest += LoadSpriteAsset;
            spriteAssetLoaderInitialized = true;
        }
    }

    // Check if sprite exists in loaded assets and creates missing assets
    public static TMP_Sprite AddSprite(Sprite sprite)
    {
        // check if we have sprite
        if (!LoadedSpriteAssets.TryGetValue(sprite.texture.name, out var asset))
        {
            if (asset == null) asset = LoadSpriteAsset(0, sprite.texture.name); //first, try to load local asset
            if (asset == null) asset = CreateSpriteAsset(sprite.texture.name, sprite);
        }

        int indexOfSprite = asset.GetSpriteIndexFromName(sprite.name);
        if (indexOfSprite == -1) return AddSpriteToSpriteAsset(asset, sprite); //Add sprite to TMP_SpriteAsset if it doesnt exist
        else if (indexOfSprite < asset.spriteInfoList.Count) return UpdateSpriteAsset(asset.spriteInfoList[indexOfSprite], sprite); //or update sprite data from spriteinfolist    
        return null;
    }

    // Because TMP is not supposed to use dynamic assets, this hack is needed
    // This will build sprite asset data from added sprites via AddSprite()
    public static void BuildSpriteAssets()
    {
        foreach (var asset in CreatedSpriteAssets)
        {
            if (asset.material == null) // check if it is not already built
            {
                BuildSprite(asset);
            }
        }
    }

    /// <summary>Builds TMP_SpriteAsset and updates glyph and character table</summary>
    public static void BuildSprite(TMP_SpriteAsset asset)
    {
        AddDefaultMaterial(asset);
        asset.UpdateLookupTables();

        // this hack is needed because glyphIndex is not being set after upgrade, so force set it manually
        var spriteCharacterTable = asset.spriteCharacterTable;
        for (var i = 0; i < spriteCharacterTable.Count; i++)
            spriteCharacterTable[i].glyphIndex = (uint)i;

        TMPUtils.FixFaceInfoValues(asset);
        foreach (var spriteInfo in asset.spriteGlyphTable)
            TMPUtils.FixGlyphs(spriteInfo);


        // apply changes - will recalculate glyph data, so double UpdateLookupTables call is unavoidable
        asset.UpdateLookupTables();
    }

    /// <summary>Force builds TMP_SpriteAsset</summary>
    public static void RebuildSprite(string textureName)
    {
        if (!LoadedSpriteAssets.TryGetValue(textureName, out var asset)) { Debug.LogError("TMPRO texture doesn't exist"); return; }

        //this hack causes sprite asset rebuild
        //TMP_SpriteAsset.UpgradeSpriteAsset is called when material is not empty and version is set
        //so for performance reasons material is set after all sprites initialized
        UnityEngine.Object.Destroy(asset.material);
        //The reflection is the only way to reset version value  
        var m_Version = typeof(TMP_SpriteAsset).GetField("m_Version", BindingFlags.NonPublic | BindingFlags.Instance);
        m_Version.SetValue(asset, null);
        BuildSprite(asset);
    }

    // TMP sprite assets are supposed to be local, so it does not have instruments for asset unload
    // but this is required because old textures could be destroyed
    public static void ClearSpriteAssets()
    {
        // These two dictionaries are private and only way to remove dynamic assets is to get get them with reflection
        // to perform revert of MaterialReferenceManager.AddSpriteAssetInternal actions

        var m_SpriteAssetReferenceLookup = typeof(MaterialReferenceManager).GetField("m_SpriteAssetReferenceLookup", BindingFlags.NonPublic | BindingFlags.Instance);
        var spriteAssetReferenceLookupInternal = (Dictionary<int, TMP_SpriteAsset>)m_SpriteAssetReferenceLookup.GetValue(MaterialReferenceManager.instance);

        var m_FontMaterialReferenceLookup = typeof(MaterialReferenceManager).GetField("m_FontMaterialReferenceLookup", BindingFlags.NonPublic | BindingFlags.Instance);
        var fontMaterialReferenceLookupInternal = (Dictionary<int, Material>)m_FontMaterialReferenceLookup.GetValue(MaterialReferenceManager.instance);

        foreach (var spriteAsset in CreatedSpriteAssets)
        {
            spriteAssetReferenceLookupInternal.Remove(spriteAsset.hashCode);
            fontMaterialReferenceLookupInternal.Remove(spriteAsset.hashCode);
            LoadedSpriteAssets.Remove(spriteAsset.name);
        }

        CreatedSpriteAssets.Clear();
    }

    private static TMP_SpriteAsset LoadSpriteAsset(int hash, string name)
    {
        if (LoadedSpriteAssets.TryGetValue(name, out var asset))
            return asset;

        asset = Resources.Load<TMP_SpriteAsset>(TMP_Settings.defaultSpriteAssetPath + name); // clone of TMP internal code
        LoadedSpriteAssets[name] = asset;
        return asset;
    }

    // Tricky creation of runtime TMP_SpriteAsset
    private static TMP_SpriteAsset CreateSpriteAsset(string name, Sprite sprite)
    {
        TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
        spriteAsset.name            = name;
        spriteAsset.hashCode        = TMP_TextUtilities.GetHashCode(spriteAsset.name);// Compute the hash code for the sprite asset, (make sure its the sam to how we save them in m_SpriteAssetReferenceLookup)
        spriteAsset.spriteSheet     = sprite.texture;
        spriteAsset.spriteInfoList  = new List<TMP_Sprite>();
        LoadedSpriteAssets[name]    = spriteAsset;
        CreatedSpriteAssets.Add(spriteAsset);
        return spriteAsset;
    }

    // Tricky creation of runtime TMP_SpriteAsset glyph
    private static TMP_Sprite AddSpriteToSpriteAsset(TMP_SpriteAsset spriteAsset, Sprite sprite)
    {
        // prevent key collisions
        if (spriteAsset.spriteInfoList.Exists(x => x.name == sprite.name)) return null;

        var spriteTmp = new TMP_Sprite();
        UpdateSpriteAsset(spriteTmp, sprite);

        // hack: this list is not supposed to be used like that
        // spriteInfoList is a legacy sprite info, but TMP has internal upgrader which will perform all of the work of initializing asset
        spriteAsset.spriteInfoList.Add(spriteTmp);

        TMPUtils.FixFaceInfoValues(spriteAsset);
        return spriteTmp;
    }

    private static TMP_Sprite UpdateSpriteAsset(TMP_Sprite spriteTmp, Sprite sprite)
    {
        spriteTmp.x        = sprite.rect.x;
        spriteTmp.y        = sprite.rect.y;
        spriteTmp.width    = sprite.rect.width;
        spriteTmp.height   = sprite.rect.height;
        spriteTmp.xOffset  = 0f;
        spriteTmp.yOffset  = sprite.rect.height; // The sprite is aligned under the line by default this gives it the character offset it needs in y.
        spriteTmp.xAdvance = sprite.rect.width;  // xAdvance defines how much the cursor can move after this character, this insures that no characters after the sprite hit it.
        spriteTmp.scale    = 1;
        spriteTmp.name     = sprite.name;
        if (spriteTmp.sprite == null) spriteTmp.sprite = sprite;
        else if (spriteTmp.sprite.texture != sprite.texture)
        {
            var texture = spriteTmp.sprite.texture;
            texture.SetPixels(0, 0, sprite.texture.width, sprite.texture.height, sprite.texture.GetPixels()); //Rewrites sprite pixel data to current one
            texture.Apply();
        }
        return spriteTmp;
    }

    // clone of TMP internal code
    private static void AddDefaultMaterial(TMPro.TMP_SpriteAsset spriteAsset)
    {
        Shader shader = Shader.Find("TextMeshPro/Sprite");
        Material material = new Material(shader);
        material.SetTexture(TMPro.ShaderUtilities.ID_MainTex, spriteAsset.spriteSheet);

        spriteAsset.material = material;
        material.hideFlags = HideFlags.HideInHierarchy;
    }
}

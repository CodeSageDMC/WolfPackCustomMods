// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomElements;
using System.Threading.Tasks;
using System.Text;
using System.IO;

[CustomEditor(typeof(BlockSet))]
public class BlockSetEditor : Editor
{
    VisualElement rootElement;
    VisualTreeAsset visualTree;

    BlockSet blockSet;
    Block selectedBlock;
    PreviewRenderUtility previewRenderer;

    ListView blockList;
    BlockEditorElement blockElement;
    Button newBlock;
    Button removeBlock;
    Button forceSave;
    Button changeAllAudio;
    Button generateRotations;
    Button validate;
    TextField allAudioCategory;

    readonly Dictionary<string, BlockMeshLodGroup> bmlgCache = new Dictionary<string, BlockMeshLodGroup>(1000);

    public void OnEnable()
    {
        // Hierarchy
        rootElement = new VisualElement();
        visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EcoModKit/VoxelEngine/Editor/BlockSetEditor.uxml");

        // Styles
        var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/EcoModKit/VoxelEngine/Editor/BlockSetEditor.uss");
        rootElement.styleSheets.Add(stylesheet);

        blockSet = target as BlockSet;
    }

    public void OnDisable()
    {
        previewRenderer?.Cleanup();
    }

    public override VisualElement CreateInspectorGUI()
    {
        // Reset root element and reuse.
        rootElement.Clear();

        // Turn the UXML into a VisualElement hierarchy under root.
        visualTree.CloneTree(rootElement);

        // Find elements
        blockList           = rootElement.Q("BlockList") as ListView;
        blockElement        = rootElement.Q("BlockEditor") as BlockEditorElement;
        newBlock            = rootElement.Q("NewBlock") as Button;
        removeBlock         = rootElement.Q("RemoveBlock") as Button;
        forceSave           = rootElement.Q("ForceSave") as Button;
        changeAllAudio      = rootElement.Q("SetAllAudio") as Button;
        allAudioCategory    = rootElement.Q("SetAllAudioField") as TextField;
        generateRotations   = rootElement.Q("GenerateRotations") as Button;
        validate            = rootElement.Q("Validate") as Button;

        // Update elements
        blockList.fixedItemHeight   = 42;
        blockList.makeItem          = ListView_Makeitem;
        blockList.bindItem          = ListView_BindItem;
        blockList.selectionChanged += ListView_onSelectionChange;
        blockList.itemsSource       = blockSet.Blocks;
        blockList.Rebuild();

        newBlock.clickable.clicked          += NewBlock_Clicked;
        removeBlock.clickable.clicked       += RemoveBlock_Clicked;
        forceSave.clickable.clicked         += ForceSave_Clicked;
        changeAllAudio.clickable.clicked    += ChangeAllAudio_Clicked;
        generateRotations.clickable.clicked += GenerateRotations_Clicked;
        validate.clickable.clicked          += Validate_Clicked;

        return rootElement;
    }

    /// <summary>Validate all contained assets (just the Custombuilders for now)</summary>
    private async void Validate_Clicked()
    {
        bool running = true;
        int progressId = Progress.Start("Validate");
        Progress.RegisterCancelCallback(progressId, () => running = false);

        int max = blockSet.Blocks.Count;
        int current = 0;

        var sb = new StringBuilder(1024);

        foreach (var block in blockSet.Blocks)
        {
            if (!running) break;

            sb.Clear();
            sb.AppendLine($"Block {block.Name} has validation errors:");

            bool hasErrors = false;

            for (int i = 0; i < block.Materials.Length; ++i)
            {
                if (block.Materials[i] == null)
                {
                    sb.AppendLine($"SubMaterial {i} is null");
                    hasErrors = true;
                }
            }

            // include block.Material as first material
            int materialCount = block.Materials.Length + 1;

            if (block.Builder is CustomBuilder builder)
            {
                foreach (var usageCase in builder.usageCases)
                {
                    try
                    {
                        var bmlg = usageCase.blockMeshLodGroup;
                        if (bmlg != null)
                        {
                            bmlg.OnValidate();

                            var meshes = bmlg.LOD0.Select(l => l.mesh).ToList();
                            if (bmlg.LOD1.mesh != null) meshes.Add(bmlg.LOD1.mesh);

                            foreach (var mesh in meshes)
                            {
                                if (mesh.subMeshCount > materialCount)
                                {
                                    sb.AppendLine($"Mesh {mesh.name} has {mesh.subMeshCount} submeshes but block has {materialCount} materials");
                                    hasErrors = true;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to validate: {e.Message}");
                    }

                    Progress.Report(progressId, current, max);
                    await Task.Yield();
                }
            }

            if (hasErrors)
            {
                Debug.LogError(sb.ToString());
            }
            ++current;
        }

        Progress.Remove(progressId);
    }

    /// <summary>Generates rotations and automatically assigns them into the blockset, copying all properties of the original block</summary>
    private void GenerateRotations_Clicked()
    {
        int index = blockSet.Blocks.IndexOf(selectedBlock);
        CustomBuilder[] rotatedBuilders = CustomBuilderUtilities.GenerateRotations(selectedBlock.Builder as CustomBuilder);

        for (int i = 0; i < rotatedBuilders.Length; i++)
        {
            CustomBuilder builder = rotatedBuilders[i];
            Block block = new Block();

            block.Name                  = selectedBlock.Name + (int)builder.usageCases[0].importRotation.y;
            block.Layer                 = selectedBlock.Layer;
            block.Tier                  = selectedBlock.Tier;
            block.Category              = selectedBlock.Category;
            block.Builder               = builder;
            block.Material              = selectedBlock.Material;
            block.Materials             = selectedBlock.Materials;
            block.LODTexture            = selectedBlock.LODTexture;
            block.MinimapColor          = selectedBlock.MinimapColor;
            block.IsDiggable            = selectedBlock.IsDiggable;
            block.Solid                 = selectedBlock.Solid;
            block.BuildCollider         = selectedBlock.BuildCollider;
            block.Rendered              = selectedBlock.Rendered;
            block.ShadowCastingMode     = selectedBlock.ShadowCastingMode;
            block.WaterOccupancy        = selectedBlock.WaterOccupancy;
            block.WaterLoggable         = selectedBlock.WaterLoggable;
            block.IsWater               = selectedBlock.IsWater;
            block.IsLadder              = selectedBlock.IsLadder;
            block.IsSlope               = selectedBlock.IsSlope;
            block.BlendingPriority      = selectedBlock.BlendingPriority;
            block.PrefabHeightOffset    = selectedBlock.PrefabHeightOffset;
            block.ActualHeight          = selectedBlock.ActualHeight;
            block.AudioCategory         = selectedBlock.AudioCategory;
            block.MusicCategory         = selectedBlock.MusicCategory;
            block.EffectNames           = selectedBlock.EffectNames;
            block.Effects               = selectedBlock.Effects;

            blockSet.Blocks.Insert(index+1+i,block);
        }
        ForceSave_Clicked();
        blockList.Rebuild();
    }    

    private static int PrefixLength(string a, string b)
    {
        int i = 0;
        for (; i < a.Length && i < b.Length; i++)
        {
            if (a[i] != b[i])
                break;
        }

        return i;
    }

    private void ChangeAllAudio_Clicked()
    {
        foreach (var block in blockSet.Blocks)
            block.AudioCategory = allAudioCategory.value;

        ForceSave_Clicked();
    }

    private void NewBlock_Clicked()
    {
        Block newBlock = (Block)Activator.CreateInstance(typeof(Block));
        newBlock.Name = "NewBlock";
        blockSet.Blocks.Add(newBlock);
        blockList.Rebuild();
    }

    private void RemoveBlock_Clicked()
    {
        if (EditorUtility.DisplayDialog("Delete Block", $"Are you sure you want to delete the block {selectedBlock.Name}", "Kill it with fire","I clicked this by accident"))
        {
            blockSet.Blocks.Remove(selectedBlock);
            blockList.selectedIndex = -1;
            blockList.Rebuild();
        }        
    }

    private void ForceSave_Clicked()
    {
        EditorUtility.SetDirty(blockSet);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ListView_onSelectionChange(IEnumerable<object> objs) => this.blockElement.Block = this.selectedBlock = objs.FirstOrDefault() as Block;

    private VisualElement ListView_Makeitem()
    {
        var root = new VisualElement() { name = "ListItem" };
        root.Add(new Image() { name = "Preview" });
        root.Add(new Label() { name = "Name" });
        return root;
    }

    private void ListView_BindItem(VisualElement e, int index)
    {
        var block = blockSet.Blocks[index];

        var label = e.Q("Name") as Label;
        var preview = e.Q("Preview") as Image;

        preview.RemoveFromClassList("shader-problem");

        label.text = block.Name;
        preview.image = null;

        bool exception = false;
        bool shaderProblem = false;

        try
        {
            var shaderName = block.Material == null ? null :
                block.Material.shader == null ? null :
                block.Material.shader.name;
            if (shaderName != null && !(shaderName.StartsWith("Curved/") || shaderName.StartsWith("CalmWater")))
                shaderProblem = true;
        }
        catch
        {
            shaderProblem = true;
            exception = true;
        }

        if (block.Builder is CustomBuilder customBuilder)
        {
            if (customBuilder.usageCases.Count > 0)
            {
                if (customBuilder.usageCases[0].blockMeshLodGroup != null)
                {
                    var mesh = customBuilder.usageCases[0].blockMeshLodGroup.LOD0[0].mesh;
                    if (mesh != null)
                        preview.image = DrawRenderPreview(new Rect(0, 0, 40, 40), mesh, block.Material);
                }
                else
                {
                    var go = customBuilder.usageCases[0].mesh;
                    var mf = go.GetComponent<MeshFilter>();
                    preview.image = DrawRenderPreview(new Rect(0, 0, 40, 40), mf.sharedMesh, block.Material);
                }
            }
        }
        else if (block.Builder is WeightedPrefabBlockBuilder weightedPrefabBlockBuilder)
        {
            if (weightedPrefabBlockBuilder.prefabs.Length > 0)
            {
                var prefab = weightedPrefabBlockBuilder.prefabs[0];
                preview.image = AssetPreview.GetAssetPreview(prefab.prefab);
            }
        }

        if (shaderProblem || exception)
        {
            preview.AddToClassList("shader-problem");
        }
    }

    public Texture DrawRenderPreview(Rect size, Mesh mesh, Material material)
    {
        if (previewRenderer == null)
        {
            previewRenderer = new PreviewRenderUtility();
            previewRenderer.cameraFieldOfView = 15.0f;
            previewRenderer.ambientColor = (Color.white * 0.6f);
            previewRenderer.camera.transform.position = (Vector3.forward * 5.0f) + (Vector3.up * 5.0f) + (Vector3.right * 5.0f);
            previewRenderer.camera.transform.LookAt(Vector3.zero, Vector3.up);
            previewRenderer.camera.nearClipPlane = 0.01f;
            previewRenderer.camera.farClipPlane = 50.0f;

            previewRenderer.lights[0].enabled = true;
            previewRenderer.lights[0].type = LightType.Directional;
            previewRenderer.lights[0].color = Color.white;
            previewRenderer.lights[0].intensity = 1.5f;
            previewRenderer.lights[0].transform.rotation = Quaternion.Euler(30f, 0f, 0f);
            previewRenderer.lights[1].enabled = true;
            previewRenderer.lights[1].intensity = 0.5f;
        }

        // Create a duplicate material with NO_CURVE enabled for rendering the preview
        Material previewMaterial = Instantiate(material);
        previewMaterial.EnableKeyword("NO_CURVE");

        previewRenderer.BeginStaticPreview(size);
        previewRenderer.DrawMesh(mesh, Matrix4x4.identity, previewMaterial, 0);
        previewRenderer.Render();

        DestroyImmediate(previewMaterial);
        return previewRenderer.EndStaticPreview();
    }
}

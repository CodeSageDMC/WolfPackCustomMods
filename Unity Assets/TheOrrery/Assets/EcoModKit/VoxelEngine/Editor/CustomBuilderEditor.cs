// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(CustomBuilder))]
class CustomBuilderEditor : Editor
{
    CustomBuilder customBuilder;
    
    void OnEnable() => customBuilder = (CustomBuilder)this.target;
    
    public override void OnInspectorGUI()
    {        
        if (GUILayout.Button("Open Editor"))                CustomBuilderEditorWindow.Open(customBuilder);
        
        if (GUILayout.Button("Create Lod Groups"))          CustomBuilderUtilities.CreateLodGroups(customBuilder);

        base.OnInspectorGUI();
    }    
    
    void OnValidate()
    {
        // Check to make sure that every mesh usage case has a valid mesh data.
        foreach (var usageCase in this.customBuilder.usageCases)
            if (usageCase.mesh == null)
                Debug.LogWarning($"CustomBuilder {this.customBuilder.name} is missing a mesh", this.customBuilder);
    }
}

public class MeshUsageCaseEditorComponent : MonoBehaviour
{
    new Renderer renderer;

    bool firstSelected = false;
    public Action OnSelected;

    public OffsetCondition condition;
    public Color color;

    static void DrawString(string text, Vector3 worldPos, Color? colour = null)
    {
        Handles.BeginGUI();
        if (colour.HasValue)
            GUI.color = colour.Value;
        Vector2 screenPos = HandleUtility.WorldToGUIPoint(worldPos);
        var size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), screenPos.y - (size.y / 2), size.x, size.y), text);
        Handles.EndGUI();
    }

    void OnDrawGizmos()
    {
        Color color = Color.white;
        if (renderer == null)
            renderer = this.GetComponent<Renderer>();

        if (condition != null)
            DrawString(condition.ToString(), this.transform.position);

        bool selected = (Selection.activeGameObject == this.gameObject);

        if (selected) { renderer.sharedMaterial.color = new Color(.4f,.4f,.6f,1f); return; } // Preview the selected offset in blue and exit here

        if (!firstSelected && selected)
        {
            renderer.sharedMaterial.color = new Color(0, 1, 0, .125f);
            firstSelected = true;
            if (OnSelected != null)
                OnSelected();
        }
        else if (!selected && firstSelected == true)
        {
            firstSelected = false;
        }
        else
        { 
            color = new Color(1, 1, 1, .125f);

            if (condition != null && condition.rules.Count > 0)
            {
                var rule = condition.rules[0];
                switch (rule.ruleType)
                {
                    case BlockRule.RuleType.EqualsType:             color = new Color(0, 1, 0, .125f); break;
                    case BlockRule.RuleType.NotEqualsType:          color = new Color(1, 0, 0, .125f); break;
                    case BlockRule.RuleType.NotSolidType:           color = new Color(0, 0, 1, .125f); break;
                    case BlockRule.RuleType.IsSolidType:            color = new Color(1, 1, 1, .125f); break;
                    case BlockRule.RuleType.EqualsCategory:         color = new Color(0, 1, .5f, .125f); break;
                    case BlockRule.RuleType.NotEqualsCategory:      color = new Color(1, 0, .5f, .125f); break;
                    case BlockRule.RuleType.EqualsThisType:         color = new Color(0, 1, 0, .125f); break;
                    case BlockRule.RuleType.NotEqualsThisType:      color = new Color(1, 0, 0, .125f); break;
                    case BlockRule.RuleType.SameOrHigherPriority:   color = new Color(1, .5f, 0, .125f); break;
                    case BlockRule.RuleType.LowerPriorirty:         color = new Color(1, 0, .5f, .125f); break;
                }
            }

            renderer.sharedMaterial.color = color;
        }

        color.a = 1.0f;
        Gizmos.color = color;
        Gizmos.DrawWireCube(this.transform.position, UnityEngine.Vector3.one);
    }
}

public class CustomBuilderEditorWindow : EditorWindow 
{
    CustomBuilder target;
    MeshUsageCase selectedUsageCase;
    GUIStyle selectedUsageCaseStyle;
    GUIStyle disabledUsageCaseStyle;
    Vector2 scrollPos;
    SceneSetup[] sceneSetup;
    SceneView view;
    Color backgroundColor;
    


    static public void Open(CustomBuilder bldr)
    {
        CustomBuilderEditorWindow editor = GetWindow<CustomBuilderEditorWindow>();
        editor.SetTarget(bldr);
    }

    public void SetTarget(CustomBuilder bldr)
    {
        target = bldr; 
    }

    void OnDestroy()
    {
        ActiveEditorTracker.sharedTracker.isLocked = false;
        EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
        GameObject.DestroyImmediate(this.previewBuilder);
    }
    private void OnEnable()
    {
        ActiveEditorTracker.sharedTracker.isLocked = true;
        sceneSetup = EditorSceneManager.GetSceneManagerSetup();
        var previewScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        previewScene.name = "Custom Builder Preview";
        
        if (SceneView.sceneViews.Count == 0) view = EditorWindow.GetWindow<SceneView>();
        else view = SceneView.lastActiveSceneView;
        view.orthographic = false;
        view.sceneLighting = true;
        view.drawGizmos = true;
        view.in2DMode = false;

        backgroundColor = GUI.backgroundColor;

        // Setup style to be used when a usage case is selected
        selectedUsageCaseStyle = new GUIStyle(EditorStyles.miniButton);
        selectedUsageCaseStyle.normal.textColor = Color.green;
        selectedUsageCaseStyle.normal.background = new Texture2D(1, 1);
        selectedUsageCaseStyle.normal.background.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 1));
        selectedUsageCaseStyle.normal.background.Apply();

        // setup style to be used when a usage case is disabled
        disabledUsageCaseStyle = new GUIStyle(EditorStyles.miniButton);
        disabledUsageCaseStyle.normal.textColor = Color.black;
        disabledUsageCaseStyle.normal.background = new Texture2D(1, 1);
        disabledUsageCaseStyle.normal.background.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 1));
        disabledUsageCaseStyle.normal.background.Apply();               
    }

    void OnGUI()
    {        
        CreateRuleObjects();

        if (target != null)
        {
            EditorUtility.SetDirty(target);
            this.titleContent.text = target.name;
        }

        if (GUILayout.Button("Export builder"))
        {
            string filename = target.name + ".eco";
            string json = JsonUtility.ToJson(target, true);

            File.WriteAllText(filename, json);
        }

        if (GUILayout.Button("Import builder"))
        {
            string filename = EditorUtility.OpenFilePanel("Overwrite Builder", "", "eco");
            string json = File.ReadAllText(filename);

            JsonUtility.FromJsonOverwrite(json, target);
        }
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(10);
            ShowMeshSet();
            ShowNeighborGrids();
            ShowConditions();
            GUILayout.Space(10);
        }
        EditorGUILayout.EndHorizontal();
    }

    void OnChange()
    {
        if (target != null)
        {
            UpdateRuleObjects();            
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
        }
    }

    GameObject previewBuilder;
    GameObject previewMeshObject;
    GameObject topLayer;
    GameObject middleLayer;
    GameObject bottomLayer;

    // Updates the preview mesh, if one is available, otherwise hides the preview mesh object
    void UpdateSelectedMeshPreview()
    {
        previewBuilder.SetActive(true);
        var filter = previewMeshObject.GetComponent<MeshFilter>();
        var renderer = previewMeshObject.GetComponent<MeshRenderer>();

        if (selectedUsageCase.mesh == null)
        {
            previewBuilder.SetActive(false);
            return;
        }
        
        filter.sharedMesh = selectedUsageCase.mesh.GetComponent<MeshFilter>().sharedMesh;        

        Material tmp = Material.Instantiate(target.previewMaterial);
        tmp.EnableKeyword("NO_CURVE");
        renderer.sharedMaterial = tmp;
    }

    Dictionary<OffsetCondition.Offset, GameObject> offsetObjects;

    void CreateRuleObjects()
    {
        if (previewBuilder == null)
        {
            previewBuilder = new GameObject("PreviewMeshBuilder");

            previewMeshObject = new GameObject("NewPreviewMeshBuilder", typeof(MeshFilter), typeof(MeshRenderer));
            previewMeshObject.transform.SetParent(previewBuilder.transform);

            topLayer = new GameObject("TopLayer");
            topLayer.transform.SetParent(previewBuilder.transform);
            topLayer.transform.localPosition = UnityEngine.Vector3.zero;

            middleLayer = new GameObject("MiddleLayer");
            middleLayer.transform.SetParent(previewBuilder.transform);
            middleLayer.transform.localPosition = UnityEngine.Vector3.zero;

            bottomLayer = new GameObject("BottomLayer");
            bottomLayer.transform.SetParent(previewBuilder.transform);
            bottomLayer.transform.localPosition = UnityEngine.Vector3.zero;                                
        }

        if (offsetObjects == null)
        {
            offsetObjects = new Dictionary<OffsetCondition.Offset, GameObject>();

            for (int y = -1; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            continue;

                        GameObject offsetObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        offsetObj.name = string.Format("OFFSET {0},{1},{2}", x, y, z);
                        var usageComponent = offsetObj.AddComponent<MeshUsageCaseEditorComponent>();

                        if (y == -1)
                            offsetObj.transform.SetParent(bottomLayer.transform);
                        else if (y == 0)
                            offsetObj.transform.SetParent(middleLayer.transform);
                        else if (y == 1)
                            offsetObj.transform.SetParent(topLayer.transform);

                        offsetObj.transform.localPosition = new UnityEngine.Vector3(x, y, z);

                        var renderer = offsetObj.GetComponent<MeshRenderer>();
                        var material = AssetDatabase.LoadAssetAtPath("Assets/EcoModKit/Materials/CustomBuilderPreview.mat", typeof(Material)) as Material;
                        if (material != null)
                            renderer.sharedMaterial = new Material(material);

                        OffsetCondition.Offset condition = OffsetCondition.GetFromVector(new Vector3(x, y, z));

                        offsetObjects[condition] = offsetObj;

                        int index = (x + 1) + ((1 - z) * 3);
                        if (y == 1)
                            usageComponent.OnSelected = () => { this.ResetSelectedOffset(); this.topLayerSelection = index; this.Repaint(); };
                        if (y == 0)
                            usageComponent.OnSelected = () => { this.ResetSelectedOffset(); this.midLayerSelection = index; this.Repaint(); };
                        if (y == -1)
                            usageComponent.OnSelected = () => { this.ResetSelectedOffset(); this.bottomLayerSelection = index; this.Repaint(); };

                    }
        }

        UpdateRuleObjects();
    }

    void UpdatePreviewMesh() => previewMeshObject.transform.rotation = Quaternion.Euler(selectedUsageCase.importRotation);
   
    void UpdateRuleObjects()
    { 
        foreach (var offset in offsetObjects.Keys)
        {
            var obj = offsetObjects[offset];

            if (obj == null)
                continue;

            var usageComponent = obj.GetComponent<MeshUsageCaseEditorComponent>();

            usageComponent.condition = selectedUsageCase != null ? selectedUsageCase.conditions.Find(cond => cond.offsetType == offset) : null;
            obj.SetActive(usageComponent.condition != null);

            if (offset == selectedOffset)
            {
                Selection.activeObject = obj;
                obj.SetActive(true);
            }                       
            
        }
    }

    // show the list of meshes, allowing selection of one
    void ShowMeshSet()
    {
        EditorGUILayout.BeginVertical();
        {
            
            GUILayout.Label("Blocks", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // There should always be at least one usage case so if there are none we'll create a blank one here.
            if (target.usageCases.Count == 0)
            {
                target.usageCases.Add(new MeshUsageCase());
                EditUsageCase(target.usageCases[0]);
            }

            if (selectedUsageCase == null) EditUsageCase(target.usageCases.FirstOrDefault());
            
            int i = 0;
            int removeIndex = -1;
            int duplicateIndex = -1;
            int moveUpIndex = -1;
            int moveDownIndex = -1;
            int selectedIndex = -1;

            foreach (var usageCase in target.usageCases)
            {
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);

                if (usageCase == selectedUsageCase)
                {
                    selectedIndex = i;
                    style = selectedUsageCaseStyle;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
                    foldoutStyle.fixedWidth = 80;
                    usageCase.foldout = EditorGUILayout.Foldout(usageCase.foldout, usageCase.meshAlternates.Length + " variants", foldoutStyle);
                    GUILayout.Space(20);

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(10);

                        usageCase.enabled = GUILayout.Toggle(usageCase.enabled, "enable",GUILayout.Width(60));
                        if (!usageCase.enabled) style = disabledUsageCaseStyle;

                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("X", style, GUILayout.Width(20)))
                        {
                            removeIndex = i;
                            Undo.RecordObject(target, "Remove Usage Case");                            
                        }
                        GUI.backgroundColor = backgroundColor;

                        EditorGUI.BeginChangeCheck();
                        usageCase.mesh = EditorGUILayout.ObjectField(usageCase.mesh, typeof(GameObject), false) as GameObject;
                        if (EditorGUI.EndChangeCheck()) EditUsageCase(target.usageCases[i]);                        

                        if (GUILayout.Button("Edit", style, GUILayout.Width(50)))
                        {
                            EditUsageCase(usageCase);
                        }
                        if (GUILayout.Button("Duplicate", style, GUILayout.Width(80)))
                        {
                            duplicateIndex = i;
                            Undo.RecordObject(target, "Duplicate Usage Case");
                        }
                        if (GUILayout.Button("▲", style, GUILayout.Width(20)))
                        {
                            moveUpIndex = i;
                            EditUsageCase(usageCase);
                            Undo.RecordObject(target, "Move Usage Case");
                        }
                        if (GUILayout.Button("▼", style, GUILayout.Width(20)))
                        {
                            moveDownIndex = i;
                            EditUsageCase(usageCase);
                            Undo.RecordObject(target, "Move Usage Case");
                        }
                        GUILayout.Label("Count: " + usageCase.conditions.Count.ToString(), GUILayout.Width(60));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();

                if (usageCase.foldout)
                {
                    int numVariants = usageCase.meshAlternates.Length;
                    int newVariants = EditorGUILayout.IntField("Variants: ", numVariants);
                    if (newVariants != numVariants)
                    {
                        var old = usageCase.meshAlternates;
                        usageCase.meshAlternates = new GameObject[newVariants];
                        if (old != null)
                        {
                            for (int idx = 0; idx < Math.Min(old.Length, newVariants); idx++)
                            {
                                usageCase.meshAlternates[idx] = old[idx];
                            }
                        }
                    }

                    if (usageCase.meshAlternates != null)
                    {
                        for (int idx = 0; idx < usageCase.meshAlternates.Length; idx++)
                        {
                            usageCase.meshAlternates[idx] = EditorGUILayout.ObjectField(usageCase.meshAlternates[idx], typeof(GameObject), false) as GameObject;
                        }
                    }
                }

                i++;
                GUILayout.Space(5);
            }
            EditorGUILayout.EndScrollView();

            // Create a new usage case and clone from the one we are duplicating
            if (duplicateIndex != -1)
            {
                MeshUsageCase originalUsageCase = target.usageCases[duplicateIndex];
                MeshUsageCase copy = new MeshUsageCase();                   
                copy = originalUsageCase.Clone() as MeshUsageCase;
                copy.conditions = new List<OffsetCondition>();

                foreach (var originalCondition in originalUsageCase.conditions)
                {
                    var newCondition = new OffsetCondition(selectedOffset);
                    newCondition.offsetType = originalCondition.offsetType;
                    for (int ruleIndex = 0; ruleIndex < originalCondition.rules.Count; ruleIndex++)
                    {
                        BlockRule newRule = new BlockRule();
                        newCondition.rules.Add(newRule);
                        BlockRule oldRule = originalCondition.rules[ruleIndex];
                        newCondition.rules[ruleIndex].ruleID = oldRule.ruleID;
                        newCondition.rules[ruleIndex].ruleString = oldRule.ruleString;
                        newCondition.rules[ruleIndex].ruleType = oldRule.ruleType;

                    }
                    copy.conditions.Add(newCondition);
                }
                target.usageCases.Insert(duplicateIndex, copy);
                EditUsageCase(target.usageCases[duplicateIndex]);
                OnChange();
            }

            if (removeIndex != -1)
            {
                target.usageCases.RemoveAt(removeIndex);
                EditUsageCase(target.usageCases[removeIndex]);
                OnChange();
            }

            if (moveUpIndex != -1 && moveUpIndex > 0)
            {
                var src = target.usageCases[moveUpIndex];
                var dst = target.usageCases[moveUpIndex - 1];
                target.usageCases[moveUpIndex - 1] = src;
                target.usageCases[moveUpIndex] = dst;
                OnChange();
            }

            if (moveDownIndex != -1 && moveDownIndex < target.usageCases.Count - 1)
            {
                var src = target.usageCases[moveDownIndex];
                var dst = target.usageCases[moveDownIndex + 1];
                target.usageCases[moveDownIndex + 1] = src;
                target.usageCases[moveDownIndex] = dst;
                OnChange();
            }

            // If clicking the add new cube button, insert a new cube at the top of the list if none is selected, otherwise insert it above the selected cube
            if (GUILayout.Button("Add New Cube"))
            {
                Undo.RecordObject(target, "Add Usage Case");
                if (selectedIndex == -1) target.usageCases.Insert(0, new MeshUsageCase());
                else target.usageCases.Insert(selectedIndex, new MeshUsageCase());         
                EditUsageCase(target.usageCases[selectedIndex]);                           
                OnChange();
            }
        }
        GUILayout.Space(10);
        EditorGUILayout.EndVertical();
    }

    // Makes sure scene view is active and focused on the preview mesh
    void CenterViewOnPreviewMesh()
    {                
        view.Focus();
        view.Frame(new Bounds(Vector3.zero, Vector3.one * 3));        
    }

    // Selects the usage case, updates the preview mesh, and centers the view on the preview mesh
    void EditUsageCase(MeshUsageCase usageCase)
    {
        selectedOffset = OffsetCondition.Offset.Offset_Null;
        Selection.activeObject = null;
        selectedUsageCase = usageCase;
        UpdateSelectedMeshPreview();
        ResetSelectedOffset();
        CenterViewOnPreviewMesh();
    }

    void ResetSelectedOffset()
    {        
        topLayerSelection = -1;
        midLayerSelection = -1;
        bottomLayerSelection = -1;        
    }

    int topLayerSelection = -1;
    int midLayerSelection = -1;
    int bottomLayerSelection = -1;
    OffsetCondition.Offset selectedOffset = OffsetCondition.Offset.Offset_Null;

    // show the grid of neighbors to add conditions for, defaulting to all null (don't care)
    void ShowNeighborGrids()
    {
        if (selectedUsageCase == null)
            return;

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(400));
        {
            GUILayout.Label("Usage Conditions", EditorStyles.largeLabel);
            GUILayout.Label(selectedUsageCase.ToString(), EditorStyles.label);

            topLayer.SetActive(ShowLayer(Vector3.up, "Top", ref topLayerSelection, topLayer.activeSelf));
            middleLayer.SetActive(ShowLayer(Vector3.zero, "Middle", ref midLayerSelection, middleLayer.activeSelf));
            bottomLayer.SetActive(ShowLayer(Vector3.down, "Bottom", ref bottomLayerSelection, bottomLayer.activeSelf));

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            {
                selectedUsageCase.applyConditionsToAllRotations = EditorGUILayout.Toggle("Apply Rules To Rotations", selectedUsageCase.applyConditionsToAllRotations);
                selectedUsageCase.axis = (RotationAxis)EditorGUILayout.EnumPopup("Rotation Axis", selectedUsageCase.axis);
            }
            GUILayout.EndHorizontal();
            selectedUsageCase.dontRotateBaseMesh = EditorGUILayout.Toggle("Don't Rotate Mesh With Rules", selectedUsageCase.dontRotateBaseMesh);
            selectedUsageCase.importRotation = EditorGUILayout.Vector3Field("Import Rotation", selectedUsageCase.importRotation);
            UpdatePreviewMesh();

            GUILayout.Space(10);

            int numDecoBuilders = selectedUsageCase.decorativeBuilders != null ? selectedUsageCase.decorativeBuilders.Length : 0;
            numDecoBuilders = EditorGUILayout.IntField("Decoration Builder Count", numDecoBuilders);
            if (numDecoBuilders != selectedUsageCase.decorativeBuilders.Length)
                Array.Resize(ref selectedUsageCase.decorativeBuilders, numDecoBuilders);

            for (int decoBuilderIdx = 0; decoBuilderIdx < numDecoBuilders; decoBuilderIdx++)
                selectedUsageCase.decorativeBuilders[decoBuilderIdx] = EditorGUILayout.ObjectField("Decoration " + decoBuilderIdx, selectedUsageCase.decorativeBuilders[decoBuilderIdx], typeof(CustomBuilder), false) as CustomBuilder;
        }
        EditorGUILayout.EndVertical();
    }

    BlockRule[] copiedRules; // Stores rules that have been copied so we can paste them later
    List<OffsetCondition> conditions; // Stores the conditions for the selected usage case
    OffsetCondition selectedCondition; // Stores the selected condition for the selected usage case

    // Copies rules so they can be pasted into other conditions, used when right clicking on condition ofset buttons in the ui
    void CopyRules(OffsetCondition.Offset offsetType)
    {               
        if (selectedCondition == null || selectedCondition.rules.Count == 0 ) copiedRules = null;        
        else copiedRules = selectedCondition.rules.ToArray();                         
    }

    // Pastes copied rules from one condition to another, used when right clicking on condition ofset buttons in the ui
    void PasteRules(OffsetCondition.Offset offsetType)
    {
        Undo.RecordObject(target, "Undo Paste Rules");
        if (selectedCondition == null)
        {
            selectedCondition = new OffsetCondition(offsetType);
            conditions.Add(selectedCondition);
        }

        // Clear rules to start fresh
        selectedCondition.rules.Clear();

        // If there are no rules to paste, clear the rules and exit
        if (copiedRules == null)
        {
            ClearConditions(selectedCondition);
            return;
        }

        foreach (var rule in copiedRules)
        {
            BlockRule newRule = new BlockRule();
            newRule.ruleType = rule.ruleType;
            newRule.ruleString = rule.ruleString;
            selectedCondition.rules.Add(newRule);
        }
        OnChange();
    }

    void ClearConditions(OffsetCondition condition)
    {
        conditions.Remove(condition);
        OnChange();
    }

    void CopyLayerRules(Vector3 layerOffset)
    {
        Debug.Log($"Selected Layer is: {layerOffset}");
    }

    bool ShowLayer(Vector3 layerOffset, string name, ref int selection, bool enabled)
    {
        string[] layers = new string[9];
        Vector3[] offsets = new Vector3[9];

        int i = 0;

        for (int z = 1; z >= -1; z--)
            for (int x = -1; x <= 1; x++)
            {
                var pos = new Vector3(x, 0, z);

                Vector3 layerPos = layerOffset + pos;
                offsets[i] = layerPos;

                if (layerPos == Vector3.zero)
                    layers[i] = "[  FREE  ]\r\n[ SPACE ]";
                else
                {
                    var condition = OffsetCondition.GetFromVector(layerPos);

                    var usage = selectedUsageCase.conditions.Find(cond => cond.offsetType == condition);

                    if (usage != null)
                        layers[i] = layerPos.ToString() + "\r\n" + usage.ToString();
                    else
                        layers[i] = layerPos.ToString() + "\r\n" + "DontCare";
                }
                i++;
            }

        bool result = GUILayout.Toggle(enabled, name);

        // so dumb, selection grid steals focus so have to do this crazy button layout
        i = 0;
        GUILayout.BeginVertical();
        int newSelection = selection;
        for (int r1 = 0; r1 < 3; r1++)
        {
            GUILayout.BeginHorizontal();
            for (int r2 = 0; r2 < 3; r2++)
            {
                bool hit = selection == i;
                bool before = hit;

                // if the button is selected, make it blue
                GUI.backgroundColor = hit ? new Color(.4f, .4f, 1f, 1f) : Color.white;

                if (GUILayout.Button(layers[i], "Button", GUILayout.Width(120)))
                {
                    GUI.FocusControl(null); // Reset focus so that text fields can be updated on button click
                    
                    // Create right click menu to allow copy and pasting rulesets between conditions
                    if (Event.current.button == 1)
                    {
                        GenericMenu menu = new GenericMenu();

                        OffsetCondition.Offset currentOffset = OffsetCondition.GetFromVector(offsets[i]);
                        conditions = selectedUsageCase.conditions;
                        selectedCondition = conditions.FirstOrDefault(condition => condition.offsetType == currentOffset);

                        menu.AddItem(new GUIContent("Copy Rules"),  false, () => CopyRules(currentOffset));
                        menu.AddItem(new GUIContent("Paste Rules"), false, () => PasteRules(currentOffset));
                        menu.AddItem(new GUIContent("Clear Rules"), false, () => ClearConditions(selectedCondition));
                        menu.ShowAsContext();                        
                    }
                    hit = true;
                }
                GUI.backgroundColor = backgroundColor;

                if (before != hit)
                    newSelection = i;
                i++;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        
        if (newSelection != selection)
        {
            ResetSelectedOffset();
            selectedOffset = OffsetCondition.GetFromVector(offsets[newSelection]);

            selection = newSelection;
        }

        return result;
    }

    // when selecting a location, show the list of conditions for that
    void ShowConditions()
    {

        EditorGUILayout.BeginVertical(GUILayout.Width(400));
        {            
            GUILayout.Label("Conditions for " + selectedOffset, EditorStyles.largeLabel);

            var condition = selectedUsageCase.conditions.Find(x => x.offsetType == selectedOffset);

            if (condition != null)
            {
                if (condition.rules == null)
                    condition.rules = new List<BlockRule>();

                int i = 0;
                int removeIndex = -1;
                foreach (var rule in condition.rules)
                {
                    EditorGUILayout.BeginHorizontal();
                    {                        
                        if (GUILayout.Button("Remove"))
                            removeIndex = i;
                        if (removeIndex == -1) Undo.RecordObject(target, "Undo Rule Change");
                        rule.ruleType = (BlockRule.RuleType)EditorGUILayout.EnumPopup(rule.ruleType);
                        rule.ruleString = EditorGUILayout.TextField(rule.ruleString);
                    }
                    EditorGUILayout.EndHorizontal();
                    i++;
                }

                if (removeIndex != -1)
                {
                    Undo.RecordObject(target, "Undo Remove Rule");
                    condition.rules.RemoveAt(removeIndex);

                    if (condition.rules.Count == 0)
                        selectedUsageCase.conditions.Remove(condition);

                    OnChange();
                }
            }
            
            if (selectedOffset != OffsetCondition.Offset.Offset_Null && GUILayout.Button("Add New Rule"))
            {
                if (condition == null)
                {
                    OffsetCondition newCondition = new OffsetCondition(selectedOffset);
                    selectedUsageCase.conditions.Add(newCondition);

                    condition = newCondition;
                }

                condition.rules.Add(new BlockRule());

                OnChange();
            }
        }
        EditorGUILayout.EndVertical();
    }
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoModKitEditor.Occupancy.Internal
{
#if UNITY_EDITOR

    using System.Collections.Generic;
    using System.Linq;
    using Eco.Shared.Utils;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using EcoModKit.Occupancy;
    using Color = UnityEngine.Color;

    /// <summary> Custom editor for the CustomPositionsRequirement class, it helps to create the position requirements visually by allowing to manipulate block positions using an axis handle, 
    /// rather than of having them being defined one by one</summary>
    [CustomEditor(typeof(CustomPositionsRequirement))]
    public class CustomPositionsRequirementEditor : Editor
    {
        CustomPositionsRequirement customPositionsReq; //custom positions requirement instance
        SerializedProperty         srProperty;         //positions requirements serialized property
        ReorderableList            srReorderableList;  //main reorderable list that contains positions requirements

        //Reorderable list to create for each positions requirement in CustomPositionsRequirement instance
        Dictionary<string, ReorderableList> positionReqs = new Dictionary<string, ReorderableList>();

        GUIStyle labelStyle;
        GUIStyle textStyle;

        void OnEnable()
        {
            //Set main properties
            customPositionsReq  = (CustomPositionsRequirement)target;
            srProperty        = serializedObject.FindProperty("positionsRequirements");
            srReorderableList = new ReorderableList(serializedObject, srProperty, false, true, true, true);

            //Set axis handle label style and style with rich text
            labelStyle = new GUIStyle() { fontSize = 12, fontStyle = FontStyle.Bold };
            labelStyle.normal.textColor = Color.red;
            textStyle = new GUIStyle();
            textStyle.richText = true;
        
            //Set reorderable list draw callbacks
            srReorderableList.drawHeaderCallback    = DrawHeader;
            srReorderableList.drawElementCallback   = DrawElements;
            srReorderableList.elementHeightCallback = GetHeight;
        }

        public override void OnInspectorGUI()
        {
            //Make sure customPositionsReq isn't null
            if (customPositionsReq == null) customPositionsReq = (CustomPositionsRequirement)target;

            //Show reorderable list and apply properties to see if there any unsaved changes
            serializedObject.Update();
            srReorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        
            //Draw Remove duplicates button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove duplicates")) RemoveDuplicates();
            if (GUILayout.Button("Generate code and copy to clipboard")) BuildOccupancyUtils.CopyToClipboard(BuildOccupancyUtils.GetPositionsRequirementCode(customPositionsReq));
            GUILayout.EndHorizontal();
        }

        void OnSceneGUI()
        {
            //Make sure customPositionsReq isn't null and that positions requirements exists
            if (customPositionsReq == null) customPositionsReq = (CustomPositionsRequirement)target;
            if (customPositionsReq.positionsRequirements == null) return;

            //Get all selected positions and set pivot from where to place the axis handle, return if none found
            var allSelected = customPositionsReq.positionsRequirements.SelectMany(sr => sr.positions).Where(p => p.select).ToList();
            var pivot = allSelected.FirstOrDefault();
            if (pivot == null) return;

            // Draw position axis handle and text describing it
            Handles.Label(pivot.value, "Positions Requirement \n selected positions", labelStyle);
            var newHandlePos = Handles.PositionHandle(pivot.value, Quaternion.identity);
            
            //Round new position  to integers and calculate delta(how much did it move)
            var newPosInt = new Vector3Int(Mathf.RoundToInt(newHandlePos.x), Mathf.RoundToInt(newHandlePos.y), Mathf.RoundToInt(newHandlePos.z)); ;
            var delta     = newPosInt - pivot.value;

            //Apply deltas to all selected positions
            foreach (var item in allSelected) item.value += delta;
        }

        //Draws every positions requirement, this means its list of positions, partName and placement message
        void DrawElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            //get current positions requirements instance being drawn, and its positions property
            var element   = srReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var positions = element.FindPropertyRelative("positions");

            //List to be drawn that will contain the list of positions from the current positions requirement
            ReorderableList innerReorderableList;

            //Try to get current reorderable list, if it doesn't exist create it
            if (!positionReqs.TryGetValue(positions.propertyPath, out innerReorderableList))
            {
                innerReorderableList = new ReorderableList(positions.serializedObject, positions, true, true, true, true);
                positionReqs.Add(positions.propertyPath, innerReorderableList);
            }

            //Set reorderable list draw callbacks
            innerReorderableList.drawElementCallback = (Rect innerRect, int innerIndex, bool isActive, bool isFocused) =>
            {
                var pos = positions.GetArrayElementAtIndex(innerIndex); //Position parameter to draw

                //Get Selectable<Vector3> serialized properties
                var selectable = pos.FindPropertyRelative("select"); 
                var value      = pos.FindPropertyRelative("value");

                //Draw select box and Vector3Int fields
                EditorGUI.PropertyField(new Rect(innerRect.x, innerRect.y, innerRect.width * 0.15f, EditorGUIUtility.singleLineHeight), selectable, GUIContent.none);
                EditorGUI.PropertyField(new Rect(innerRect.x + innerRect.width * 0.2f, innerRect.y, innerRect.width * 0.75f, EditorGUIUtility.singleLineHeight), value, GUIContent.none);
            };

            innerReorderableList.drawHeaderCallback  = (Rect rect) =>
            {
                //Draw column headers for select and positions fields
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Select");
                EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.2f, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Positions Requirement Position");

                var btnWidth    = rect.width * 0.25f;  //width for buttons and also used as an offset
                var btnPosition = rect.x + rect.width; //base position from where to place the buttons

                //Draw positions requirement helper buttons 
                if (GUI.Button(new Rect(btnPosition - btnWidth * 4, rect.y - EditorGUIUtility.singleLineHeight * 2.15f, btnWidth, EditorGUIUtility.singleLineHeight), "Select all"))         { SetAllSelectedTo (customPositionsReq.positionsRequirements[index], true); }
                if (GUI.Button(new Rect(btnPosition - btnWidth * 4, rect.y - EditorGUIUtility.singleLineHeight * 1.15f, btnWidth, EditorGUIUtility.singleLineHeight), "Deselect all"))       { SetAllSelectedTo (customPositionsReq.positionsRequirements[index], false); }
                if (GUI.Button(new Rect(btnPosition - btnWidth * 3, rect.y - EditorGUIUtility.singleLineHeight * 2.15f, btnWidth, EditorGUIUtility.singleLineHeight), "Duplicate selected")) { DuplicateSelected(customPositionsReq.positionsRequirements[index]); }
                if (GUI.Button(new Rect(btnPosition - btnWidth * 3, rect.y - EditorGUIUtility.singleLineHeight * 1.15f, btnWidth, EditorGUIUtility.singleLineHeight), "Remove selected"))    { RemoveSelected   (customPositionsReq.positionsRequirements[index]); }
            };

            //Draw partName, placement msg label and it's requirement type in different rows
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + rect.height - EditorGUIUtility.singleLineHeight * 6.5f, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("partName"));
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + rect.height - EditorGUIUtility.singleLineHeight * 5.5f, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("placementRequirement"));
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + rect.height - EditorGUIUtility.singleLineHeight * 4.5f, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("requirementType"));

            //Show example of how the message would look from the server side
            var partName     = element.FindPropertyRelative("partName").stringValue;
            var placementReq = element.FindPropertyRelative("placementRequirement").stringValue;
            EditorGUI.LabelField(new Rect(rect.x, rect.y + rect.height - EditorGUIUtility.singleLineHeight * 2.5f, rect.width, EditorGUIUtility.singleLineHeight), $"<color=silver><b>WorldObject status message: </b></color> <color=blue><b>{(partName.IsSet() ? partName.Capitalize() : "")}</b></color> <color=silver>is placed</color> <color=orange><b>{placementReq}.</b></color>", textStyle);
            EditorGUI.LabelField(new Rect(rect.x, rect.y + rect.height - EditorGUIUtility.singleLineHeight * 1.5f, rect.width, EditorGUIUtility.singleLineHeight), $"<color=silver><b>Trying to place message: </b></color> <color=blue><b>{(partName.IsSet() ? partName.Capitalize() : "")}</b></color> <color=silver>must be placed</color> <color=orange><b>{placementReq}.</b></color>", textStyle);

            //Draw the inner reorderable list(positions)
            innerReorderableList.DoList(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2.25f, rect.width, rect.height * 0.8f));

        }

        //Calculates the desirable height for each positions requirement element in editor, given how many elements it possesses
        float GetHeight(int index)
        {
            //from the current element being drawn, get its positions property and try get its reorderable list if set
            var element   = srReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var positions = element.FindPropertyRelative("positions");
            var hasValue  = positionReqs.TryGetValue(positions.propertyPath, out var innerReorderableList);

            //try get desirable height but if positions req happens to not be found use alternative way of calculating height
            var height = hasValue ? innerReorderableList.GetHeight() + (9 * EditorGUIUtility.singleLineHeight) : (positions.arraySize + 7) * EditorGUIUtility.singleLineHeight * 1.05f;
            return height;
        }

        //Draws the header
        void DrawHeader(Rect rect) => EditorGUI.LabelField(rect, "Positions Requirements");

        //Creates new instances from the selected ones, they are deselected by default
        void DuplicateSelected(PositionsRequirement positionsRequirement)
        {
            if (customPositionsReq == null) return;

            var allSelected = positionsRequirement.positions.Where(p => p.select).ToList();
            foreach (var pos in allSelected) positionsRequirement.positions.Add(new Selectable<Vector3Int> { value = pos.value, select = false });
        }

        //Removes from the positions requirement all selected positions
        void RemoveSelected(PositionsRequirement positionsRequirement) => positionsRequirement.positions.RemoveAll(p => p.select);

        //Set all of selectable positions to a certain value, useful in cases where we want to either select or deselect them all
        void SetAllSelectedTo(PositionsRequirement positionsRequirement, bool value) { foreach (var pos in positionsRequirement.positions) pos.select = value; }

        //Removes duplicates on each positions requirement
        void RemoveDuplicates() 
        {
            foreach (var sr in customPositionsReq.positionsRequirements) 
                sr.positions = sr.positions.DistinctBy(a => a.value).ToList();
        }
    }
#endif
}

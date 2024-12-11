// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEditor;
using EcoModKit.Interactions.Buttons;

namespace EcoModKitEditor.Interactions
{
    /// Simple editor for ButtonStateController to warn if the interactable property hasn't been set, given that it's required.
    [CustomEditor(typeof(ButtonStateController))]
    public class ButtonStateControllerEditor : Editor
    {
        ButtonStateController buttonStateController;

        void OnEnable() => this.buttonStateController = this.target as ButtonStateController;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //  Message warning if they interactable component hasn't been set, given that it is a required
            if (this.buttonStateController.interactable == null) EditorGUILayout.HelpBox("Interactable needs to be set", MessageType.Error);
        }
    }
}

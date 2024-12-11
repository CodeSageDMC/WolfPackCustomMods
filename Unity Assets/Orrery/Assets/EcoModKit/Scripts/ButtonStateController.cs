// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

namespace EcoModKit.Interactions.Buttons
{
    /// <summary> Class that handles logic for enabled/disabled states in buttons, it makes sure to set the interactable collider to the correct layer so as to prevent undesirable behaviour when pressing down 
    /// interactable key during multiple frames. It also handles the visual states by enabling/disabling the correspondent meshes(if they are set).</summary>
    public class ButtonStateController : TrackableBehavior
    {
        [SerializeField, Tooltip("Mesh to show when setting the button status to true, meaning the button can be pressed(optional).")] 
        MeshRenderer buttonUnpressed;
        [SerializeField, Tooltip("Mesh to show when setting the button status to false, meaning the button is already pressed(optional).")]
        MeshRenderer buttonPressed;
        [Tooltip("Interactable component that is attached to the collider that the interaction system uses to detect button presses (required).")]
        public SpecificInteractable interactable;

        int defaultLayer;        //unpressed layer - allows interactions
        int blockSelectionLayer; //pressed layers - blocks interactions

        void Awake()
        {
            this.defaultLayer = LayerMask.NameToLayer("Default");
            this.blockSelectionLayer = LayerMask.NameToLayer("BlockSelection");
        }
    
        /// <summary> Allows/Blocks button interaction, and enables/disables the meshes accordingly if they happen to be set.</summary>
        public void SetButtonStatus(bool status)
        {
            if (this.buttonUnpressed != null) this.buttonUnpressed.enabled = status;
            if (this.buttonPressed != null)   this.buttonPressed.enabled   = !status;
            this.interactable.gameObject.layer = status ? defaultLayer : blockSelectionLayer; //Set appropiate layers
        }
    }
}

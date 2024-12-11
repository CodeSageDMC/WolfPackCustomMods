// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace EcoModKit.Occupancy
{
    /// <summary> Used for storing and creating positions requirements, it's gizmos shows all blocks on the different positions requirements, as well as the selected ones. </summary>
    public class CustomPositionsRequirement : MonoBehaviour
    {
    #if UNITY_EDITOR
        public List<PositionsRequirement> positionsRequirements;

        //Color to use so that the gizmos are visible and able to be diferentiated from each other
        Color[] defaultColors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black };
        
        private void OnDrawGizmosSelected()
        {
            //Early return if there nothing to draw
            if (positionsRequirements == null) return;

            //Draw block position for every defined positions requirement
            for (int i = 0; i < positionsRequirements.Count; i++)
            {
                //Set color of gizmos
                Color gizmosColor = defaultColors[i % defaultColors.Length];
                gizmosColor.a     = 0.75f;
                Gizmos.color      = gizmosColor;

                //Draw gizmos block positions and if selected draw another block inside to make it visible
                foreach (var pos in positionsRequirements[i].positions)
                {
                    if (pos.select) Gizmos.DrawCube(transform.position + pos.value, Vector3.one * 0.9f);
                    Gizmos.DrawWireCube(transform.position + pos.value, Vector3.one);
                }
            }
        }
    #endif
    }

    /// <summary> Contains the properties needed for defining a positions requirement in the server side, it contains a list of positions, some predefined requirements and 
    /// a message in case the requirement fails. E.g, The IronShipyard has a water positions requirement on certain positions</summary>
    [Serializable]
    public class PositionsRequirement
    {
        public List<Selectable<Vector3Int>> positions;
        [Tooltip("The part of the object which this requirement refers to. e.g shaft, base etc.")]
        public string partName;
        [Tooltip("What's the placement requirement for that position e.g on solid ground, on empty space, in water."), FormerlySerializedAs("placementMsg")]
        public string placementRequirement;
        [Tooltip("Some predefined position requirements, optionally can be set to custom e.g have a position be placed on solid or empty space")]
        public PositionsRequirementType requirementType;
    }

    /// <summary> Currently defined positions requirements for solid, water and empty blocks, optionally can be set to custom.</summary>
    public enum PositionsRequirementType
    {
        Solid,
        Water,
        Empty,
        Custom = -1
    }

    /// <summary> Wraps a certain type value and adds the select bool property, useful for filtering and applying modifications on selected values only </summary>
    [Serializable]
    public class Selectable<T>
    {
        public T value;
        public bool select;
    }
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

namespace Eco.Client.Utils
{
    public enum MeshPartsUpdaterMode
    {
        None,
        
        // Those two used as multiple active parts at a time, visibility updates just by increasing/decreasing amount of active parts;
        IncreasedOneByOne,
        DecreasedOneByOne,
        
        // Those two used as only one part can be active at a time, visibility updates just by indexing, current is active, other disabled
        NoneToFull, 
        FullToNone
    }
    
    /// <summary>
    /// Helper to manipulate mesh parts based on index you need to show, or percentage
    /// todo: Replace food built-in bites with this system to reuse
    /// </summary>
    public class MeshPartsUpdater : MonoBehaviour
    {
        [Tooltip("List of parts. Visibility will be evaluated based on mode")]
        public GameObject[] Parts;
        
        [Tooltip("Sequence type that will be applied to parts. Needs to be assigned based on what behavior is needed.")]
        public MeshPartsUpdaterMode Mode;
        
        [Tooltip("Element that will be evaluated by default on start. Set to -1 to disable")]
        public int StartingIndex = 0;
        
        // Current index cache
        int currentIndex = 0;

        void Awake()
        {
            if (this.StartingIndex >= 0) this.UpdateVisible(this.StartingIndex, this.Mode);
        }

        public void DecreaseIndexAnimation(int index)
        {
            if (index >= this.currentIndex) return;
            this.UpdateVisible(index, this.Mode);
        }

        public void UpdateByIndex(int index) => this.UpdateVisible(index, this.Mode);
        public void ResetVisible() => this.UpdateVisible(0, this.Mode);

        /// <summary> Evaluate current index to show by passing percentage in range 0-1 </summary>
        public void UpdateByPercent(float value, MeshPartsUpdaterMode mode = MeshPartsUpdaterMode.None)
        {
            var index = MathUtils.PercentageToRangeInt(Mathf.Clamp01(value), 0, this.Parts.Length);
            this.UpdateVisible(index, mode);
        }

        void UpdateVisible(int index, MeshPartsUpdaterMode mode)
        {
            if (mode == MeshPartsUpdaterMode.None) mode = this.Mode;
            if (index > this.Parts.Length) index = this.Parts.Length; // clamp index
            
            switch (mode)
            {
                case MeshPartsUpdaterMode.NoneToFull:        this.SequenceNoneToFull(index); break;
                case MeshPartsUpdaterMode.FullToNone:        this.SequenceFullToNone(index); break;
                case MeshPartsUpdaterMode.IncreasedOneByOne: this.SequenceIncreasedOneByOne(index); break;
                case MeshPartsUpdaterMode.DecreasedOneByOne: this.SequenceDecreasedOneByOne(index); break;
            }

            this.currentIndex = index;
        }

        void SequenceIncreasedOneByOne(int index)
        {
            // 3 part example. Index range = 0 - 3
            // i=0 - No model at all
            // i=1 - 1/3  model (first only active part)
            // i=2 - 2/3  model (Added second active part)
            // i=3 - Full model (Added third active part)
            
            for (var i = 0; i < this.Parts.Length; i++) this.Parts[i].SetActive(i < index);
        }
        
        void SequenceDecreasedOneByOne(int index)
        {
            // 3 part example. Index range = 0 - 3
            // i=0 - Full model (All part active)
            // i=1 - 2/3  model (First part deactivated, other active)
            // i=2 - 1/3  model (First and Second part deactivated, last active)
            // i=3 - No model at all (all parts deactivated)
            
            for (var i = 0; i < this.Parts.Length; i++) this.Parts[i].SetActive(i >= index);
        }

        void SequenceNoneToFull(int index)
        {
            // 3 part example. Index range = 0 - 3
            // i=0 - No model at all
            // i=1 - 1/3  model (first active part only)
            // i=2 - 2/3  model (second active part only)
            // i=3 - Full model (third active part only)
            
            if (index == 0)
            {
                foreach (var part in this.Parts) part.SetActive(false);
                return;
            }

            index -= 1; // offset index

            for (var i = 0; i < this.Parts.Length; i++) this.Parts[i].SetActive(index == i);
        }

        void SequenceFullToNone(int index)
        {
            // 3 part example. Index range = 0 - 3
            // i=0 - Full model (first active part only)
            // i=1 - 2/3  model (second active part only)
            // i=2 - 1/3  model (third active part only)
            // i=3 - No model at all
            
            if (index >= this.Parts.Length)
            {
                foreach (var part in this.Parts) part.SetActive(false);
                return;
            }
            
            for (var i = 0; i < this.Parts.Length; i++) this.Parts[i].SetActive(index == i);
        }
    }
}

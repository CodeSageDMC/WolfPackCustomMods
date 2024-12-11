// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Blocks
{
    public class BlockEditorModel : ScriptableObject
    {
        public List<Block> Blocks;
        public List<BlockSet> BlockSets;
    }
}
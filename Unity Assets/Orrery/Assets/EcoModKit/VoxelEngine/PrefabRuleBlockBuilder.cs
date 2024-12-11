// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

#nullable enable

/// <summary>
/// PrefabRuleBlockBuilder uses rules similar to CustomBuilder to return a prefab.
/// </summary>
[Serializable]
public partial class PrefabRuleBlockBuilder : PrefabBlockBuilder
{
    public List<PrefabUsageCase> usageCases = new List<PrefabUsageCase>();
}
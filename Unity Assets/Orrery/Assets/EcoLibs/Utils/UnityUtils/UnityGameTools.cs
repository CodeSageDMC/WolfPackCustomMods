// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEngine;

public static class UnityGameTools
{
    public static void SortChildrenByName(GameObject[] gameObjects)
	{
		foreach (GameObject obj in gameObjects)
			SortChildrenByName(obj);
	}

	public static void SortChildrenByName(GameObject obj)
	{
		List<Transform> children = new List<Transform>();
		for (int i = obj.transform.childCount - 1; i >= 0; i--)
		{
			Transform child = obj.transform.GetChild(i);
			children.Add(child);
			child.parent = null;
		}
		children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
		foreach (Transform child in children)
			child.parent = obj.transform;
	}
}

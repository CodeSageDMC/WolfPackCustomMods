// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Eco.Client.Utils;
using Eco.Shared.Utils;
using UnityEngine;

public static class UnityUtilsParentage
{
	public static IEnumerable<Transform> ActiveChildren(this Transform obj) => obj.Children().Where(x => x.gameObject.activeInHierarchy);

	// Transform wont let you run linq on it, this helps.
	public static TransformChildren Children(this Transform obj) => new(obj);

    /// <summary>Returns whole hierarchy of <paramref name="transform"/>.</summary>
    public static TransformHierarchyEnumerable GetHierarchy(this Transform transform) => new(transform.transform);

	public static IEnumerable<Transform> ChildrenRecursive(this Transform obj, bool includeSelf = false, Func<Transform, bool> ShouldTraverseChildren = null)
	{
		var children = obj.Children();
		var furtherDescendents = obj.Children().Where(x => ShouldTraverseChildren?.Invoke(x) ?? true).SelectMany(c => ChildrenRecursive(c));

		var all = children.Concat(furtherDescendents);
		if (includeSelf)
			all = children.Concat(obj.SingleItemAsEnumerable());

		return children;
	}

	public static void SetChildrenOrder(this Transform obj, IEnumerable<Transform> order)
	{
		foreach (Transform child in order)
			child.SetAsLastSibling();
	}

	public static Transform FindChildRecursive(this Transform obj, string name)
	{
        for (var i = 0; i < obj.childCount; i++)
        {
            var child = obj.GetChild(i);
            if (child.name == name)
                return child;
            if (child.childCount > 0)
            {
                var matchingSubChild = child.FindChildRecursive(name);
                if (matchingSubChild != null)
                    return matchingSubChild;
            }
        }
        return null;
    }

    // Return the first parent up the chain that matches the predicate
    public static Transform FindFirstParent(this Transform obj, Func<Transform, bool> predicate, bool includeSelf = true)
    {
        var scannedObj = includeSelf ? obj : obj.parent;
        while (scannedObj != null)
        {
            if (predicate(scannedObj)) return scannedObj;
            scannedObj = scannedObj.parent;
        }
        return null;
    }

    // Return the first parent up the chain that has the given component
    public static T FindFirstParent<T>(this Transform obj) where T : class
    {
        var p = obj.parent;
        while (p != null)
        {
            if (p.GetComponent<T>() != null) return p.GetComponent<T>();
            else p = p.parent;
        }
        return null;
    }
	public static void RemoveChild(this Transform t, int i)
	{
		var child = t.GetChild(i);
		child.SetParent(null);
		UnityEngine.Object.Destroy(child.gameObject);
	}

	public static void RemoveChildrenWhere(this Transform t, Func<Transform, bool> test)
	{
		for (var i = t.childCount - 1; i >= 0; --i)
		{
			var child = t.GetChild(i);
			if (test(child))
			{
				child.SetParent(null);
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
	}

	public static void RemoveChildrenWhere<Comp>(this Transform t, Func<Comp, bool> test)
			where Comp : TrackableBehavior
    {
		for (var i = t.childCount - 1; i >= 0; --i)
		{
			var child = t.GetChild(i);
			var comp = child.GetComponent<Comp>();
			if (comp != null && test(comp))
			{
				child.SetParent(null);
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
	}

	public static void RemoveChildrenWith<Comp>(this Transform t) where Comp : TrackableBehavior
	{
		t.RemoveChildrenWhere<Comp>(Predicates.AlwaysMatching);
	}

	public static void Reparent(this Transform obj, Transform newparent)
	{
		obj.SetParent(newparent);
		obj.ZeroTransform();
	}

	public static void SortChildren<TKey>(this Transform obj, Func<Transform, TKey> predicate)
	{
		foreach (Transform child in obj.Children().OrderBy(predicate))
			child.SetAsLastSibling();
	}

	public static void SortComparable(this Transform obj)
	{
		var sortable = obj.GetComponentsInChildren<IComparable>().ToList();
		sortable.Sort();
		sortable.ForEach(x => { ((MonoBehaviour)x).transform.SetAsLastSibling(); });
	}

	public static IEnumerable<TKey> SortOrFlipSort<TKey, TValue>(IEnumerable<TKey> list, Func<TKey, TValue> predicate) where TValue : IComparable
	{
		if (list.IsOrdered(predicate)) return list.OrderByDescending(predicate);
		else return list.OrderBy(predicate);
	}

	// Sort the list, unless it's already sorted, then flip sort it.
	public static void SortOrFlipSortChildren<TKey>(this Transform obj, Func<Transform, TKey> sortPredicate, Func<Transform, IComparable> thenPredicate = null)
	{
		// split active children by predicate into 2 groups (pass or fail)
		var list = obj.ActiveChildren();

		// order list by predicate and/or then predicate
		// and flips order, if sequence still the same after sort
		var sortedlist = thenPredicate != null ? list.OrderBy(sortPredicate).ThenBy(thenPredicate) : list.OrderBy(sortPredicate);
		if (sortedlist.SequenceEqual(list))
			sortedlist = thenPredicate != null ? list.OrderByDescending(sortPredicate).ThenByDescending(thenPredicate) : list.OrderByDescending(sortPredicate);

		// reorder children transforms
		sortedlist.ForEach(x => { x.SetAsLastSibling(); });
	}

	public static string Parentage(this MonoBehaviour b) => Parentage(b.transform);
	public static string Parentage(this GameObject g) => Parentage(g.transform);
	public static string Parentage(this Transform t)
	{
		var list = new List<string>();
		while (t != null)
		{
			list.Add(t.name);
			t = t.parent;
		}
		list.Reverse();
		return String.Join("/", list);
	}

	/// <summary>Return true if any of the immediatge children are 'activeSelf'</summary>
	public static bool HasActiveChildren(this Transform t)
	{
		foreach (Transform entry in t)
			if (entry.gameObject.activeSelf)
				return true;
		return false;
	}

	/// <summary>Hide all the children in the current transform</summary>
	public static void HideAllChildren(this Transform t)
	{
		foreach (Transform entry in t)
			entry.gameObject.SetActive(false);
	}

	/// <summary>Return the number of children that are 'activeSelf' in this transform.</summary>
	public static int ActiveChildrenCount(this Transform t)
	{
		var i = 0;
		foreach (Transform entry in t)
			if (entry.gameObject.activeSelf)
				i++;
		return i;
	}


	/// <summary>
	/// Tries to get <paramref name="transform"/>'s ancestor in specific <paramref name="generation"/>.
	/// In example:
	/// <ul>
	/// <li>for <paramref name="generation"/> == 0 it is <c>transform</c> itself.</li>
	/// <li>for <paramref name="generation"/> == 1 it is <c>transform.parent</c>.</li>
	/// <li>for <paramref name="generation"/> == 2 it is <c>transform.parent.parent</c>.</li>
	/// </ul>
	/// etc.
	/// </summary>
	public static bool TryGetAncestor(this Transform transform, int generation, out Transform ancestor)
    {
        if (generation < 0)
        {
            ancestor = null;
            return false;
        }

        ancestor = transform;
        for (var i = generation - 1; i >= 0; --i)
        {
            ancestor = ancestor.parent;
            if (ancestor == null)
                return false;
        }

        return true;
    }
    
    public static GameObject FindChildFromRoot(this GameObject parent, GameObject child)	// Traverse GameObject looking for child by INDEX
    {
        if (parent && child)
        {
            var indexes = child.GetPathToChild().Select(t => t.GetSiblingIndex()).Reverse().ToList();
            if (indexes.Count > 0) indexes.RemoveAt(0);
            return parent.GetChildAtPath(indexes.ToArray());
        }
        return null;
    }

	// Traverses all parents to build hierarchy path until root, includes child 
	// e.g.:
	//	A(0) <- root
	//		A.1(0)
	//		A.2(1)
	//			A.2.1(0) <- child
	// path is {0, 1, 0}
	//		 child -> root
	public static List<Transform> GetPathToChild(this GameObject child)	
    {
        var currentParent = child.transform;
        var parents = new List<Transform>() { child.transform };

        while (currentParent.parent != null)
        {
            currentParent = currentParent.parent;
            parents.Add(currentParent);
        }

        return parents;
    }

    //Find child by index from root to actual object
    // e.g.:
    //	A(0)
    //	B(1) <- root
    //		B.1(0)
    //		B.2(1)
    //			B.2.1(0) <- wanted gameobject
    //	C(2)
    //	
    //	path would be {1, 1, 0} from root to wanted gameobject
    //
    public static GameObject GetChildAtPath(this GameObject parent, int[] indexesPath) 
    {
        Transform child = parent.transform;
        foreach (var i in indexesPath) child = child.GetChild(i);
        return child.gameObject;
    }
}

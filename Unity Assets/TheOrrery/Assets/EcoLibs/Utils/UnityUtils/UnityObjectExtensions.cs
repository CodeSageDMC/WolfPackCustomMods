// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public static class UnityObjectExtensions
{
    public static string GetScenePath(this Component b) => GetScenePath(b.transform);
    public static string GetScenePath(this GameObject g) => GetScenePath(g.transform);

    public static string GetScenePath(this Transform t) => AppendScenePath(new StringBuilder(), t).ToString();
    public static StringBuilder AppendScenePath(StringBuilder sb, Transform t)
    {
        var parent = t.parent;
        if (parent != null)
            AppendScenePath(sb, parent).Append('/');
        sb.Append(t.name);
        return sb;
    }

    /// <summary> Gets or adds a component of type <typeparamref name="T"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component => gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();

    /// <summary> Gets or adds a component of the provided type. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Component GetOrAddComponent(this GameObject gameObject, Type componentType) => gameObject.TryGetComponent(componentType, out var component) ? component : gameObject.AddComponent(componentType);

    /// <summary> Checks if <paramref name="gameObject"/> has a component of type <typeparamref name="T"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasComponent<T>(this GameObject gameObject) => gameObject.TryGetComponent<T>(out _);

    /// <summary> Checks if <paramref name="component"/> has a component of type <typeparamref name="T"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasComponent<T>(this Component component) => component.gameObject.TryGetComponent<T>(out _);


    ///<summary>Gets a component of type T from the children of the game object when the component is null. GetComponentInChildren may be an expensive operation
    ///so this is mean to be used on the OnValidate method to reduce the missing references errors on the prefabs</summary>
    public static void GetAndVerifyExistsComponent<T>(this GameObject gameObject, ref T component) where T : Component
    {
        if (component == null) 
        {
            component = gameObject.GetComponentInChildren<T>(true);
            if (component == null) Debug.LogError($"Could not find component of type {typeof(T)} among the children of {gameObject.name}");
        }
    }

    ///<summary>Gets a component of type T from the children of the game object when the component is null (search based on name). GetComponentInChildren may be an expensive operation
    ///so this is mean to be used on the OnValidate method to reduce the missing references errors on the prefabs</summary>
    public static void GetAndVerifyExistsComponent<T>(this GameObject gameObject, ref T component, string name) where T : Component 
    {
        if (component == null) 
        {
            component = gameObject.GetComponentsInChildren<T>(true).Where(x => x.name.Equals(name)).FirstOrDefault();
            if (component == null) Debug.LogError($"Could not find an object called {name} that contains the component of type {typeof(T)} among the children of {gameObject.name}");
            else Debug.Log($"Component of type {typeof(T)} found");
        }
    }
}

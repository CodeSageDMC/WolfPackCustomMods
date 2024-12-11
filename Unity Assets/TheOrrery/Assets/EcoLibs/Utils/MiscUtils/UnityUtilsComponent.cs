// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Eco.Shared.Utils;
using UnityEngine;

public static class UnityUtilsComponent
{
    private static class ComponentLists<T> { public static readonly List<T> List = new List<T>(); }
    public delegate void ItemWithValueAction<in T, TValue>(T item, ref TValue value);
    public static bool AllComponents<TComponent>(this GameObject obj, Func<TComponent, bool> predicate)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponents(list);
            foreach (var item in list)
            {
                if (!predicate(item))
                    return false;
            }

            return true;
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    /// <summary>Removes <see cref="UnityEngine.Object"/> null ref. <see cref="HashSet{T}.Remove"/>(null) won't work for unity objects</summary>
    public static void RemoveNull<T>(this HashSet<T> source) where T: UnityEngine.Object => source.RemoveWhere(item => item == null);

    /// <summary> Checks if all components in children (including <paramref name="obj"/> components)  matches <paramref name="predicate"/>. By default it ignores inactive game objects, but <paramref name="includeInactive"/> may be set to <c>true</c> to include them. </summary>
    public static bool AllComponentsInChildren<TComponent>(this GameObject obj, Func<TComponent, bool> predicate, bool includeInactive = false)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponentsInChildren(includeInactive, list);
            foreach (var item in list)
            {
                if (!predicate(item))
                    return false;
            }

            return true;
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    /// <summary> Checks <paramref name="predicate"/> for every component returned by <see cref="GameObject.GetComponentsInParent{T}(bool)"/>. Returns <c>true</c> if at least one component matches the <paramref name="predicate"/>. </summary>
    public static bool AnyComponentInParent<TComponent>(this GameObject obj, bool includeInactive, Func<TComponent, bool> predicate)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponentsInParent(includeInactive, list);
            foreach (var item in list)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    /// <summary> Behaviour is not null and is Active and Enabled </summary>
    public static bool NotNullActiveAndEnabled(this Behaviour behaviour) => behaviour != null && behaviour.isActiveAndEnabled;

    /// <summary> Destroy component's game object (if component not null). </summary>
    public static void DestroyGameObjectIfSet(this Component component) { if (component != null) UnityEngine.Object.Destroy(component.gameObject); }

    /// <summary>
    /// Invokes action for each component of specified type with same argument value and returns number of processed components.
    /// </summary>
    /// <param name="obj">game object.</param>
    /// <param name="arg">argument for action.</param>
    /// <param name="action">action to invoke on each component.</param>
    /// <typeparam name="TComponent">component type.</typeparam>
    /// <typeparam name="TArg">argument type.</typeparam>
    /// <returns>number of processed elements.</returns>
    public static int ForEachComponent<TComponent, TArg>(this GameObject obj, TArg arg, Action<TComponent, TArg> action)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponents(list);
            foreach (var item in list)
                action.Invoke(item, arg);
            return list.Count;
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    /// <summary>
    /// Invokes action for each component of specified type and returns number of processed components.
    /// </summary>
    /// <param name="obj">game object.</param>
    /// <param name="action">action to invoke on each component.</param>
    /// <typeparam name="TComponent">component type.</typeparam>
    /// <returns>number of processed elements.</returns>
    public static int ForEachComponent<TComponent>(this GameObject obj, Action<TComponent> action)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponents(list);
            foreach (var item in list)
                action.Invoke(item);
            return list.Count;
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    /// <summary> Returns children components only checking our immediate children, not going recursively.</summary>
    public static List<TComponent> GetComponentsInImmediateChildren<TComponent>(this GameObject obj) where TComponent : TrackableBehavior
    {
        var list = TempLists.Rent<TComponent>();
        foreach (Transform entry in obj.transform)
            if (entry.gameObject.TryGetComponent<TComponent>(out var comp))
                list.Add(comp);
        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEachComponentInChildren<TComponent>(this GameObject obj, Action<TComponent> action) => obj.ForEachComponentInChildren(false, action);

    public static void ForEachComponentInChildren<TComponent>(this GameObject obj, bool includeInactive, Action<TComponent> action)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponentsInChildren(includeInactive, list);
            foreach (var item in list)
                action.Invoke(item);
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    public static void ForEachComponentInChildrenWithArgs<TComponent, TArg0, TArg1>(this GameObject obj, TArg0 arg0, TArg1 arg1, Action<TComponent, TArg0, TArg1> action)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponentsInChildren(list);
            foreach (var item in list)
                action.Invoke(item, arg0, arg1);
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    public static void ForEachComponentInChildrenWithArgs<TComponent, TArg0, TArg1, TArg2>(this GameObject obj, TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TComponent, TArg0, TArg1, TArg2> action)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponentsInChildren(list);
            foreach (var item in list)
                action.Invoke(item, arg0, arg1, arg2);
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    public static void ForEachComponentInChildrenWithValue<TValue, TComponent>(this GameObject obj, ref TValue value, ItemWithValueAction<TComponent, TValue> action) where TValue : struct
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponentsInChildren(list);
            foreach (var item in list)
                action.Invoke(item, ref value);
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEachComponentInChildrenWithValue<TComponent, TValue>(this GameObject obj, TValue value, Action<TComponent, TValue> action)
        => obj.ForEachComponentInChildrenWithValue(value, false, action);

    /// <summary>
    /// Invokes specified action for every child component of specified type with a context value.
    /// </summary>
    /// <param name="obj">root game object.</param>
    /// <param name="value">context value.</param>
    /// <param name="includeInactive">should it include inactive components or not.</param>
    /// <param name="action">action to invoke.</param>
    /// <typeparam name="TComponent">component type.</typeparam>
    /// <typeparam name="TValue">context value type.</typeparam>
    public static void ForEachComponentInChildrenWithValue<TComponent, TValue>(this GameObject obj, TValue value, bool includeInactive, Action<TComponent, TValue> action)
    {
        var list = TempLists.Rent<TComponent>();
        try
        {
            obj.GetComponentsInChildren(includeInactive, list);
            foreach (var item in list)
                action.Invoke(item, value);
        }
        finally
        {
            TempLists.Return(list);
        }
    }

    /// <summary> Extends standard Unity API with method which allows to lookup for inactive parent components. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetComponentInParent<T>(this Component comp, bool includeInactive) where T : class => comp.gameObject.GetComponentInParent<T>(includeInactive);

    //Get the component in parent, including deactivated game objects if includeInactive set (GC effective)
    public static T GetComponentInParent<T>(this GameObject obj, bool includeInactive) where T : class
    {
        if (!includeInactive)
            return obj.GetComponentInParent<T>();

        var list = ComponentLists<T>.List;
        obj.GetComponentsInParent(true, list);
        var result = list.Count > 0 ? list[0] : null;
        list.Clear();
        return result;
    }

    public static void GetComponents<TSrcComp, TDestComp>(this IList<TSrcComp> components, List<TDestComp> results) where TSrcComp : Component where TDestComp : Component
    {
        results.Clear();
        var count = components.Count;
        var list = ComponentLists<TDestComp>.List;
        for (var i = 0; i < count; i++)
        {
            var component = components[i];
            component.GetComponents(list);
            var foundCount = list.Count;
            for (var j = 0; j < foundCount; j++)
                results.Add(list[j]);
        }
        list.Clear();
    }

    /// <summary> Returns number of components of type <typeparamref name="TComponent"/> for <paramref name="gameObject"/>. </summary>
    public static int GetComponentsCount<TComponent>(this GameObject gameObject)
    {
        try
        {
            gameObject.GetComponents(ComponentLists<TComponent>.List);
            return ComponentLists<TComponent>.List.Count;
        }
        finally
        {
            ComponentLists<TComponent>.List.Clear();
        }
    }

    public static void GetComponentsInChildren<T>(this Component component, bool includeInactive, ISet<T> result) => component.gameObject.GetComponentsInChildren(includeInactive, result);

    public static void GetComponentsInChildren<T>(this GameObject gameObject, bool includeInactive, ISet<T> result)
    {
        result.Clear();
        gameObject.GetComponentsInChildren(includeInactive, ComponentLists<T>.List);
        result.AddRange(ComponentLists<T>.List);
    }

    public static void GetComponentsInChildrenNotRecursive<T>(this Component comp, bool includeInactive, List<T> results)
    {
        results.Clear();
        var list = ComponentLists<T>.List;
        var transform = comp.transform;
        var childCount = transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!(includeInactive || child.gameObject.activeInHierarchy))
                continue;

            child.GetComponents(list);
            results.AddRange(list);
        }
        list.Clear();
    }

    //Get the children of a given transform recursively, depth-first, but skip ones that return 'false' to a 'peruseChildren' call.
    public static IEnumerable<T> GetComponentsInSpecificChildren<T>(this Transform t, bool includeSelf, Func<Transform, bool> peruseChildren)
    {
        var s = new Stack<Transform>();
        s.Push(t);
        while (s.Any())
        {
            var obj = s.Pop();

            if (includeSelf || obj != t)
            {
                var comp = obj.GetComponent<T>();
                if (comp != null) yield return comp;
            }

            if (peruseChildren(obj.transform))
                s.PushRange(obj.transform.Children());
        }
    }

    /// <summary> Checks if <paramref name="component"/> has a component of type <typeparamref name="T"/> in children. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasComponentInChildren<T>(this Component component, bool includeInactive = false) => component.GetComponentInChildren<T>(includeInactive) != null;

    /// <summary> Checks if <paramref name="component"/> has a component of type <typeparamref name="T"/> in parent. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasComponentInParent<T>(this Component component, bool includeInactive = false) => component.GetComponentInParent<T>(includeInactive) != null;

    /// <summary> Checks requested component in children, and creates one on target object if not found </summary>
    public static T GetComponentInChildrenOrCreate<T>(this GameObject go) where T : Component
    {
        var target = go.GetComponentInChildren<T>();
        if (target == null) target = go.AddComponent<T>();
        return target;
    }
    
    public static T IfComponentExists<T>(this GameObject obj, Action<T> func)
    {
        var comp = obj.GetComponent<T>();
        if (comp != null)
            func(comp);
        return comp;
    }

    public static T IfComponentInChildrenExists<T>(this GameObject obj, Action<T> func)
    {
        var comp = obj.GetComponentInChildren<T>();
        if (comp != null)
            func(comp);
        return comp;
    }

    /// <summary> Initializes <paramref name="component"/> with <paramref name="behaviour"/>'s component if <paramref name="component"/> not set. </summary>
    public static T InitWithComponentIfNotSet<T>(this MonoBehaviour behaviour, ref T component) where T : Component
    {
        if (component == null)
            component = behaviour.GetComponent<T>();
        return component;
    }

    public static void SetRecursiveReadOnly(this GameObject beh, bool readOnly)
    {
        foreach (var entry in beh.GetComponentsInChildren<IReadOnly>(true))
            entry.SetReadOnly(readOnly);
    }

    //This is usefull for check monobehaviors that implements some interface. You should not check directly == null since destroyed unity object will still be accesible by interface ref. == of UnityEngine.Component also checks for destroyed state
    public static bool UnityCheckIsNull(this object o) => (o == null || o is Component component && !component);

    /// <summary>Copy a component with the fields values to the new game object. Record all fields, including private.</summary>
    public static T CopyComponent<T>(this GameObject destination,  T original) where T : Component
    {
        var type = original.GetType();                                                                    //This is a component we want to copy.
        var copy = destination.AddComponent(type);                                                   //And this is a new component which will be a copy.
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public); //Take all fields we can find on our component type, including private.
        foreach (var field in fields)                                                                      //Copy values of all fields from original component to the new one.
            field.SetValue(copy, field.GetValue(original));

        return copy as T;
    }
}

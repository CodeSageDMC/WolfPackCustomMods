// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Eco.Shared.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Object      = UnityEngine.Object;
using Text        = Eco.Shared.Utils.Text;
using Color       = UnityEngine.Color;
using BoxGeometry = Unity.Physics.BoxGeometry;
using System.Buffers;
using Eco.Shared.View;
using System.Diagnostics;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public static class UnityGameObjectUtils
{
    /// <summary>Checks if the game object has the specified parent or ancestor. If passed the same object, returns true.</summary>
    /// <param name="child">The game object to check.</param>
    /// <param name="ancestor">The game object to look for in the ancestry chain.</param>
    /// <returns>True if the ancestor is found; otherwise, false.</returns>
    public static bool HasAncestor(this GameObject child, GameObject ancestor)
    {
        if (child == ancestor) return true;
        Transform currentParent = child.transform.parent;
        while (currentParent != null)
        {
            if (currentParent.gameObject == ancestor)
                return true;
            currentParent = currentParent.parent;
        }
        return false;
    }

    public static void AddPointerDownHandler(this GameObject go, Action<PointerEventData> action) => go.GetOrAddComponent<PointerDownHandler>().OnPointerDownInvoked += action;

    public static Transform DelayedDestroyRoot;

    const float MinDepenetrateDistance = 0.001f;

    private static readonly IComparer<SpreadEntry> SpreadEntryComparer = new SpreadComparer();

    /// <summary> cached mask that checks every layer except the player layer.</summary>
    public static LayerMask? PlayerInvertedMask = null;

    public static bool MemoryRequirementsMet => SystemInfo.graphicsMemorySize >= 2048 && SystemInfo.systemMemorySize >= 8192;

    public static Joint AddJointBySample(this GameObject go, GameObject root, Joint sample)
    {
        if (sample is CharacterJoint characterJoint)
            return go.AddJointBySample(root, characterJoint);
        var joint = (Joint)go.AddComponent(sample.GetType());
        joint.ApplySample(root, sample);
        Debug.LogError($"Unknown joint type: {sample.GetType()}. Can't fully apply sample data.");
        return joint;
    }

    public static CharacterJoint AddJointBySample(this GameObject go, GameObject root, CharacterJoint sample)
    {
        var joint = go.AddComponent<CharacterJoint>();
        joint.ApplySample(root, sample);
        joint.swingAxis = sample.swingAxis;
        joint.twistLimitSpring = sample.twistLimitSpring;
        joint.lowTwistLimit = sample.lowTwistLimit;
        joint.highTwistLimit = sample.highTwistLimit;
        joint.swingLimitSpring = sample.swingLimitSpring;
        joint.swing1Limit = sample.swing1Limit;
        joint.swing2Limit = sample.swing2Limit;
        joint.enableProjection = sample.enableProjection;
        joint.projectionDistance = sample.projectionDistance;
        joint.projectionAngle = sample.projectionAngle;
        return joint;
    }

    public static Rigidbody AddRigidbodyBySample(this GameObject go, Rigidbody sample)
    {
        var newRigidbody = go.AddComponent<Rigidbody>();
        newRigidbody.mass = sample.mass;
        newRigidbody.drag = sample.drag;
        newRigidbody.angularDrag = sample.angularDrag;
        newRigidbody.useGravity = sample.useGravity;
        newRigidbody.isKinematic = sample.isKinematic;
        newRigidbody.interpolation = sample.interpolation;
        newRigidbody.detectCollisions = sample.detectCollisions;
        newRigidbody.collisionDetectionMode = sample.collisionDetectionMode;
        newRigidbody.constraints = sample.constraints;
        return newRigidbody;
    }

    public static void BroadcastAll(string fun, Object msg)
    {
        GameObject[] gos = (GameObject[])GameObject.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);
        foreach (GameObject go in gos)
        {
            if (go && go.transform.parent == null)
            {
                go.gameObject.BroadcastMessage(fun, msg, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    //Take any \t's in a string and turn them into a table.
    public static void BuildTable(this IEnumerable<TextMeshProUGUI> list, int spread = 10)
    {
        var listOfColWidths = list.Select(entry => entry.text.Split('\t').Select(col =>
        {
            var width = entry.GetPreferredValues(col).x;
            //var iconwidth = 4 * Regex.Matches(col, "<sprite").Count;
            return width;// + iconwidth;
        }).ToArray()).ToArray();

        var colcount = listOfColWidths.Max(x => x.Count());
        var maxCol = new float[colcount];
        for (int i = 0; i < colcount; i++)
            maxCol[i] = listOfColWidths.Max(x => x.Length > i ? x[i] : 0);

        //Sum previous columns to next columns
        for (int i = 0; i < colcount; i++)
            for (int j = i + 1; j < colcount; j++)
                maxCol[j] += maxCol[i] + spread;

        //Apply
        foreach (var entry in list)
        {
            entry.text = entry.text.Split('\t').Select((text, i) =>
            {
                if (i == 0) return text;
                return Text.Pos((int)maxCol[i - 1], text);
            }).TextList();
        }
    }

    public static bool ContactedBy(this Collision collision, Collider collider)
    {
        for (var i = 0; i < collision.contactCount; i++)
            if (collision.GetContact(i).HasCollider(collider))
                return true;
        return false;
    }

    public static bool ContactedByAny<T>(this Collision collision, IList<T> colliders) where T : Collider
    {
        for (var i = 0; i < collision.contactCount; i++)
        {
            var thisCollider = collision.GetContact(i).thisCollider;
            var otherCollider = collision.GetContact(i).otherCollider;
            for (var j = 0; j < colliders.Count; j++)
            {
                var collider = colliders[j];
                if (ReferenceEquals(thisCollider, collider) || ReferenceEquals(otherCollider, collider))
                    return true;
            }
        }

        return false;
    }

    public static Vector2 ConvertFromCoords(this Transform tTarget, Transform source, Vector2 p)
    {
        var world = source.transform.TransformPoint(p);
        var local = tTarget.InverseTransformPoint(world);
        return local;
    }

    public static void DestroyActiveChildren(this Transform transform, bool forceDestroyImmediate = false)
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).gameObject.activeSelf)
                DestroyHelper.Destroy(transform.GetChild(i).gameObject, forceDestroyImmediate);
    }

    /// <summary>
    /// This one may be helpful when you trying to remove all children of toggle group with allowSwitchOff = false
    /// It will prevent callbacks of toggles caused by assignation of new active toggle
    /// </summary>
    public static void DestroyActiveChildrenAndPreventToggleGroupCallbacks(this Transform transform, ToggleGroup group)
    {
        if (group == null)
        {
            transform.DestroyActiveChildren();
            return;
        }

        bool oldValue = false;
        oldValue = group.allowSwitchOff;
        group.allowSwitchOff = true;
        transform.DestroyActiveChildren(true);
        group.allowSwitchOff = oldValue;
    }

    /// <summary>
    /// This one may be helpful when you trying to remove all children of toggle group with allowSwitchOff = false
    /// It will prevent callbacks of toggles caused by assignation of new active toggle
    /// </summary>
    /// <param name="transform"></param>
    public static void DestroyActiveChildrenAndPreventToggleGroupCallbacks(this Transform transform)
    {
        var group = transform.GetComponent<ToggleGroup>();
        transform.DestroyActiveChildrenAndPreventToggleGroupCallbacks(group);
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        if (transform == null) return;
        for (var i = transform.childCount - 1; i >= 0; i--)
            DestroyHelper.Destroy(transform.GetChild(i).gameObject);
    }

    public static void DestroyAllChildren(this Transform transform, Func<GameObject, bool> testFunc, bool forceDestroyImmediate = false)
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var childObj = transform.GetChild(i).gameObject;
            if (childObj != null && testFunc(childObj))
                DestroyHelper.Destroy(childObj, forceDestroyImmediate);
        }
    }

    public static void DestroyIfSet(this UnityEngine.Object obj)
    {
        if (obj != null) UnityEngine.Object.Destroy(obj);
    }

    /// <summary>
    /// Detaches <see cref="gameObject"/> from parent transform and <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)"/> it.
    /// You can use it as safe alternative to <see cref="UnityEngine.Object.DestroyImmediate(UnityEngine.Object)"/> if you need to remove the <paramref name="gameObject"/> from hierarchy immediately.
    /// </summary>
    public static void DetachAndDestroy(this GameObject gameObject)
    {
        // destroyRoot may only fail check if scene is destroying
        if (!ReferenceEquals(DelayedDestroyRoot, null))
            // detach from current parent (use custom destroy root)
            gameObject.transform.SetParent(DelayedDestroyRoot);

        UnityEngine.Object.Destroy(gameObject);
    }


    public static string GamePath(this Transform t)
    {
        var sb = new StringBuilder();
        while (t != null)
        {
            sb.Append(t.gameObject.name);
            sb.Append($"/{t.name}");
            t = t.parent;
        }
        return sb.ToString();
    }

    /// <summary>Retrieves an int <see cref="List{T}"/> from the user's PlayerPrefs.</summary>
    /// <param name="key">Key of the player pref to retrieve.</param>
    /// <returns>List containing the items loaded from PlayerPrefs.</returns>
    public static List<int> GetIntArrayFromPrefs(this string key) => 
        PlayerPrefs.HasKey(key) ? 
            PlayerPrefs
                .GetString(key)
                .Split(',')
                .Select(x => Convert.ToInt32(x)).ToList() : 
            new List<int>();

    /// <summary>Retrieves a string <see cref="List{T}"/> from the user's PlayerPrefs.</summary>
    /// <inheritdoc cref="GetIntArrayFromPrefs(string)"/>
    public static List<string> GetStringArrayFromPrefs(this string key) =>
        PlayerPrefs.HasKey(key) ?
            PlayerPrefs
                .GetString(key)
                .Split(',')
                .ToList() :
            new List<string>();

    public static bool HasChild(this Transform t, Transform test) => (test.parent == t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasCollider(this ContactPoint contact, Collider collider) => ReferenceEquals(contact.thisCollider, collider) || ReferenceEquals(contact.otherCollider, collider);

    public static void Instantiate<TView, TPrefab>(this IEnumerable<TView> viewList, TPrefab prefab, Transform parent, Action<TView, TPrefab> init, bool destroyChildren = true)
         where TPrefab : TrackableBehavior
    {
        if (destroyChildren) parent.DestroyAllChildren(x => x.activeSelf);
        foreach (var view in viewList)
        {
            var obj = GameObject.Instantiate(prefab, parent, false);
            obj.gameObject.SetActive(true);
            init?.Invoke(view, obj);
        }
    }

    /// <summary>Instantiate object from <paramref name="prefab"/> with null safety check. If <paramref name="prefab"/> is destroyed or null then returns <c>null</c>.</summary>
    public static T InstantiateNullSafe<T>(this T prefab) where T : UnityEngine.Object  => prefab != null ? UnityEngine.Object.Instantiate(prefab) : null;

    /// <summary>Returns all game objects in <paramref name="scene"/>.</summary>
    public static IEnumerable<GameObject> GetAllSceneObjects(Scene scene) => scene.GetRootGameObjects().SelectMany(go => go.transform.GetHierarchy()).Select(t => t.gameObject);

    public static bool IsPrefab(this GameObject go) => !go.scene.IsValid();

    public static bool IsSceneLoaded(string sceneName_no_extention)
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name.ToLower() == sceneName_no_extention.ToLower())
                return true;//the scene is already loaded
        }
        //scene not currently loaded in the hierarchy:
        return false;
    }

    public static string LoadLogFile()
    {
        var log = Path.Combine(Application.persistentDataPath, "Player.log");
        var logcopy = Path.Combine(Application.persistentDataPath, "Player.log.copy.txt");

        if (!File.Exists(log)) return "Log File missing.";

        if (File.Exists(logcopy)) File.Delete(logcopy);
        File.Copy(log, logcopy);
        return File.ReadAllText(logcopy);
    }

    /// <summary>based on the bool(set), set the material to gray or default to the GameObject(obj) and all its children.</summary>
    public static void MakeGrayscale(this GameObject obj, bool set, Material defaultMat = null)
    {
        foreach (var child in obj.GetComponentsInChildren<Image>(true))
            child.material = set ? EcoColors.obj.GrayScaleMaterial : defaultMat;
    }

    /// <summary>based on the bool(set), reduce or increase the alpha to the GameObject(obj) and all its children.</summary>
    public static void ReduceAlpha(this GameObject obj, bool set, float defaultAlpha = 1, float reducedAlpha = 0.5f)
    {
        foreach (var child in obj.GetComponentsInChildren<Image>())
            child.color = set ? new Color(child.color.r, child.color.g, child.color.b, reducedAlpha) : new Color(child.color.r, child.color.g, child.color.b, defaultAlpha);
    }

    /// <summary>Set alpha in byte format [0..255]</summary>
    public static void SetAlpha(this Image image, int alpha) => SetAlpha(image, alpha / 255f);
    public static void SetAlpha(this Image image, float alpha) => image.color = image.color.WithAlpha(alpha);

    public static bool OnScreen(Vector3 worldPos, float marginPercent, out Vector3 screenPos)
    {
        screenPos = UtilCache.MainCamera.WorldToScreenPoint(worldPos);

        var cameraRelative = UtilCache.MainCamera.transform.InverseTransformPoint(worldPos); //convert to local position to check if is in front of the camera
        if (cameraRelative.z < 0) return false;

        return screenPos.x > Screen.width * marginPercent &&
               screenPos.x < Screen.width * (1 - marginPercent) &&
               screenPos.y > Screen.height * marginPercent &&
               screenPos.y < Screen.height * (1 - marginPercent);
    }


    ///<summary>
    ///Returns size of a box according to Camera's near clip plane height and width, and where to position it such that a face of the box coincides with the MainCamera's nearClipPlane
    ///used mainly for <see cref="IsCameraClipping"/> and may also be used for Gizmos.
    ///</summary>
    public static BoxGeometry GetBoxAtClipPlane()
    {
        var cam = UtilCache.MainCamera;
        // We get the size of the nearClipPlane and then use it to get the size of the box to use.
        var nearClipPlaneSize = GetNearClipPlaneSize(cam);
        var size   = new Vector3(nearClipPlaneSize.x, nearClipPlaneSize.y , cam.nearClipPlane * 0.5f);
        //ensures the checkbox face is aligned with the nearClipPlane
        return new BoxGeometry { Center = cam.transform.position + cam.transform.forward * cam.nearClipPlane, Orientation = cam.transform.rotation, Size = size };
    }

    /// <summary> Calculates the size of the near clip plane of a given camera by using it's FOV and nearClipPlane values </summary>
    public static Vector2 GetNearClipPlaneSize(Camera camera)
    {
        // We use half the angle of FOV to find half the height of the nearClipPlane | tan(fov/2) = half height / nearClip | --> halfHeight = tan(fov/2) * nearClip
        var height = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * camera.nearClipPlane;

        // After calculating height we can calculate it's width by using the camera's aspect.
        var width  = height * camera.aspect;
        return new Vector2(width * 2, height * 2);
    }

    public static LayerMask GetInvertedLayerMaskFromName(string name) => InvertLayerMask(LayerMask.NameToLayer(name));
    public static LayerMask InvertLayerMask(LayerMask mask) => ~(1 << mask);

    public static bool ParticlesAlive(this GameObject obj)
    {
        foreach (var particle in obj.GetComponentsInChildren<ParticleSystem>())
        {
            if (particle.IsAlive(true))
                return true;
        }
        return false;
    }

    public static float RandomInRange(this Vector2 range) => UnityEngine.Random.Range(range.x, range.y);


    public static void RotateTowards(this Transform transform, Vector3 pos, float percent)
    {
        Vector3 direction = pos - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, percent);
    }

	/// <summary>Converts <param name="quaternion"/> to 4-component vector holding same components as the quaternion.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 ToVector(this in Quaternion quaternion) => new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);

    public static void RotateTowards(this Transform transform, Quaternion toRotation, float percent)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, percent);
    }

    public static void RunAfterDelay(this MonoBehaviour obj, float delay, Action action)
    {
        if (delay == 0) { action.Invoke(); return; }
        obj.StartCoroutine(DelayHelper(delay, action));
    }

    /// <summary>Saves an integer <see cref="IEnumerable"/> to the user's PlayerPrefs.</summary>
    /// <param name="array">IEnumerable to store/</param>
    /// <param name="key">Key to store the IEnumerable under.</param>
    public static void SaveToPrefs(this IEnumerable<int> array, string key)     => PlayerPrefs.SetString(key, string.Join(",", array.Select(x => x.ToString())));

    /// <summary>Saves a string <see cref="IEnumerable"/> to the user's PlayerPrefs.</summary>
    /// <inheritdoc cref="SaveToPrefs(IEnumerable{int}, string)"/>
    public static void SaveToPrefs(this IEnumerable<string> array, string key)  => PlayerPrefs.SetString(key, string.Join(",", array.Select(x => x.ToString())));

    /// <summary>Saves a float <see cref="IEnumerable"/> to the user's PlayerPrefs.</summary>
    /// <inheritdoc cref="SaveToPrefs(IEnumerable{int}, string)"/>
    public static void SaveToPrefs(this IEnumerable<float> array, string key)   => PlayerPrefs.SetString(key, string.Join(",", array.Select(x => x.ToString())));

    /// <summary>Saves a double <see cref="IEnumerable"/> to the user's PlayerPrefs.</summary>
    /// <inheritdoc cref="SaveToPrefs(IEnumerable{int}, string)"/>
    public static void SaveToPrefs(this IEnumerable<double> array, string key)  => PlayerPrefs.SetString(key, string.Join(",", array.Select(x => x.ToString())));

    /// <summary>Saves a bool <see cref="IEnumerable"/> to the user's PlayerPrefs.</summary>
    /// <inheritdoc cref="SaveToPrefs(IEnumerable{int}, string)"/>
    public static void SaveToPrefs(this IEnumerable<bool> array, string key)    => PlayerPrefs.SetString(key, string.Join(",", array.Select(x => x.ToString())));


    public static void SetActive(this IEnumerable<MonoBehaviour> list, bool set)
    {
        foreach (var entry in list)
            entry.gameObject.SetActive(set);
    }

    /// <summary>Itereate through this IEnumerator until its done.</summary>
    /// <param name="e"></param>
    public static void IterateFully(this IEnumerator e)
    {
        while(e.MoveNext());
    }

    public static void SetPixelRect(this Texture2D texture, Rect r, Color c)
    {
        texture.SetPixels((int)r.xMin, (int)r.yMin, (int)r.width, (int)r.height, Enumerable.Repeat(c, (int)r.Area()).ToArray());
    }

    public static void SetX(this Transform obj, float x) { var pos = obj.position; pos.x = x; obj.position = pos; }

    public static void SetX(this GameObject obj, float x) { var pos = obj.transform.position; pos.x = x; obj.transform.position = pos; }

    public static void SetX(this RectTransform obj, float x) { var pos = obj.anchoredPosition; pos.x = x; obj.anchoredPosition = pos; }

    public static void SetY(this Transform obj, float y) { var pos = obj.position; pos.y = y; obj.position = pos; }

    public static void SetY(this GameObject obj, float y) { var pos = obj.transform.position; pos.y = y; obj.transform.position = pos; }

    public static void SetY(this RectTransform obj, float y) { var pos = obj.anchoredPosition; pos.y = y; obj.anchoredPosition = pos; }

    public static void SetZ(this Transform obj, float z) { var pos = obj.position; pos.z = z; obj.position = pos; }

    public static void SetZ(this GameObject obj, float z) { var pos = obj.transform.position; pos.z = z; obj.transform.position = pos; }

    //Spread out a number of game objects vertically or horizontally, within bounds.
    public static void Spread<TComponent>(this TComponent[] components, float min, float max, bool vertical, Func<Component, bool> filter) where TComponent : Component
    {
        var pool = ArrayPool<SpreadEntry>.Shared;
        var entries = pool.Rent(components.Length);
        var count = 0;

        //check if the items in the components pass the filter and get its position
        foreach (var item in components)
        {
            if (!filter(item))
                continue;
            var transform = item.transform;
            entries[count++] = new SpreadEntry { transform = transform, pos = Mathf.Round(GetSpreadVal(transform, vertical)) };
        }

        // If there is more than 1 item to spread, then sort them by position if not then name or by id.
        // We also want to sepparate the entries by how many there are (if there are a lot then space them by little, otherwise spread them as much as we can max 50 (this number can be changed if needed))
        // We need to have in mind that we are stacking the entries Y+. so if we reach the max height we need to put them under the first entrie.
        // so we evaluate if we reached the max height if so then we do the same process but now substracting and under the first entrie.
        if (count > 1)
        {
            Array.Sort(entries, 0, count, SpreadEntryComparer);

            //we set the spacing depending on how many entries we have (setting a min and a max spacing)
            var spacing = Mathf.Clamp((max - min) / count, 10, 50);
            var reachedTop = false;
            var tempPos = entries[0].pos;

            // we are basically getting the first entrie as the point of reference and each entrie adding the spacing size to the last entrie registered, so it makes a stack of entries separated with the same size
            // we need to save the position of the last entrie in tempPos because it can change if we reach the top, if we reach the top of the screen, what we do is
            // get the first entrie in the array and substract the space from there, so the stack keeps growing but underneath the base entrie.
            for (var j = 0; j < entries.Length; j++)
            {
                if (reachedTop == false)
                {
                    var nextPos = Mathf.Clamp(tempPos + spacing, min, max);
                    if (nextPos == max && reachedTop == false)
                    {
                        reachedTop = true;
                        tempPos = Mathf.Clamp(entries[0].pos, min, max);
                    }
                }

                if (reachedTop)
                    entries[j].pos = Mathf.Clamp(tempPos - spacing, min, max);
                else
                    entries[j].pos = Mathf.Clamp(tempPos + spacing, min, max);

                tempPos = entries[j].pos;
            }
        }
        else
            entries[0].pos = Mathf.Clamp(entries[0].pos, min, max);

        for (var i = 0; i < count; i++)
            SetSpreadVal(entries[i].transform, vertical, entries[i].pos);
        pool.Return(entries);
    }

    public static void SyncNumChildren(this Transform t, GameObject prefab, int count)
    {
        while (t.childCount < count)
            GameObject.Instantiate(prefab, t, false);

        while (t.childCount > count)
            t.RemoveChild(0);
    }

    public static void SyncWithView(this Toggle toggle, ISubscriptions<Subscriptions> subscriptions, View view, string propname)
    {
        subscriptions.SubscribeAndCall(view, propname, () => toggle.isOn = (bool)view.GetValue(propname));
        toggle.onValueChanged.AddListener(val => view.RPC("Set" + propname, val));
    }

    [Conditional("UNITY_EDITOR"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNotUnityThread(string name)
    {
#if UNITY_EDITOR
        if (!UnityEditorInternal.InternalEditorUtility.CurrentThreadIsMainThread())
            throw new NotSupportedException($"Can't run {name} not on Unity thread.");
#endif
    }
    static Animator hostAnimator;
    static Animator GetHostAnimator() => hostAnimator == null ? (hostAnimator = new GameObject("Animators (used to bypass unity animator limitation)").AddComponent<Animator>()) : hostAnimator;
    /// <summary> Animators in unity won't return any parameter unless it was activated. It bypass this limitation by creating and activating a copy of this animator if necessary.  </summary>
    public static IEnumerable<UnityEngine.AnimatorControllerParameter> GetParametersSafe(this Animator animator)
    {
        if (animator.isInitialized)
            return animator.parameters;

        //if there are no parameters, maybe it's situation when unity won't give you nothing until animator object is enabled
        //In this case to bypass it we may just create animator at some active object and get parameters from there
        var newAnimator = GetHostAnimator();
        newAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
        var parameters = newAnimator.parameters;
            newAnimator.runtimeAnimatorController = null;
            return parameters;
    }

    public static Color ToUnityColor(int val)
    {
        var a = (byte)((val >> (8 * 3)) & 0xff);
        var r = (byte)((val >> (8 * 2)) & 0xff);
        var g = (byte)((val >> (8 * 1)) & 0xff);
        var b = (byte)((val >> (8 * 0)) & 0xff);
        return new Color32(r, g, b, a);
    }

    /// <summary>
    /// Traverse <paramref name="obj"/> hierarchy starting from <paramref name="obj"/>.
    /// It will continue traverse for children until <paramref name="visit"/> function return <c>false</c>.
    /// I.e. if you return <c>false</c> for <paramref name="obj"/> itself then it won't visit any children and if you return <c>false</c>
    /// for first child then it won't continue with this child hierarchy, but will visit other children and their children until <c>false</c> returned.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Traverse(this GameObject obj, Func<GameObject, bool> visit)
    {
        if (!visit(obj))
            return;
        TraverseChildren(obj, visit);
    }

    /// <summary>
    /// Traverse <paramref name="obj"/> hierarchy starting from <paramref name="obj"/> and passes provided <paramref name="context"/> to every <paramref name="visit"/> call.
    /// It will continue traverse for children until <paramref name="visit"/> function return <c>false</c>.
    /// I.e. if you return <c>false</c> for <paramref name="obj"/> itself then it won't visit any children and if you return <c>false</c>
    /// for first child then it won't continue with this child hierarchy, but will visit other children and their children until <c>false</c> returned.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Traverse<T>(this GameObject obj, T context, Func<GameObject, T, bool> visit)
    {
        if (!visit(obj, context))
            return;
        TraverseChildren(obj, context, visit);
    }

    /// <summary> Same as <see cref="Traverse(UnityEngine.GameObject,System.Func{UnityEngine.GameObject,bool})"/>, but starts with children collection (doesn't call <paramref name="visit"/> for <paramref name="obj"/> itself). </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TraverseChildren(this GameObject obj, Func<GameObject, bool> visit)
    {
        foreach (var child in obj.transform.Children())
            Traverse(child.gameObject, visit);
    }

    /// <summary> Same as <see cref="Traverse{T}(UnityEngine.GameObject,T,System.Func{UnityEngine.GameObject,T,bool})"/>, but starts with children collection (doesn't call <paramref name="visit"/> for <paramref name="obj"/> itself). </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TraverseChildren<T>(this GameObject obj, T context, Func<GameObject, T, bool> visit)
    {
        foreach (var child in obj.transform.Children())
            Traverse(child.gameObject, context, visit);
    }

    public static bool TryGetContactBy(this Collision collision, Collider collider, out ContactPoint contact)
    {
        for (var i = 0; i < collision.contactCount; i++)
        {
            contact = collision.GetContact(i);
            if (contact.HasCollider(collider))
                return true;
        }

        contact = default;
        return false;
    }

    public static IEnumerator ValueLerper(Action<float> whatToDo, float fromValue, float toValue, float duration, Action onFinished = null)
    {
        float progress = 0;
        while (progress < 1)
        {
            yield return null;
            progress += Time.deltaTime / duration;
            whatToDo(Mathf.Lerp(fromValue, toValue, progress));
        }
        onFinished?.Invoke();
    }

    /// <summary> This will position popup so it is attached to parent control and have max space.
    /// It calculates free space window will have, checking if center is going away the screen to the left and top.
    /// And then decides where to open a popup using appropriate corner of a parent rectangle..</summary>
    public static void PositionPopup(this RectTransform popupRect, RectTransform parentRect)
    {
        // World position to put popup on same side
        var parentCorners = new Vector3[4];
        parentRect.GetWorldCorners(parentCorners);

        // Decide which sides to use based on where more free space
        Vector2 center = parentRect.ToScreenSpace().center;
        bool closerToLeft = center.x < Screen.width / 2f;
        bool closerToTop = center.y > Screen.height / 2f;
        // Array of corners contains (0,0) (0,1) (1,0) (1,1) positions. So let's find index acording closer sides.
        int positionIndex = closerToLeft ? (closerToTop ? 0 : 1) : (closerToTop ? 3 : 2);

        // Setting calculated position
        popupRect.transform.position = parentCorners[positionIndex];
        popupRect.pivot = new Vector2(closerToLeft ? 0 : 1, closerToTop ? 1 : 0);

        UnityUIUtils.ClampToScreen(popupRect);
    }

    public static void ZeroTransform(this Transform t)
    {
        t.transform.localScale = Vector3.one;
        t.transform.localRotation = Quaternion.identity;
        t.transform.localPosition = Vector3.zero;
    }

    private static void ApplySample(this Joint joint, GameObject root, Joint sample)
    {
        joint.connectedBody = root.transform.FindChildRecursive(sample.connectedBody.name).GetComponent<Rigidbody>();
        joint.anchor = sample.anchor;
        joint.axis = sample.axis;
        joint.autoConfigureConnectedAnchor = sample.autoConfigureConnectedAnchor;
        joint.connectedAnchor = sample.connectedAnchor;
        joint.breakForce = sample.breakForce;
        joint.breakTorque = sample.breakTorque;
        joint.enableCollision = sample.enableCollision;
        joint.enablePreprocessing = sample.enablePreprocessing;
        joint.massScale = sample.massScale;
        joint.connectedMassScale = sample.connectedMassScale;
    }

    private static Comparison<TKey> CompareWithPredicate<TKey, TValue>(Func<TKey, TValue> predicate) where TValue : IComparable
    {
        return new Comparison<TKey>((l, r) => predicate(l).CompareTo(predicate(r)));
    }
    private static bool ComputePenetrationTowardPoint(Vector3 origin, Vector3 point, LayerMask layerMask, out Vector3 direction, out float distance)
    {
        var delta = point - origin;
        var maxDistance = delta.magnitude;
        var rayDirection = (origin - point).normalized;
        if (!Physics.Raycast(point, rayDirection, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            direction = default;
            distance = default;
            return false;
        }

        distance = maxDistance - hitInfo.distance;
        direction = -rayDirection;
        return distance >= MinDepenetrateDistance;
    }
    private static IEnumerator DelayHelper(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetSpreadVal(Transform trans, bool vertical) { return vertical ? trans.position.y : trans.position.x; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetSpreadVal(Transform trans, bool vertical, float val) { if (vertical) trans.SetY(val); else trans.SetX(val); }
    private struct SpreadEntry
    {
        public float pos;
        public Transform transform;
    }

	private class SpreadComparer : IComparer<SpreadEntry>
	{
		public int Compare(SpreadEntry one, SpreadEntry other)
		{
			var cmp = one.pos.CompareTo(other.pos);
			if (cmp != 0)
				cmp = string.Compare(one.transform.name, other.transform.name, StringComparison.Ordinal);
			if (cmp != 0)
				cmp = one.transform.GetInstanceID().CompareTo(other.transform.GetInstanceID());
			return cmp;
		}
	}

	/// <summary>Subscribe to an event and then call it now with the passed curval.</summary>
	public static void AddListenerAndCall<T>(this UnityEvent<T> e, UnityAction<T> call, T curVal)
	{
		e.AddListener(call);
		call.Invoke(curVal);
	}

    /// <summary>Set the visibility of a canvas group either instatntly, or otherwise animate it.</summary>
    /// <param name="instant">If set to true, the visibility will be animated. Otherwise it will happen instantly.</param>
    /// <remarks>This is different from how <see cref="Fader"/> works, as it relies on <see cref="DOTween"/> and allows for control over the level of transparency for an element.</remarks>
    public static void SetOrAnimateVisibility(this CanvasGroup canvasGroup, float alphaEndValue, float fadeDuration = 1, object owner = null, bool instant = true, Ease ease = Ease.OutQuad, Action onComplete = null)
    {
        //If not instant, do not apply animations
        if (instant)
        {
            canvasGroup.alpha = alphaEndValue;
            if (onComplete != null) onComplete();
        }
        else
        {
            var tween = canvasGroup.DOFade(alphaEndValue, fadeDuration).SetEase(ease).SetId(owner);
            if (onComplete != null) tween.OnComplete(() => onComplete());
        }
    }

    /// <summary>Animate canvas group visibility while also setting interactable and blocksRaycasts flags if needed. Also invoked a delayed callback when animation is finished.</summary>
    public static void SetVisibleAnimated(this GameObject gameObject, bool visible, float fadeDuration = 0.5f, object owner = null, Ease ease = Ease.OutQuad, bool forceInteraction = false, Action onComplete = null)
        => SetVisibleAnimated(gameObject.GetOrAddComponent<CanvasGroup>(), visible, fadeDuration, owner, ease, forceInteraction, onComplete);

    /// <summary>Animate canvas group visibility while also setting interactable and blocksRaycasts flags if needed. Also invoked a delayed callback when animation is finished.</summary>
    public static void SetVisibleAnimated(this CanvasGroup canvasGroup, bool visible, float fadeDuration = 0.5f, object owner = null, Ease ease = Ease.OutQuad, bool forceInteraction = false, Action onComplete = null)
    {
        if (forceInteraction)
        {
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable   = visible;
        }
        var tween = canvasGroup.DOFade(visible ? 1f : 0f, fadeDuration).SetEase(ease).SetId(owner);
        if (onComplete != null) tween.OnComplete(() => onComplete());
    }

    /// <summary>Set canvas group visible instanly while also setting additional CanvasGroup settings.</summary>
    public static void SetVisibleInstant(this Component component, bool visible, bool forceInteraction = false)
        => SetVisibleInstant(component.gameObject.GetOrAddComponent<CanvasGroup>(), visible, forceInteraction);

    /// <summary>Set canvas group visible instanly while also setting additional CanvasGroup settings.</summary>
    public static void SetVisibleInstant(this GameObject gameObject, bool visible, bool forceInteraction = false)
        => SetVisibleInstant(gameObject.GetOrAddComponent<CanvasGroup>(), visible, forceInteraction);

    /// <summary>Set canvas group visible instanly while also setting additional CanvasGroup settings.</summary>
    public static void SetVisibleInstant(this CanvasGroup canvasGroup, bool visible, bool forceInteraction = false)
    {
        if (forceInteraction)
        {
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable   = visible;
        }
        canvasGroup.alpha = visible ? 1f : 0f;
    }

    /// <summary>Get the bounds from the collider if it exists, and the renderer if not. </summary>
    public static Bounds? GetAvailableBounds(this GameObject targetObj) => targetObj.GetComponentInChildren<Collider>(true)?.bounds ?? targetObj.GetComponentInChildren<Renderer>()?.bounds;

    /// <summary>Merges together all the collider bounds on a gameobject and returns it.</summary>
    public static Bounds? GetCosolidatedBounds(this GameObject targetObj) 
    { 
        Bounds? bounds = null;
        foreach(var collider in targetObj.GetComponentsInChildren<Collider>())
        { 
            if (bounds == null) bounds = collider.bounds;
            else                bounds = bounds.Value.Merge(collider.bounds);
        }
        return bounds;
    }

    /// <summary>Create a new bounds that minimally covers the two passed bounds.</summary>\
    public static Bounds Merge(this Bounds bounds, Bounds other)
    {
        return new Bounds() 
        { 
            min = new Vector3(Math.Min(bounds.min.x, other.min.x),
                              Math.Min(bounds.min.y, other.min.y),
                              Math.Min(bounds.min.z, other.min.z)),
            max = new Vector3(Math.Max(bounds.max.x, other.max.x),
                              Math.Max(bounds.max.y, other.max.y),
                              Math.Max(bounds.max.z, other.max.z))
        };
    }

    /// <summary>Returns true if component is not null, was not destroyed and is active.</summary>
    public static bool IsActiveAndExists(this Behaviour component) => component != null && component.isActiveAndEnabled;
}

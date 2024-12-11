// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public interface IDelayedDestroy{  void StartDestroy(); }

public static class UnityUIUtils
{
    [Flags]
    public enum ScrollAxis
    {
        Horizontal = 1 << 0,
        Vertical   = 1 << 1
    }
    public enum AnchorMode
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,

        StretchLeft,
        StretchCenter,
        StretchRight,

        StretchTop,
        StretchMiddle,
        StretchBottom,

        StretchStretch
    }

    public static void SetEnabled(GameObject obj, bool enabled)
    {
        Canvas canvas = obj.GetComponent<Canvas>();
        if (canvas == null)
        {
            if (obj.activeSelf != enabled)
                obj.SetActive(enabled);
        }
        else
        {
            if (canvas.enabled != enabled)
                canvas.enabled = enabled;
        }
    }

    /// <summary>Get screen dimensions rectangle of given rect transform</summary>
    public static Rect GetScreenRect(this RectTransform transform)
    {
        var ltw  = transform.localToWorldMatrix;
        var rect = transform.rect;
        var p0   = ltw.MultiplyPoint(new Vector3(rect.x, rect.y, 0f));
        var p2   = ltw.MultiplyPoint(new Vector3(rect.xMax, rect.yMax, 0f));
        return new Rect(p0, p2 - p0);
    }

    /// <summary>Get screen dimensions rectangle of given rect transform with given inflate amount (in transform space, not in screen space)</summary>
    public static Rect GetScreenRect(this RectTransform transform, Vector2 inflateAmount)
    {
        var ltw  = transform.localToWorldMatrix;
        var rect = transform.rect.Inflate(inflateAmount.x, inflateAmount.y);
        var p0   = ltw.MultiplyPoint(new Vector3(rect.x, rect.y, 0f));
        var p2   = ltw.MultiplyPoint(new Vector3(rect.xMax, rect.yMax, 0f));
        return new Rect(p0, p2 - p0);
    }

    public static void CopyAnchoredRect(this RectTransform recttransform, RectTransform other)
    {
        recttransform.pivot = other.pivot;
        recttransform.anchorMin = other.anchorMin;
        recttransform.anchorMax = other.anchorMax;
        recttransform.offsetMin = other.offsetMin;
        recttransform.offsetMax = other.offsetMax;
        recttransform.anchoredPosition = other.anchoredPosition;
    }

    public static void SetRect(this RectTransform recttransform, Rect rect)
    {
        recttransform.position = rect.position;
        recttransform.sizeDelta = rect.size;
    }

    public static void SetRectLocal(this RectTransform recttransform, Rect rect)
    {
        recttransform.localPosition = rect.position;
        recttransform.sizeDelta = rect.size;
    }

    public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null)
            return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    public static void SetAnchorAndPosition(this RectTransform rectTransform, Vector2 anchor)
    {
        SetAnchorAndPosition(rectTransform, anchor, anchor);
    }

    public static void SetAnchorAndPosition(this RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector3.zero;
    }

    public static void SetAnchorModeAndPivot(this RectTransform rectTransform, AnchorMode mode, Vector2 pivot)
    {
        rectTransform.SetPivot(pivot);
        rectTransform.SetAnchorMode(mode);
    }

    public static void SetAnchorMode(this RectTransform rectTransform, AnchorMode mode)
    {
        switch (mode)
        {
            case AnchorMode.TopLeft:
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                break;
            case AnchorMode.TopCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                break;
            case AnchorMode.TopRight:
                rectTransform.anchorMin = new Vector2(1f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                break;
            case AnchorMode.MiddleLeft:
                rectTransform.anchorMin = new Vector2(0f, 0.5f);
                rectTransform.anchorMax = new Vector2(0f, 0.5f);
                break;
            case AnchorMode.MiddleCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            case AnchorMode.MiddleRight:
                rectTransform.anchorMin = new Vector2(1f, 0.5f);
                rectTransform.anchorMax = new Vector2(1f, 0.5f);
                break;
            case AnchorMode.BottomLeft:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0f, 0f);
                break;
            case AnchorMode.BottomCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0f);
                rectTransform.anchorMax = new Vector2(0.5f, 0f);
                break;
            case AnchorMode.BottomRight:
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                break;
            case AnchorMode.StretchLeft:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                break;
            case AnchorMode.StretchCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                break;
            case AnchorMode.StretchRight:
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                break;
            case AnchorMode.StretchTop:
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                break;
            case AnchorMode.StretchMiddle:
                rectTransform.anchorMin = new Vector2(0f, 0.5f);
                rectTransform.anchorMax = new Vector2(1f, 0.5f);
                break;
            case AnchorMode.StretchBottom:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                break;
            case AnchorMode.StretchStretch:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                break;
            default:
                break;
        }

    }

    public static void ClampToScreen(RectTransform obj, float maxOffScreenPercentage = 0f, float maxParentSizeRatio = 1f, float topMargin = 0f, float bottomMargin = -1f)
    {
        var worldToLocalMatrix = obj.worldToLocalMatrix;

        //getting screen corners in rect local space
        var bottomLeft = worldToLocalMatrix.MultiplyPoint3x4(new Vector3(0, 0));
        var topRight = worldToLocalMatrix.MultiplyPoint3x4(new Vector3(Screen.width, Screen.height));

        var boundRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);

        var pos = obj.anchoredPosition;
        var bounds = GetClampRect(obj, boundRect, maxOffScreenPercentage, maxParentSizeRatio, topMargin, bottomMargin).Move(pos.x, pos.y);

        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y);

        obj.anchoredPosition = pos;
    }

    public static Rect GetClampRect(RectTransform obj, Rect boundRect, float maxOffScreenPercentage = 0f, float maxParentSizeRatio = 1f, float topMargin = 0f, float bottomMargin = 0f)
    {
        var rect = obj.rect;

        var maxOffScreenWidth = rect.width * maxOffScreenPercentage;
        var maxOffScreenHeight = (bottomMargin != 0f) ? bottomMargin : (rect.height * maxOffScreenPercentage);

        obj.sizeDelta = new Vector2(Math.Min(obj.sizeDelta.x, boundRect.width * maxParentSizeRatio),
            Math.Min(obj.sizeDelta.y, boundRect.height * maxParentSizeRatio));

        var minPosition = boundRect.min - rect.min - new Vector2(maxOffScreenWidth, maxOffScreenHeight);
        var maxPosition = boundRect.max - rect.max + new Vector2(maxOffScreenWidth, -topMargin);

        return Rect.MinMaxRect(minPosition.x, minPosition.y, maxPosition.x, maxPosition.y);
    }

    public static void StretchToLine(this RectTransform t, Vector2 pointA, Vector2 pointB, int width)
    {
        Vector3 differenceVector = pointB - pointA;

        t.sizeDelta = new Vector2(differenceVector.magnitude, width);
        t.pivot = new Vector2(0, 0.5f);
        t.localPosition = pointA;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        t.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary> Sets slider bounds. Also if current setting of slider is not matching min max - updates that setting to nearest clamped value.
    /// By default its using notify to trigger side events of a slider </summary>
    public static void SetBoundsWithClamp(this Slider slider, float min, float max, float current, bool notify = true)
    {
        slider.minValue = min;
        slider.maxValue = max;

        if (current > max || current < min)
        {
            var clampedCurrent = current.Clamp(min, max);
            if (notify) slider.value = clampedCurrent;
            else slider.SetValueWithoutNotify(clampedCurrent);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValueNoNotify(this Slider instance, float value) => instance.SetValueWithoutNotify(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValueNoNotify(this Toggle instance, bool value) => instance.SetIsOnWithoutNotify(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValueNoNotify(this InputField instance, string value) => instance.SetTextWithoutNotify(value);

    public static bool IsPointInside(Transform transform, Vector3 position)
    {
        if (!transform.gameObject.activeSelf)
            return false;

        if (transform is RectTransform rect && rect.GetScreenRect().Contains(position))
            return true;

        for (var i = 0; i < transform.childCount; i++)
            if (IsPointInside(transform.GetChild(i), position))
                return true;

        return false;
    }

    public static void SmoothMove(this Slider slider, float from, float to, float time) => slider.StartCoroutine(SliderSmoothMove(slider, from, to, time));
    private static IEnumerator SliderSmoothMove(this Slider slider, float from, float to, float time)
    {
        var startingTime = Time.time;
        var amount = Mathf.Abs(to - from);
        var elapsed = 0f;

        while (elapsed < time)
        {
            elapsed = Mathf.Clamp(Time.time - startingTime, 0, time);
            slider.value = from + (elapsed / time) * amount;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public static void SetWidth(this RectTransform rect, float newWidth, bool updateLayoutElement = false)
    {
        var sizeDelta = rect.sizeDelta;
        sizeDelta.x = newWidth;
        rect.sizeDelta = sizeDelta;

        if (updateLayoutElement) rect.GetComponent<LayoutElement>().minWidth = newWidth;
    }

    public static void SetHeight(this RectTransform rect, float newHeight, bool updateLayoutElement = false)
    {
        var sizeDelta = rect.sizeDelta;
        sizeDelta.y = newHeight;
        rect.sizeDelta = sizeDelta;

        if (updateLayoutElement) rect.GetComponent<LayoutElement>().minHeight = newHeight;
    }

    public static void SetAnchoredX(this RectTransform rect, float newX)
    {
        var pos = rect.anchoredPosition;
        pos.x = newX;
        rect.anchoredPosition = pos;
    }

    public static void SetAnchoredY(this RectTransform rect, float newY)
    {
        var pos = rect.anchoredPosition;
        pos.y = newY;
        rect.anchoredPosition = pos;
    }    

    // find the element equal to value first, then find the element contains value
    public static T FindBestNameMatch<T>(this IEnumerable<T> source, Func<T, string> selector, string searchingValue) where T : Component
    {
        var components = source.ToArray();
        return components.FirstOrDefault(tValue => selector(tValue).Equals(searchingValue)) ?? 
               components.FirstOrDefault(tValue => selector(tValue).Contains(searchingValue));
    }

    /// <summary>unity doesnt update the canvas order correctly when it is an inactive GameObject since startup and is child of LayoutGroup
    /// showing it above all other UIs and ignoring canvas order</summary>
    public static void ForceCanvasOrderUpdate(this Canvas canvas, RectTransform uiToRebuild)
    {
        canvas.sortingOrder++;  // must change sort order to make it dirty
        canvas.sortingOrder--;  // and change back
        LayoutRebuilder.ForceRebuildLayoutImmediate(uiToRebuild); // force update
    }

    /// <summary>Set <see cref="Selectable.targetGraphic"/> color for any selectable except <see cref="Button"/>, and also set <see cref="Toggle.graphic"/> color of the selectable if it is a <see cref="Toggle"/>. In case of a <see cref="Button,"/>, button normal color will be changed.</summary>
    public static void SetGraphicColors(this Selectable selectable, UnityEngine.Color color)
    {
        if (selectable is Toggle toggle && toggle.graphic != null) toggle.graphic.color = color;

        if (selectable is Button button) button.SetNormalColor(color);
        else if (selectable.targetGraphic != null) selectable.targetGraphic.color = color;
    }

    /// <summary>Change the color for all supported UI Behaviours in this list.</summary>
    /// <remarks>Currently supports <see cref="Selectable"/> and <see cref="MaskableGraphic"/>.</remarks>
    public static void ColorAll(this IEnumerable<UIBehaviour> colorableUIBehaviours, UnityEngine.Color color)
    {
        foreach (var uiElement in colorableUIBehaviours)
        {
            if (uiElement == null) //Warn if an element is null, then skip it
            {
                Debug.LogWarning($"Null colorable UI element detected in a colorable UIBehaviour list.");
                continue;
            }

            //Set color for every supported UI element
            if (uiElement is Selectable selectable) selectable.SetGraphicColors(color);
            else if (uiElement is MaskableGraphic graphic) graphic.color = color;
            else Debug.LogWarning($"Unsupported UI element of type {uiElement.GetType()} found in a colorable UIBehaviour list. Consider adding support."); //Warn if type is not supported
        }
    }



    /// <summary>Set the <see cref="ColorBlock.normalColor"/> of a <see cref="Button"/>.</summary>
    public static void SetNormalColor(this Button button, UnityEngine.Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        button.colors = cb;
    }

    /// <summary>returns true if entry is visible inside rectMask otherwise returns false</summary>
    public static bool IsEntryVisible(this RectTransform entry, RectTransform rectMask, ScrollAxis Axis = ScrollAxis.Horizontal)
    {
        var entryRect = entry.GetScreenRect();
        var viewportRect = rectMask.GetScreenRect();

        if (Axis.HasFlag(ScrollAxis.Horizontal))
        {
            if (entryRect.xMax < viewportRect.xMin || entryRect.xMin > viewportRect.xMax)
                return false;
        }
        if (Axis.HasFlag(ScrollAxis.Vertical))
        {
            if (entryRect.yMax > viewportRect.yMin || entryRect.yMin < viewportRect.yMax)
                return false;
        }

        return true;
    }

    static List<RaycastResult> pointerOverResults = new(); //Helper list for not allocating when using the function below.
    /// <summary> Returns all UI GameObjects that are under the mouse, by visibility order. </summary>
    /// <remarks> Elements that are behind a UI that is under the mouse will also appear here. </remarks>
    public static IEnumerable<GameObject> GetUIElementsUnderMouse()
    {
        var pointerData = new PointerEventData(EventSystem.current) { pointerId = -1, position = Input.mousePosition };
        EventSystem.current.RaycastAll(pointerData, pointerOverResults);
        return pointerOverResults.Select(x => x.gameObject);
    }
}
public static class ScrollRectExtensions
{
    public static void ScrollToTop(this ScrollRect scrollRect)    => scrollRect.normalizedPosition = new Vector2(0, 1);
    public static void ScrollToBottom(this ScrollRect scrollRect) => scrollRect.normalizedPosition = new Vector2(0, 0);
    public static void ScrollToRight(this ScrollRect scrollRect)  => scrollRect.normalizedPosition = new Vector2(1, 0);
    public static void ScrollToLeft(this ScrollRect scrollRect)   => scrollRect.normalizedPosition = new Vector2(0, 0);
}

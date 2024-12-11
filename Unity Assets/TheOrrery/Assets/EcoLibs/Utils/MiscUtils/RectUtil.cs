// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections;
using Eco.Shared.Math;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Rect = UnityEngine.Rect;

public static class RectExtensions
{
    public static Rect Translate(this Rect rect, int x, int y)
    {
        return new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
    }
    public static bool ApproximatelyEquals(this Rect rect, Rect other, float errorMargin)
    {
        return Mathf.Abs(rect.xMin - other.xMin) < errorMargin
            && Mathf.Abs(rect.yMin - other.yMin) < errorMargin
            && Mathf.Abs(rect.xMax - other.xMax) < errorMargin
            && Mathf.Abs(rect.yMax - other.yMax) < errorMargin;
    }

    public static Rect Inflate(this Rect rect, float x, float y)
    {
        Rect result = new Rect();
        result.xMin = rect.xMin - x;
        result.xMax = rect.xMax + x;
        result.yMin = rect.yMin - y;
        result.yMax = rect.yMax + y;
        return result;
    }

    public static Vector2 DistanceTo(this Rect rect, Vector2 p)
    {
        float x = p.x < rect.xMin ? p.x - rect.xMin : (p.x > rect.xMax ? p.x - rect.xMax : 0f);
        float y = p.y < rect.yMin ? p.y - rect.yMin : (p.y > rect.yMax ? p.y - rect.yMax : 0f);
        return new Vector2(x, y);
    }
    
    public static Vector2 Clamp(this Rect rect, Vector2 p)
    {
        p.x = Mathf.Clamp(p.x, rect.xMin, rect.xMax);
        p.y = Mathf.Clamp(p.y, rect.yMin, rect.yMax);
        return p;
    }

    public static Rect Move(this Rect rect, float x, float y)
    {
        rect.x += x;
        rect.y += y;
        return rect;
    }

    public static Rect ContainInRect(this Rect rect, Rect container)
    {
        if (container.xMin > rect.xMin) rect = rect.Move(container.xMin - rect.xMin, 0);
        if (container.xMax < rect.xMax) rect = rect.Move(container.xMax - rect.xMax, 0);
        if (container.yMin > rect.yMin) rect = rect.Move(0,container.yMin - rect.yMin);
        if (container.yMax < rect.yMax) rect = rect.Move(0,container.yMax - rect.yMax);
        return rect;
    }

    public static Rect Intersection(this Rect rect, Rect other)
    {
        Rect intersection = new Rect();
        intersection.xMin = Mathf.Max(rect.xMin, other.xMin);
        intersection.yMin = Mathf.Max(rect.yMin, other.yMin);
        intersection.xMax = Mathf.Min(rect.xMax, other.xMax);
        intersection.yMax = Mathf.Min(rect.yMax, other.yMax);
        return intersection;
    }

    public static Rect NewFromPoints(Vector2 p1, Vector2 p2)
    {
        var topLeft     = new Vector2(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y));
        var bottomRight = new Vector2(Mathf.Max(p1.x, p2.x), Mathf.Max(p1.y, p2.y));
        return new Rect(topLeft, bottomRight - topLeft);
    }

    /// <summary>
    /// When Pivot of RectTransform is changed, the position is changed too (even if visible place of rect is the same). 
    /// This function will return values like they would be with (0;0) pivot, so for same visible place it will be the same.
    /// </summary>
    /// <param name="rectTransform"></param>
    public static Vector2 GetRectTransformPositionIgnoringPivot(this RectTransform rectTransform, UnityEngine.Vector2 position)
    {
        Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
        size.Scale(rectTransform.pivot);
        return position + size;
    }

    public static Rect ToScreenSpace(this RectTransform transform)
    {
         Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
         Rect rect = new Rect(transform.position, size);
         rect.x -= (transform.pivot.x * size.x);
         rect.y -= (transform.pivot.y * size.y);
         return rect;
    }
    
    public static Rect Side(this Rect rect, Vector2i dir)
    {
        var topOrLeft   = dir == Vector2i.Left || dir == Vector2i.Up;
        var topOrRight  = dir == Vector2i.Right || dir == Vector2i.Up;
        var start       = topOrLeft  ? new Vector2(rect.xMin, rect.yMin)  : new Vector2(rect.xMax, rect.yMax);
        var end         = topOrRight ? new Vector2(rect.xMax, rect.yMin) : new Vector2(rect.xMin, rect.yMax);
        end.x -= dir.x;
        end.y += dir.y; //For the funky texture system, it inverts y in rects.

        return RectExtensions.NewFromPoints(start, end);
    }


    public static Vector2 EdgePoint(this Rect rect, HorzDir dir)
    {
        switch (dir)
        {
            case HorzDir.West:      return LeftMiddle(rect);
            case HorzDir.East:      return RightMiddle(rect);
            case HorzDir.North:     return TopMiddle(rect);
            case HorzDir.South:     return BottomMiddle(rect);

            case HorzDir.Northwest: return TopLeft(rect);
            case HorzDir.Northeast: return TopRight(rect);
            case HorzDir.Southwest: return BottomLeft(rect);
            case HorzDir.Southeast: return BottomRight(rect);
        }
        return Vector2.zero;
    }

    public static Vector2 EdgeMidpoint(this Rect rect, Direction2D dir)     
    {
        switch (dir)
        { 
            case Direction2D.Left:    return LeftMiddle(rect); 
            case Direction2D.Right:   return RightMiddle(rect); 
            case Direction2D.Up:      return TopMiddle(rect); 
            case Direction2D.Down:    return BottomMiddle(rect); 
        }
        return Vector2.zero;
    }
    
    public static Vector2 TopMiddle(this Rect rect)     { return new Vector2((rect.xMin + rect.xMax) / 2, rect.yMax); }
    public static Vector2 BottomMiddle(this Rect rect)  { return new Vector2((rect.xMin + rect.xMax) / 2, rect.yMin); }
    public static Vector2 LeftMiddle(this Rect rect)    { return new Vector2(rect.xMin, (rect.yMin + rect.yMax) / 2); }
    public static Vector2 RightMiddle(this Rect rect)   { return new Vector2(rect.xMax, (rect.yMin + rect.yMax) / 2); }
    public static Vector2 TopLeft(this Rect rect)       { return new Vector2(rect.xMin, rect.yMax); }
    public static Vector2 TopRight(this Rect rect)      { return new Vector2(rect.xMax, rect.yMax); }
    public static Vector2 BottomLeft(this Rect rect)    { return new Vector2(rect.xMin, rect.yMin); }
    public static Vector2 BottomRight(this Rect rect)   { return new Vector2(rect.xMax, rect.yMin); }

    public static float Area(this Rect rect) { return rect.width * rect.height; }

    public static Vector2 GetPositionInRect(this Rect rect, Vector2 percent)
    {
        return new Vector2( Mathf.Lerp(rect.position.x, rect.position.x + rect.size.x, percent.x), 
                            Mathf.Lerp(rect.position.y, rect.position.y + rect.size.y, percent.y));
    }

    public static bool ContainsRect(this Rect rect, Rect other) => rect.Contains(other.TopLeft()) && rect.Contains(other.BottomRight());

    public static void ScrollTo(this ScrollRect scrollRect, RectTransform target)
    {
        scrollRect.normalizedPosition = GetTargetScrollPosition(scrollRect, target);
    }

    public static void ScrollTo(this ScrollRect scrollRect, RectTransform target, float percentPerSecond)
    {
        Vector2 startPos = scrollRect.normalizedPosition;
        Vector2 targetPos = GetTargetScrollPosition(scrollRect, target);

        if (!startPos.Equals(targetPos))
        {
            float duration = Mathf.Max(Mathf.Abs(targetPos.x - startPos.x), Mathf.Abs(targetPos.y - startPos.y)) / percentPerSecond;
            scrollRect.StartCoroutine(ScrollToOverTime(scrollRect, startPos, targetPos, Time.time, duration));
        }
    }

    private static IEnumerator ScrollToOverTime(ScrollRect scrollRect, Vector2 startPos, Vector2 targetPos, float startTime, float duration)
    {
        while (true)
        {
            float percent = Mathf.Clamp01(Mathf.Sin(Mathf.PI * (Time.time - startTime) / (2 * duration)));
            scrollRect.horizontalNormalizedPosition = (targetPos.x - startPos.x) * percent + startPos.x;
            scrollRect.verticalNormalizedPosition = (targetPos.y - startPos.y) * percent + startPos.y;

            if (Time.time >= startTime + duration)
            {
                scrollRect.normalizedPosition = targetPos;
                yield break;
            }
            else
                yield return null;
        }
    }

    private static Vector2 GetTargetScrollPosition(ScrollRect scrollRect, RectTransform target)
    {
        Rect selectionRect = target.GetScreenRect();
        Rect scrollRectRect = ((RectTransform)scrollRect.transform).GetScreenRect();
        Rect contentRect = scrollRect.content.GetScreenRect();

        // Horizontal Scroll
        float x;
        if (scrollRect.horizontal)
        {
            float widthRightOfSelection = selectionRect.xMin - contentRect.xMin;
            float widthLeftOfSelection = contentRect.xMax - selectionRect.xMax;

            float maxXScrollFactor = widthRightOfSelection;
            float minXScrollFactor = contentRect.width - scrollRectRect.width - widthLeftOfSelection;
            float halfwayXScrollFactor = ((maxXScrollFactor - minXScrollFactor) / 2) + minXScrollFactor;

            x = Mathf.Clamp01(halfwayXScrollFactor / (contentRect.width - scrollRectRect.width));
        }
        else
            x = scrollRect.normalizedPosition.x;

        // Vertical Scroll
        float y;
        if (scrollRect.vertical)
        {
            float heightAboveSelection = selectionRect.yMin - contentRect.yMin;
            float heightBelowSelection = contentRect.yMax - selectionRect.yMax;

            float maxYScrollFactor = heightAboveSelection;
            float minYScrollFactor = contentRect.height - scrollRectRect.height - heightBelowSelection;
            float halfwayYScrollFactor = ((maxYScrollFactor - minYScrollFactor) / 2) + minYScrollFactor;

            y = Mathf.Clamp01(halfwayYScrollFactor / (contentRect.height - scrollRectRect.height));
        }
        else
            y = scrollRect.normalizedPosition.y;

        return new Vector2(x, y);
    }
}
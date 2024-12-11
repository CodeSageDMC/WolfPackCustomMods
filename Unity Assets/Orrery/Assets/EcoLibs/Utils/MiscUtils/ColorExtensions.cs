// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public static class ColorExtensions
{
    public static string ToHex(this Color c)
    {
        // it is absolutely bizarre that unity doesn't have a way to get the integer color, and uint.tostring("X") doesn't work either
        return ((int)(c.r * 255)).ToString("X2") + ((int)(c.g * 255)).ToString("X2") + ((int)(c.b * 255)).ToString("X2");
    }
    
    public static Color FromUInt(uint color)
    {
        float r = ((color & 0xff000000) >> 24) / 255.0f;
        float g = ((color & 0x00ff0000) >> 16) / 255.0f;
        float b = ((color & 0x0000ff00) >> 8) / 255.0f;
        float a = (color & 0x000000ff) / 255.0f;

        return new Color(r, g, b, a);
    }

    public static uint ToUInt(this Color c)
    {
        return 
            ((uint)Mathf.Clamp(c.r * 255, 0, 255) << 24) |
            ((uint)Mathf.Clamp(c.g * 255, 0, 255) << 16) |
            ((uint)Mathf.Clamp(c.b * 255, 0, 255) << 8) |
            ((uint)Mathf.Clamp(c.a * 255, 0, 255));
    }

    // order is AARRGGBB to match System.Drawing.Color 
    public static int ToInt(this Color c)
    {
        return unchecked ((int) (
            ((uint)Mathf.Clamp(c.a * 255, 0, 255) << 24) |
            ((uint)Mathf.Clamp(c.r * 255, 0, 255) << 16) |
            ((uint)Mathf.Clamp(c.g * 255, 0, 255) << 8) |
            ((uint)Mathf.Clamp(c.b * 255, 0, 255))));
    }

    public static Color WithAlpha(this Color c, float alpha)
    {
        c.a = alpha;
        return c;
    }

}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using UnityEngine;
    using Color = UnityEngine.Color;
    using Eco.Shared.Graphics;
    using Eco.Shared.Utils;
    using Unity.Mathematics;

    public static class MoreColors
    {
        public static readonly Color darkBlue    = Color.blue * .5f;
        public static readonly Color darkGreen   = Color.green * .5f;
        public static readonly Color dargMagenta = Color.magenta * .5f;
        public static readonly Color darkRed     = Color.red * .5f;
        public static readonly Color darkYellow  = Color.yellow * .5f;
        public static readonly Color darkCyan    = Color.cyan * .5f;

        public static readonly Color brown       = new Color32(173, 69, 0, 255);
        public static readonly Color lightbrown  = new Color32(255, 229, 175, 255);
        public static readonly Color lightBlue   = new Color32(124, 200, 255, 255);
        public static readonly Color lightYellow = new Color32(255, 249, 142, 255);
        public static readonly Color lightGreen  = new Color32(100, 255, 100, 255);
        public static readonly Color lightRed    = new Color32(255, 128, 128, 255);

        //Needed because Unity's Color.Yellow isnt full 255
        public static readonly Color trueYellow = new Color32(255, 255, 0, 255);

        // Colorblind friendly colors
        public static readonly Color niceGreen   = new Color32(100, 200, 50, 255);
        public static readonly Color niceYellow  = new Color32(255, 240, 0, 255);
    }

    public static class ColorUtils
    {
        public static Color[] BrightColorList = new Color[] { Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow };
        public static Color RandomBrightColor(int seed)
        {
            //Random blend between two different random colors.
            var rand = new System.Random(seed);

            var color1 = BrightColorList[(int)(rand.NextDouble() * BrightColorList.Length)];
            Color color2;
            do color2 = BrightColorList[(int)(rand.NextDouble() * BrightColorList.Length)];
            while (color1 != color2);

            return Color.Lerp(color1, color2, (float)rand.NextDouble());
        }

        public static Color Fade(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        public static Color Blend(this Color color1, Color color2, float percent)
        {
            var other = 1 - percent;
            return color1 * percent + color2 * other;
        }

        public static Color ToColor(int hexVal)
        {
            var R = (byte)((hexVal >> 16) & 0xFF);
            var G = (byte)((hexVal >> 8) & 0xFF);
            var B = (byte)((hexVal) & 0xFF);
            return new Color32(R, G, B, 255);
        }

        public static string Text(this Color color, string s) {return Eco.Shared.Utils.Text.Color(color.ToHex(), s); } 
        public static string Num(this Color color, float v)   {return Eco.Shared.Utils.Text.Color(color.ToHex(), v.ToString("0.#")); } 
        public static string Num(this Color color, int v)     {return Eco.Shared.Utils.Text.Color(color.ToHex(), v.ToString()); }

        public static Gradient ToGradient(this Color color)  {return new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(color, 0.0f)} }; }

        /// <summary>Pack int into four 8bit channels of color (ARGB32). Used to pass into shader, for example.</summary>
        public static Color PackIntToColor(this int num)
        {
            var result = new Color();

            result.b = ((num)       & 0xFF) / 255f;
            result.g = ((num >> 8)  & 0xFF) / 255f;
            result.r = ((num >> 16) & 0xFF) / 255f;
            result.a = ((num >> 24) & 0xFF) / 255f;

            return result;
        }

        /// <summary>Unpack ARGB32-packed number into int. Used to get output from shader as COLOR.</summary>
        public static int UnpackInt(this Color packedInt)
        {
            int result = 0;

            result |= (Mathf.RoundToInt(packedInt.b * 255f));
            result |= (Mathf.RoundToInt(packedInt.g * 255f)) << 8;
            result |= (Mathf.RoundToInt(packedInt.r * 255f)) << 16;
            result |= (Mathf.RoundToInt(packedInt.a * 255f)) << 24;

            return result;
        }

        /// <summary> Converts Eco.Shared.Utils.Color to float4 representation. </summary>
        public static float4 ConvertF(this ByteColor c) => new(c.R/255f, c.G/255f, c.B/255f, c.A/255f);
        public static ByteColor Convert(this float4 c) => ByteColor.Get(c.x, c.y, c.z, c.w);
        public static ByteColor ConvertU(this Color c) => ByteColor.Get(c.r, c.g, c.b, c.a);
        public static Color ConvertU(this ByteColor c) => new (c.R/255f, c.G/255f, c.B/255f, c.A/255f);
    }
}

// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

/// <summary>
/// <para>Container object for passing font assets from mod client bundles to the Eco client for use in the rich text tag parser system.</para>
/// <para>See the Eco wiki page available <a href="https://wiki.play.eco/en/Custom_Fonts">here</a> for more info.</para>
/// </summary>
public class FontContainer : TrackableBehavior
{
    public Font[] RegisteredFonts;
}

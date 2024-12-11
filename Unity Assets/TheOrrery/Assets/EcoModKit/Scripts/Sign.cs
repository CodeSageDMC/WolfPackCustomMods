// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using TMPro;

/// <summary>Client side component for consuming the data from a CustomTextComponent to create custom sign <seealso cref="WorldObject"/>s using TextMeshPro.</summary>
/// <inheritdoc/>
public partial class Sign : WorldObject
{
    /// <summary><seealso cref="TextMeshPro"/> instance to contain the text data from the CustomTextComponent.</summary>
    public TextMeshPro SignText;
    public TextMeshPro[] ExtraSignTexts;

    ///<summary>Sign prefabs might have different vertical alignment set which has inconsistent behavior with rich text, this makes all signs behave same way</summary>
    void Start() 
    {
        this.SignText.verticalAlignment = VerticalAlignmentOptions.Middle;
        foreach (var text in ExtraSignTexts)
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
    }
}

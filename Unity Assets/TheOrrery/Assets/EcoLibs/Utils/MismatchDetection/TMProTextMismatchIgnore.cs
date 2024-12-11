// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.TMPro
{
    using System;

    /// <summary>
    /// Special marker attribute which may be added to inform <see cref="TMPTextMismatchDetector"/> to ignore text mismatches for referenced component.
    /// It only works if TMP_Text added to same game object or nested within the game object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TMProTextMismatchIgnoreAttribute : Attribute { }
}
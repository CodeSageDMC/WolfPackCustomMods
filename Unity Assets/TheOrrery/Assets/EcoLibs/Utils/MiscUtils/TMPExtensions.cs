// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using System;
    using System.Text;
    using TMPro;
    using UnityEngine;

    public static class TMPExtensions
    {
        private static readonly StringBuilder TmpStringBuilder = new StringBuilder();

        public static TMP_LinkInfo? GetIntersectingLink(this TMP_Text text, Vector3 position, Camera linkCamera)
        {
            if (string.IsNullOrEmpty(text.text)) return null;

            var rectTransform = text.rectTransform;

            //Convert position into Worldspace coordinates
            TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, linkCamera, out position);

            //We go through all the links that are inside the current text and check whether the current position is inside one of them, and if that's the case we return that link.
            var linkInfoLength = text.textInfo.linkInfo.Length;
            for (int i = 0; i < linkInfoLength; i++)
            {
                var linkInfo = text.textInfo.linkInfo[i];     //Get current link info.
                if (linkInfo.textComponent == null) continue; //If the link doesn't have a text component we ignore it.

                var lastIndexInLink = linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength - 1; //The last character index in the current link.

                if (text.firstVisibleCharacter > lastIndexInLink) continue; //We ignore any link that's currently not visible. This very important for texts that uses linked text as an overflow method.

                int currentLineIndex = Array.FindIndex(text.textInfo.lineInfo, x => x.lastCharacterIndex >= linkInfo.linkTextfirstCharacterIndex && x.firstCharacterIndex <= linkInfo.linkTextfirstCharacterIndex); //We get current line index.
                if (currentLineIndex < 0 || currentLineIndex >= text.textInfo.lineInfo.Length) continue; //we ignore this link if we can't find corresponding line
                var lastIndexInCurrentLine = 0;

                //If part of link was truncated linkTextLength will be set to 0, so in that case we use end of line instead
                if (lastIndexInLink <= 0) lastIndexInLink = text.textInfo.lineInfo[currentLineIndex].lastCharacterIndex;

                var currentCharacterIndex = linkInfo.linkTextfirstCharacterIndex; //We initialize the index with the index of the first character in the link.
                if (currentCharacterIndex < 0 || currentCharacterIndex >= text.textInfo.characterInfo.Length) continue;    //If its for some reason a bad index, skip next step
                
                //The idea here is to check whether the link is a single line or a multi-lines link and if it's the latter, we check every line that still have that link.
                //Note : You are maybe thinking "Why not just take the first and last characters of the link and then just check it once" and the reason is multi-lines links (I let you think that through :) ).
                do
                {
                    lastIndexInCurrentLine = text.textInfo.lineInfo[currentLineIndex].lastCharacterIndex; //We get the last character in the current line.

                    var firstCharacter          = text.textInfo.characterInfo[currentCharacterIndex]; //We get the first character in the link or in the line.
                    
                    //If the last character in the link is on the same line as the first we get it else we get the last character in the current line.
                    var lastIndex = Mathf.Min(lastIndexInLink, lastIndexInCurrentLine);
                    var lastLinkCharacterInLine = text.textInfo.characterInfo[lastIndex];

                    //We calculate topY and bottomY coordinates for text rect, we compare topY coordinates and bottomY coordinates for each character to find the highest and lowest y coordinate for the entire text
                    var topY    = text.textInfo.lineInfo[currentLineIndex].lineExtents.max.y;
                    var bottomY = text.textInfo.lineInfo[currentLineIndex].lineExtents.min.y;

                    //We get the Bottom Left/right of the first character and the Top Left/Right of the second character positions so that we can check if the current position is between them.
                    Vector3 bl = rectTransform.TransformPoint(new Vector3(firstCharacter.bottomLeft.x, bottomY, 0));
                    Vector3 tl = rectTransform.TransformPoint(new Vector3(firstCharacter.bottomLeft.x, topY, 0));
                    Vector3 tr = rectTransform.TransformPoint(new Vector3(lastLinkCharacterInLine.topRight.x, topY, 0));
                    Vector3 br = rectTransform.TransformPoint(new Vector3(lastLinkCharacterInLine.topRight.x, bottomY, 0));

                    if (PointIntersectRectangle(position, bl, tl, tr, br)) return linkInfo;

                    currentLineIndex++;                                  //We go to the next line.
                    currentCharacterIndex  = lastIndexInCurrentLine + 1; //We get the first character index on the next line.

                } while (currentCharacterIndex < lastIndexInLink);
            }
            return null;
        }

        /// <summary>
        /// Fix for <see cref="TMP_TextUtilities.FindIntersectingCharacter"/>
        /// The original method won't work for TMP_Text with linked text, unless visibleOnly is true, but then spaces will be ignored
        /// </summary>
        public static int FindIntersectingCharacter(TMP_Text text, Vector3 position, Camera camera, bool visibleOnly)
        {
            var rectTransform = text.rectTransform;

            // Convert position into Worldspace coordinates
            TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);

            for (int i = 0; i < text.textInfo.characterCount; i++)
            {
                var cInfo = text.textInfo.characterInfo[i]; // Get current character info.
                var isWhiteSpace = char.IsWhiteSpace(cInfo.character);
                // there is no distinction between not visible char and space, so count it as a visible character
                // and check bounderies, if top left and bot right are the same, space is not visible
                if (visibleOnly && !cInfo.isVisible && (!isWhiteSpace || cInfo.topLeft == cInfo.bottomRight)) continue;

                // Get Bottom Left and Top Right position of the current character
                Vector3 bl = rectTransform.TransformPoint(cInfo.bottomLeft);
                Vector3 tl = rectTransform.TransformPoint(new Vector3(cInfo.bottomLeft.x, cInfo.topRight.y, 0));
                Vector3 tr = rectTransform.TransformPoint(cInfo.topRight);
                Vector3 br = rectTransform.TransformPoint(new Vector3(cInfo.topRight.x, cInfo.bottomLeft.y, 0));

                if (PointIntersectRectangle(position, bl, tl, tr, br)) return i;
            }
            return -1;
        }

        /// <summary> Function to check if a Point is contained within a Rectangle.
        /// Internal dependency from <see cref="TMP_TextUtilities.FindIntersectingCharacter"/> </summary>
        public static bool PointIntersectRectangle(Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 ab = b - a;
            Vector3 am = m - a;
            Vector3 bc = c - b;
            Vector3 bm = m - b;

            float abamDot = Vector3.Dot(ab, am);
            float bcbmDot = Vector3.Dot(bc, bm);

            return 0 <= abamDot && abamDot <= Vector3.Dot(ab, ab) && 0 <= bcbmDot && bcbmDot <= Vector3.Dot(bc, bc);
        }

        private static readonly string linkTag = "<link";
        /// <summary>
        /// This is faster than generate textinfo, and text.textinfo is not initializated immediatly after text change
        /// </summary>
        public static bool ContainsLinks(this TMP_Text text) => !string.IsNullOrEmpty(text.text) && text.text.Contains(linkTag);
        public static void SetSubstring(this TMP_Text text, string value, int startIndex)
        {
            TmpStringBuilder.Clear();
            TmpStringBuilder.Append(value, startIndex, value.Length - startIndex);
            text.SetText(TmpStringBuilder);
        }

        /// <summary>
        /// Only get the rect for characters that are on the current line.
        /// This avoids a rect that includes text outside of the link.
        /// </summary>
        public static Rect GetRect(this TMP_LinkInfo link, Vector3 position, Camera linkCamera)
        {
            // Only get the rect for characters that are on the current line.
            // This avoids a rect that goes over the bounds of the link.
            int currentCharIndex = TMP_TextUtilities.FindNearestCharacter(link.textComponent, position, linkCamera, true);
            var wordBounds = GetTextWordBounds(link, currentCharIndex, false, false);
            return wordBounds;
        }

        /// <summary>Gets the full covering rect for the given link.</summary>
        public static Rect GetRect(this TMP_LinkInfo link) => GetTextWordBounds(link, link.linkTextfirstCharacterIndex, true, true);

        /// <summary>Calculates the Word rect bounds for the given link and target first character index, the resulting rect can be in local space for the result inside the text component or world space for tooltips.</summary>
        private static Rect GetTextWordBounds(TMP_LinkInfo link, int currentCharIndex, bool useAllLines, bool localSpace)
        {
            // Get First Character info and bind the wordbounds to it.
            TMP_CharacterInfo currentCharInfo = link.textComponent.textInfo.characterInfo[currentCharIndex];
            int currentLineNumber = currentCharInfo.lineNumber;
            Rect wordBounds = new Rect();
            wordBounds.min = currentCharInfo.bottomLeft;
            wordBounds.max = currentCharInfo.topRight;

            //When TMP link is partially truncated linkTextLength will return 0 so we need to look for index of text last
            //character in characterInfo, when text is truncated the last character is End of Text special character (ord 3)
            var lastCharacterIndex = link.linkTextLength;
            if (lastCharacterIndex == 0)
                lastCharacterIndex = Array.FindIndex(link.textComponent.textInfo.characterInfo, x => x.character == (char)3);
            // Iterate through characters and widen word bound based on its x/y more info on rects: https://docs.unity3d.com/ScriptReference/Rect.html
            for (int j = 0; j < lastCharacterIndex; j++)
            {
                int characterIndex = link.linkTextfirstCharacterIndex + j;
                TMP_CharacterInfo charInfo = link.textComponent.textInfo.characterInfo[characterIndex];
                if (charInfo.lineNumber == currentLineNumber || useAllLines)
                {
                    wordBounds.xMin = Mathf.Min(wordBounds.xMin, charInfo.bottomLeft.x);
                    wordBounds.yMin = Mathf.Min(wordBounds.yMin, charInfo.bottomLeft.y);
                    wordBounds.xMax = Mathf.Max(wordBounds.xMax, charInfo.topRight.x);
                    wordBounds.yMax = Mathf.Max(wordBounds.yMax, charInfo.topRight.y);
                }
            }

            if(!localSpace)
            {
                wordBounds.min = link.textComponent.rectTransform.TransformPoint(wordBounds.min).XY();
                wordBounds.max = link.textComponent.rectTransform.TransformPoint(wordBounds.max).XY();
            }

            return wordBounds;
        }

        public static void CopyFontSettingsFrom(this TMP_Text text, TMP_Text other)
        {
            text.font = other.font;
            text.fontSize = other.fontSize;
            text.fontSizeMin = other.fontSizeMin;
            text.fontSizeMax = other.fontSizeMax;
            text.alignment = other.alignment;
            text.fontStyle = other.fontStyle;
        }

        /// <summary>Force rebuild text and build all submeshes</summary>
        public static void RebuildText(this TMP_Text text)
        {
            // HACK: we actually need to call ParseInputText but it's private. Thus, use the only public method available:
            text.GetTextInfo(text.text);
        }
    }
}

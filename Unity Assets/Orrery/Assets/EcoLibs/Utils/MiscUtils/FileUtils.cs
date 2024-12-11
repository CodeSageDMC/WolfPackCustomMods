// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.Utils;
using System.IO;

public static class FileUtils
{
    public static bool IsValidFilename(string filename) 
    { 
        return filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
    }

    public static string CreateCopiedFileName(string filePathAndName)
    { 
        var filename = Path.GetFileNameWithoutExtension(filePathAndName);

        // Extract the num, increment, and re-add
        var digits  = new[] { ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var trimmed = filename.TrimEnd(digits);
        var num     = filename.Substring(trimmed.Length).ToInt();

        num++;
        trimmed += " " + num.ToString() + Path.GetExtension(filePathAndName);
        var newName = Path.Combine(Path.GetDirectoryName(filePathAndName), trimmed);
        return newName;
    }

    // Copy to a newly named file
    public static string CopyFile(string filePathAndName)
    {
        var newName = GetUniqueName(filePathAndName);
        File.Copy(filePathAndName, newName);
        return newName;
    }

    public static string GetUniqueName(string oldName)
    { 
        string newName = oldName;
        while (File.Exists(newName))
            newName = CreateCopiedFileName(newName);

        return newName;
    }
}
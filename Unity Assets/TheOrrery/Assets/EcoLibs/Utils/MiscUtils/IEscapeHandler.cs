// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.


/// <summary>
/// Use this interface to add additional actions to ESC button for UIManager
/// </summary>
public interface IEscapeHandler
{
    //return true to consume escape
    bool HandleEscape();
}

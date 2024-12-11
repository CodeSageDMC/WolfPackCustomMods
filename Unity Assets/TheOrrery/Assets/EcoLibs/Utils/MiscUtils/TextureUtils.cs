// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;


public static class TextureUtils
{
    ///<summary>Creates a Texture2D from provided byte array</summary>
    public static Texture2D CreateTextureFromBytes(byte[] textureBytes)
    {
        Texture2D texture = new Texture2D(1, 1);  //Actual Resolution of this texture is set from byte array by LoadImage
        texture.LoadImage(textureBytes);
        texture.Apply();
        return texture;
    }

    ///<summary>Creates a sprite from provided byte array. Used when displaying texture on the UI is needed </summary>
    public static Sprite CreateSpriteFromBytes(byte[] textureBytes)
    {
        Texture2D texture = CreateTextureFromBytes(textureBytes);
        texture.Compress(false); //Texture data is JPG, the texture will be RGA and compressed to DXT1.
        return texture.CreateSpriteFromTexture();
    }

    /// <summary>Renders material into texture using GPU</summary>
    public static Texture2D RenderMaterialIntoTexture(Material material, Vector2Int resolution, bool mipmap = false)
    {
        //Create renderTexture and copy material texture into renderTexture
        RenderTexture renderTexture = RenderTexture.GetTemporary(resolution.x, resolution.y);
        Graphics.Blit(null, renderTexture, material);

        //Create empty texture with custom resultion
        Texture2D texture = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, mipmap);
        texture.filterMode = mipmap ? FilterMode.Trilinear : FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        //Read pixel data from renderTexture
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(Vector2.zero, resolution), 0, 0);
        RenderTexture.active = null;
        
        //Clear temporary data and apply into new texture
        RenderTexture.ReleaseTemporary(renderTexture);
        texture.Apply(true, false);
        return texture;
    }

    /// <summary> Fits the source RectTransform to its parent RectTransform while maintaining the specified aspect ratio. </summary>
    public static void FitParent(this RectTransform source, float aspectRatio) => FitRectTransform(source, source.parent.GetComponent<RectTransform>(), aspectRatio);
    /// <summary> Fits the source RectTransform to the provided reference RectTransform while maintaining the specified aspect ratio. </summary>
    public static void FitTransform(this RectTransform source, RectTransform transformToFit, float aspectRatio) => FitRectTransform(source, transformToFit, aspectRatio);


    /// <summary> Fits the source RectTransform to the target RectTransform while maintaining the specified aspect ratio. </summary>
    public static void FitRectTransform(this RectTransform source, RectTransform target, float sourceAspectRatio) 
    {
        float targetAspectRatio = target.rect.width / target.rect.height;
        float w, h;

        if (sourceAspectRatio >= targetAspectRatio) 
        {
            w = target.rect.width; //Keep same width and recalculate new height
            h = Mathf.RoundToInt(w * (1f / sourceAspectRatio));
        }
        else 
        {
            h = target.rect.height;
            w = Mathf.RoundToInt(h * sourceAspectRatio);
        }
        source.sizeDelta = new Vector2(w, h);
    }

}

public static class TextureExtensions
{
    /// <summary>Overwrites everything with black pixels</summary>
    public static void Clear(this Texture2D texture) => Clear(texture, Color.black);

    /// <summary>Overwrites every pixel with specific color</summary>
    public static void Clear(this Texture2D texture, Color c)
    {
        var pixels = texture.GetPixels();
        for (var i = 0; i < pixels.Length; ++i)
            pixels[i] = c;

        texture.SetPixels(pixels);
        texture.Apply();
    }

    ///<summary>Creates a sprite from provided texture, used when displaying texture on the UI is needed</summary>
    public static Sprite CreateSpriteFromTexture(this Texture2D texture) => CreateSpriteFromTexture(texture, new Rect(0, 0, texture.width, texture.height));
    ///<summary>Creates a sprite from provided texture, used when displaying texture on the UI is needed</summary>
    public static Sprite CreateSpriteFromTexture(this Texture2D texture, Rect rect) => Sprite.Create(texture, rect, Vector2.zero, 100f, 0u, SpriteMeshType.FullRect);


    /// <summary>Returns resized texture. Resize calculation is done on GPU</summary>
    /// <param name="originalTexture"></param>
    /// <param name="width">new target width</param>
    /// <param name="height">new target height</param>
    /// <param name="shaderName">Pass custom shader name. Otherwise it will be rendered on Unlit/Texture shader</param>
    /// <param name="texturePropertyName">Pass custom texture property name</param>
    public static Texture2D ResizeTexture(this Texture2D originalTexture, int width, int height, string shaderName = "Unlit/Texture", string texturePropertyName = "_MainTex", bool mipmap = false)
    {
        var shader = Shader.Find(shaderName);
        if (shader == null)
        {
            Debug.LogError($"Shader with name '{shaderName}' doesn't exist");
            return null;
        }

        Material material = new Material(shader);
        if (!material.HasProperty(texturePropertyName))
        {
            Debug.LogError($"Shader doesn't have {texturePropertyName} property to set texture");
            return null;
        }
        material.SetTexture(texturePropertyName, originalTexture);

        return TextureUtils.RenderMaterialIntoTexture(material, new Vector2Int(width, height), mipmap);
    }

    /// <summary> Returns the dimensions of a texture (width, height). </summary>
    public static Vector2Int Dimensions(this Texture texture) => new Vector2Int(texture.width, texture.height);

    /// <summary>/ Resizes a source texture to fill an output rectangle while preserving the selected aspect ratio. </summary>
    /// <param name="source">The input texture to resize.</param>
    /// <param name="aspectRatio">The desired aspect ratio of the output rectangle.</param>
    /// <returns>A resized texture that fills the output rectangle while preserving the selected aspect ratio.</returns>
    public static Texture2D EnlargeToFill(this Texture2D source, float aspectRatio, float offset = 0.5f)
    {
        int sourceW = source.width, sourceH = source.height;
        int outputW, outputH; //Output width and height.
        int offsetX = 0, offsetY = 0; //Offset required to define the start point of the ROI (Region Of Interest)
        float sourceAR = (float)sourceW / sourceH, outputAR = aspectRatio; //Calculate source aspect ratio and rename output aspect ratio (just for symmetry)

        if (sourceAR < outputAR)
        {
            outputW = sourceW; //Keep same width and recalculate new height
            outputH = Mathf.RoundToInt(outputW * (1f / outputAR));
            offsetY = Mathf.RoundToInt(Mathf.Lerp(0, sourceH - outputH, offset));
        } else 
        {
            outputH = sourceH;
            outputW = Mathf.RoundToInt(outputH * outputAR);
            offsetX = Mathf.RoundToInt(Mathf.Lerp(0, sourceW - outputW, offset));
        }

        Color32[] sourcePixels = source.GetPixels32(); //Get the pixel data from the source texture
        Color32[] roiPixels = new Color32[outputW * outputH];
            
        //Create a new texture to hold the ROI (Region Of Interest)
        Texture2D output = new (outputW, outputH, TextureFormat.RGBA32, false);

        // copy the pixels in the ROI to the new array
        for (int y = 0; y < outputH; y++)
        {
            for (int x = 0; x < outputW; x++)
            {
                int sourceIndex = (y + offsetY) * sourceW + (x + offsetX);
                int destIndex = y * outputW + x;
                roiPixels[destIndex] = sourcePixels[sourceIndex];
            }
        }

        // apply the ROI pixels to the new texture
        output.SetPixels32(roiPixels);
        output.Apply();
        return output;
    }

    /// <summary> Resizes a given texture to fit within a specified aspect ratio while maintaining its original aspect ratio.
    /// If the input texture's aspect ratio is larger than the specified aspect ratio, color (selected in the parameter) bars
    /// will be added to the top and bottom of the output texture. If the input texture's aspect ratio is smaller than the
    /// specified aspect ratio, color bars will be added to the left and right of the output texture. </summary>
    /// <param name="source">The input texture to resize.</param>
    /// <param name="aspectRatio">The aspect ratio to fit the input texture within.</param>
    /// <param name="color">The color of the blank bars added to the output texture.</param>
    /// <returns>A new texture with the resized image.</returns>
    public static Texture2D ShrinkToFit(this Texture2D source, float aspectRatio, Color color)
    {
        int sourceW = source.width, sourceH = source.height;
        int outputW, outputH; //Output width and height.
        int offsetX = 0, offsetY = 0; //Offset required to place the picture on the center of the image
        float sourceAR = (float)sourceW / sourceH, outputAR = aspectRatio; //Calculate source aspect ratio and rename output aspect ratio (just for symmetry)

        if (sourceAR >= outputAR)
        {
            outputW = sourceW; //Keep same width and recalculate new height
            outputH = Mathf.RoundToInt(outputW * (1f / outputAR));
            offsetY = (outputH - sourceH) / 2;
        } else 
        {
            outputH = sourceH;
            outputW = Mathf.RoundToInt(outputH * outputAR);
            offsetX = (outputW - sourceW) / 2;
        }

        Texture2D output = new(outputW, outputH);

        //Create a new array of pixels with the same dimensions as the texture
        Color32[] pixels = new Color32[outputW * outputH];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        output.SetPixels32(pixels);

        output.SetPixels32(offsetX, offsetY, sourceW, sourceH, source.GetPixels32());
        output.Apply();
        return output;
    }
    /// <summary>Creates a copy of the given Texture2D object.</summary>
    /// <returns>A new Texture2D object with the same properties and pixel data as the source texture.</returns>
    public static Texture2D Copy(this Texture2D source)
    {
        Texture2D textureCopy = new(source.width, source.height, source.format, source.mipmapCount > 1);
        Graphics.CopyTexture(source, textureCopy);
        return textureCopy;
    }
}

public static class SpriteExtensions
{
    ///<summary>Destroys the sprite, its texture and nulls the reference</summary>
    ///<remarks>Only use if sprite and its texture were created on runtime</remarks>
    public static void Dispose(this Sprite sprite)
    {
        if (sprite == null) return;
        Sprite.Destroy(sprite.texture);
        Sprite.Destroy(sprite);
        sprite = null;
    }
}

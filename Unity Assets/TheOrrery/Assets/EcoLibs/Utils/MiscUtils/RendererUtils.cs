// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eco.Client.Utils
{
    public static class RendererUtils
    {
        public static void ReplaceRendererShaders(Renderer renderer, Shader shader, string modificationName="modified")
        {
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < renderer.sharedMaterials.Length; ++i)
            {
                newMaterials[i] = new Material(renderer.sharedMaterials[i]);
                newMaterials[i].name += " (" + modificationName + ")";
                newMaterials[i].shader = shader;
            }
            renderer.sharedMaterials = newMaterials;
        }

        public static void ReplaceRendererShaders(IEnumerable<Renderer> renderers, Shader shader)
        {
            // Make non-curved copies of all the shaders for each renderer
            foreach (var renderer in renderers)
                ReplaceRendererShaders(renderer, shader);
        }

        public static Rect GetScreenSpaceBounds(this Renderer renderer)
        {
            Bounds worldBounds = renderer.bounds;
            IEnumerable<Vector3> worldPoints = EnumerateCombinations(worldBounds.max, worldBounds.min);
            IEnumerable<Vector3> screenPoints = worldPoints
                .Select(point => Camera.main.WorldToScreenPoint(point))
                .Where(point => point.z >= 0);

            if (screenPoints.Any())
            {
                Bounds screenBounds = new Bounds(screenPoints.First(), Vector3.zero);
                foreach (Vector3 screenPoint in screenPoints)
                    screenBounds.Encapsulate(screenPoint);
                
                return RectExtensions.NewFromPoints(screenBounds.min, screenBounds.max);
            }
            else
                return default(Rect);
        }

        private static IEnumerable<Vector3> EnumerateCombinations(params Vector3[] vectors)
        {
            IEnumerable<float> xs = vectors.Select(v => v.x);
            IEnumerable<float> ys = vectors.Select(v => v.y);
            IEnumerable<float> zs = vectors.Select(v => v.z);
            List<Vector3> combinations = new List<Vector3>();

            foreach (float x in xs)
                foreach (float y in ys)
                    foreach (float z in zs)
                        combinations.Add(new Vector3(x, y, z));

            return combinations;
        }
    }
}

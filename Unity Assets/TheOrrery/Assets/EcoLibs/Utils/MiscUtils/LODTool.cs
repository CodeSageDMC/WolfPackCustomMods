// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class LODTool : TrackableBehavior
{
    public bool applyLODs;
    public float[] lodValues = new float[] { .03f };

    void Update()
    {
        if (applyLODs)
        {
            foreach (Transform child in this.transform)
            {
                var lod = child.GetComponent<LODGroup>();
                if (lod != null)
                {
                    var lods = lod.GetLODs();
                    if (lods.Length == lodValues.Length)
                    {
                        for (int i = 0; i < lods.Length; i++)
                        {
                            lods[i].screenRelativeTransitionHeight = lodValues[i];
                        }
                        lod.SetLODs(lods);

                        var source = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                        var path = AssetDatabase.GetAssetPath(source);
                        PrefabUtility.SaveAsPrefabAsset(child.gameObject, path);
                    }
                }
            }

            applyLODs = false;
        }
    }
}
#endif

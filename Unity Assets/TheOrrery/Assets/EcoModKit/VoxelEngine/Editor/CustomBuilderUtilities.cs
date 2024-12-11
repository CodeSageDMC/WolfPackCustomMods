using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomBuilderUtilities
{
    // Struct to hold the meshes for the LOD group, used so we can automatically extract them from the FBXs exported by the blender script
    struct lodGroupMeshes
    {
        public Mesh lod0Mesh;
        public Mesh lod1Mesh;
        public Mesh lod2Mesh;
        public Mesh colliderMesh;
    }

    // This method is called from the CustomBuilder class when clicking on the generate Lod group button
    public static void CreateLodGroups(CustomBuilder builder)
    {
        EditorUtility.SetDirty(builder);

        foreach (var usageCase in builder.usageCases)
        {
            BlockMeshLodGroup lodGroup = null;
            string lodGroupPath = AssetDatabase.GetAssetPath(usageCase.mesh).Replace(".fbx", "_LodGroup.asset");
            bool assetExists = AssetDatabase.AssetPathExists(lodGroupPath);

            // Load existing LOD group if it exists
            if (assetExists)
            {
                lodGroup = AssetDatabase.LoadAssetAtPath<BlockMeshLodGroup>(lodGroupPath);
            }

            // Check if the LOD group in the usage case needs updating or creating
            if (usageCase.blockMeshLodGroup != null)
            {
                if (lodGroup == null || usageCase.blockMeshLodGroup != lodGroup)
                {
                    Debug.Log("Updating existing LOD group in usage case");
                    usageCase.blockMeshLodGroup = lodGroup;                    
                }
                UpdateLodGroup(lodGroup, GetLodMeshes(usageCase.mesh));
            }
            else
            {
                if (lodGroup == null)
                {
                    Debug.Log("Creating new LOD group");
                    lodGroup = CreateNewLodGroup(GetLodMeshes(usageCase.mesh));
                    AssetDatabase.CreateAsset(lodGroup, lodGroupPath);
                }
                else
                {
                    Debug.Log("Updating existing LOD group asset");
                    UpdateLodGroup(lodGroup, GetLodMeshes(usageCase.mesh));
                }
            }

            // Assign the correct LOD group to the usage case
            usageCase.blockMeshLodGroup = lodGroup;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    // Updates an existing LOD group with new meshes
    static void UpdateLodGroup(BlockMeshLodGroup lodGroup, lodGroupMeshes meshes)
    {
        lodGroup.LOD0[0].mesh   = meshes.lod0Mesh;
        lodGroup.LOD1.mesh      = meshes.lod1Mesh;
        lodGroup.LOD2.mesh      = meshes.lod2Mesh;
        lodGroup.Collider       = meshes.colliderMesh;
    }

    // Get all the Lod Group Meshes from the game object specified in the usage case
    static lodGroupMeshes GetLodMeshes(GameObject GO)
    {        
        Transform[] children = GO.GetComponentsInChildren<Transform>();
        lodGroupMeshes meshes = new lodGroupMeshes();

        foreach (var child in children)
        {
            if (child.gameObject.GetComponent<MeshFilter>() != null)
            {
                Mesh mesh = child.gameObject.GetComponent<MeshFilter>().sharedMesh;
                string meshName = mesh.name;

                // Setup the meshes based on the naming convention
                if      (meshName.EndsWith("_BLOCKLOD_1")) meshes.lod1Mesh  = mesh;
                else if (meshName.EndsWith("_BLOCKLOD_2")) meshes.lod2Mesh  = mesh;
                else if (meshName.EndsWith("Collider")) meshes.colliderMesh = mesh;
                else if (child.gameObject == GO) meshes.lod0Mesh            = mesh; // Handle the base mesh
                else // Handle unexpected naming
                {
                    Debug.LogError("Unexpected mesh name: " + meshName);                    
                }
            }
        }
        return meshes;
    }

    // Create a new LOD group from the specified meshes
    static BlockMeshLodGroup CreateNewLodGroup(lodGroupMeshes meshes)
    {
        BlockMeshLodGroup lodGroup = ScriptableObject.CreateInstance<BlockMeshLodGroup>();
        lodGroup.name = meshes.lod0Mesh != null ? meshes.lod0Mesh.name : "NewLodGroup";

        if (meshes.lod0Mesh != null)        lodGroup.LOD0[0].mesh   = meshes.lod0Mesh;        
        if (meshes.lod1Mesh != null)        lodGroup.LOD1.mesh      = meshes.lod1Mesh;        
        if (meshes.lod2Mesh != null)        lodGroup.LOD2.mesh      = meshes.lod2Mesh;        
        if (meshes.colliderMesh != null)    lodGroup.Collider       = meshes.colliderMesh;        

        return lodGroup;
    }

    public static CustomBuilder[] GenerateRotations(CustomBuilder builder)
    {
        var path = AssetDatabase.GetAssetPath(builder);
        var name = builder.name;

        CustomBuilder[] newBuilders = new CustomBuilder[3];

        // Loop through for the 3 additional rotations
        for (int i = 1; i < 4; i++)
        {
            int rotation = i * 90;                                                                  // Calculate current rotation from iteration
            string newPath = path.Replace(name, name + " " + rotation);                             // Create a new path based on the old name + rotation ie. BlockName -> BlockName 90
            AssetDatabase.CopyAsset(path, newPath);                                                 // Copies the original builder to the new path
            CustomBuilder newBuilder = AssetDatabase.LoadAssetAtPath<CustomBuilder>(newPath);       // Get a reference to the new builder so we can modify it's import rotations and rules
            EditorUtility.SetDirty(newBuilder);

            foreach (var useageCase in newBuilder.usageCases)                                       // Loop through each useage case, this is where we specify what mesh will show up
            {
                useageCase.importRotation.y += rotation;                                            // Rotates the mesh in the current useage case by the current rotation amount

                foreach (var cond in useageCase.conditions)                                         // Loop through each condition so we can change the direction of the rules to match the new rotation
                {
                    Vector3 originalVector = OffsetCondition.OffsetMapping[(int)cond.offsetType];   // Get the mapped vector3 for the current offset eg. Offset_020 = (-1, 1, -1) 
                    Vector3 newRotation = originalVector.RotateAroundYClockwise(rotation);          // rotate it by the current rotation
                    cond.offsetType = OffsetCondition.GetFromVector(newRotation);                   // Set the new offsetType by getting it from the the new rotated vector
                }
            }
            newBuilders[i - 1] = newBuilder;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return newBuilders;
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;

public class ModelProcessor : AssetPostprocessor
{
    // we've got someone using 3DS, and their scales are all messed up. this will fix the scales
    // for any FBX files named _3DS.

    // also, we want to change the smoothing angle to get the look we're going for.
    void OnPreprocessModel()
    {
        ModelImporter importer = assetImporter as ModelImporter;
        //importer.globalScale = assetPath.Contains("_3DS") ? 0.01f : 0.01f;

        importer.normalImportMode = ModelImporterTangentSpaceMode.Calculate;
        importer.tangentImportMode = ModelImporterTangentSpaceMode.Calculate;
        importer.normalSmoothingAngle = 0f;
    }
}

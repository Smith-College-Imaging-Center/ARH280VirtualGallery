using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
class TexturePostprocessor : AssetPostprocessor
{
    public string imagePath = "DoNotScale";

    //When images are imported, if they are in the imagePath
    //  turn Non Power of Two Scaling to None, so that the images maintain their aspect ratio
    void OnPreprocessTexture()
    {
        if (assetPath.Contains(imagePath))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.npotScale = TextureImporterNPOTScale.None;
        }
    }
}

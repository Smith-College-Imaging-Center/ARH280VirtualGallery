using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PrefabManager : MonoBehaviour
{
#if UNITY_EDITOR
    // points to the resource path of where images are located
    // for each image in the given path a prefab will be created
    // all images must be in the Resources path (e.g. "Resources", "Resources\Textures", "Resources\Textures\Images")
    // the image path should vbe entered like so for "Resources" root folder leave empty, for "Resources\Textures" folder enter "Textures", "Resources\Textures\Images")
    // if empty images will be loaded from the "Resources" folder
    public GameObject itemInfoScreenPrefab;

    public string imagePath = "Textures/PictureFrameImages"; // this is the default (located in "Resources\Textures")

    public float frameDepth = 0.05f;
    public float frameWidth = 0.08f;
    public Color frameColor;

    // points to path of where prefabs will be stored - can be made public if desired
    private string prefabPath = "Prefabs"; // this is the default (located in "Resources\Prefabs")

    // points to path of where created materials will be stored - can be made public if desired
    // if left empty, materials will be stored in the same path as the prefab path
    private string materialPath = "Materials"; // this is the default (located in "Resources\Materials")

    // Use this for initialization
    public void CreatePrefabs()
    {
        // this can only be done in the Unity Editor
        imagePath = CleanPath(imagePath);
        if (AssetDatabase.IsValidFolder("Assets/Resources/" + imagePath))
        {
            string tempPrefabPath = "Assets/Resources/" + CleanPath(prefabPath);
            string tempMaterialPath = "Assets/Resources/" + CleanPath(materialPath);
        
            // create the folders if they do not exist
            if (!AssetDatabase.IsValidFolder(tempMaterialPath))
            {
                AssetDatabase.CreateFolder( "Assets/Resources", CleanPath(materialPath));
            }
            if (!AssetDatabase.IsValidFolder(tempPrefabPath))
            {
                AssetDatabase.CreateFolder( "Assets/Resources", CleanPath(prefabPath));
            }
            AssetDatabase.SaveAssets();

            // add path separators
            tempPrefabPath += "/";
            tempMaterialPath += "/";
            
            Debug.Log("prefabPath = " + tempPrefabPath + ", materialPath = " + tempMaterialPath);

            //Create a frame material (one material for all frames)
            Material frameMaterial = new Material(Shader.Find("Standard"));
            frameMaterial.SetColor("_Color", frameColor);
            AssetDatabase.CreateAsset(frameMaterial, tempMaterialPath + "Frame_Mat.mat");

            List<Texture2D> textures = new List<Texture2D>(Resources.LoadAll<Texture2D>(imagePath));
            if (textures.Count > 0)
            {
                for (int i = 0; i < textures.Count; i++)
                {
                    // first create the material for the image
                    Material material = new Material(Shader.Find("Sprites/Default"));
                    //Material material = new Material(Shader.Find("Standard"));
                    material.SetTexture("_MainTex", textures[i]);
                    AssetDatabase.CreateAsset(material, tempMaterialPath + textures[i].name + "_Mat.mat");

                    // now create the quad
                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quad.name = textures[i].name;
                    quad.transform.position = Vector3.zero;

                    // scale the quad to the aspect ratio of the image
                    Vector3 quadScale = new Vector3();
                    quadScale = quad.transform.localScale;
                    float w = textures[i].width;
                    //Debug.Log(textures[i] + " width: " + w);
                    float h = textures[i].height;
                    //Debug.Log(textures[i] + " height: " + h);
                    if (textures[i].width > textures[i].height)
                    {
                        quadScale.y = h / w;
                        quad.transform.localScale = quadScale;
                    }
                    else
                    {
                        quadScale.x = w / h;
                        quad.transform.localScale = quadScale;
                    }

                    //set quad material
                    MeshRenderer meshRenderer = quad.GetComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = material;

                    //make the frame
                    GameObject frameObject = new GameObject();

                    //create frame parts and set quad and all frame parts to be children of frameObject
                    quad.name = "ImageQuad";
                    quad.transform.parent = frameObject.transform;
                    GameObject frameTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    frameTop.name = "FrameTop";
                    frameTop.transform.parent = frameObject.transform;
                    GameObject frameBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    frameBottom.name = "FrameBottom";
                    frameBottom.transform.parent = frameObject.transform;
                    GameObject frameLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    frameLeft.name = "FrameLeft";
                    frameLeft.transform.parent = frameObject.transform;
                    GameObject frameRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    frameRight.name = "FrameRight";
                    frameRight.transform.parent = frameObject.transform;

                    //create framePos and frameScale variables, then set positions and scaling for frame parts
                    Vector3 framePos = new Vector3(0, (quadScale.y / 2) + (frameWidth / 2), 0);
                    Vector3 frameScale = new Vector3(quadScale.x + (frameWidth * 2), frameWidth, frameDepth);
                    meshRenderer = frameTop.GetComponent<MeshRenderer>();
                    frameTop.transform.position = framePos;
                    frameTop.transform.localScale = frameScale;
                    meshRenderer.sharedMaterial = frameMaterial;

                    framePos = new Vector3(0, (-quadScale.y / 2) - (frameWidth / 2), 0);
                    meshRenderer = frameBottom.GetComponent<MeshRenderer>();
                    frameBottom.transform.position = framePos;
                    frameBottom.transform.localScale = frameScale;
                    meshRenderer.sharedMaterial = frameMaterial;

                    framePos = new Vector3((-quadScale.x / 2) - (frameWidth / 2), 0, 0);
                    frameScale = new Vector3(frameWidth, quadScale.y, frameDepth);
                    meshRenderer = frameLeft.GetComponent<MeshRenderer>();
                    frameLeft.transform.position = framePos;
                    frameLeft.transform.localScale = frameScale;
                    meshRenderer.sharedMaterial = frameMaterial;

                    framePos = new Vector3((quadScale.x / 2) + (frameWidth / 2), 0, 0);
                    meshRenderer = frameRight.GetComponent<MeshRenderer>();
                    frameRight.transform.position = framePos;
                    frameRight.transform.localScale = frameScale;
                    meshRenderer.sharedMaterial = frameMaterial;

                    //add the scripts and components, and set up links among them
                    frameObject.AddComponent<GalleryItem>();
                    frameObject.GetComponent<GalleryItem>().itemInfoScreenPrefab = itemInfoScreenPrefab;
                    frameObject.AddComponent<BoxCollider>();
                    frameScale = new Vector3(quadScale.x + (2 * frameWidth), quadScale.y + (2 * frameWidth), frameDepth);
                    frameObject.GetComponent<BoxCollider>().size = frameScale;

                    //Set layer
                    frameObject.layer = 8;

                    // create the prefab
                    PrefabUtility.SaveAsPrefabAsset(frameObject, tempPrefabPath + textures[i].name + ".prefab");

                    // destroy the frameObject GameObject so that it does not appear on the screen after completion
                    UnityEngine.Object.DestroyImmediate(frameObject);
                }
                // save all asset changes to disk.
                AssetDatabase.SaveAssets();
            }
        }
    }

    string CleanPath(string path)
    {
        // remove "Resources" from imagePath if exists
        if ((path != "") && (path.ToLower().Contains("resources\\")))
        {
            path = path.Substring(0, 10);
        }
        else if ((path != "") && (path.ToLower().Contains("resources")))
        {
            path = path.Substring(0, 10);
        }
        Debug.Log("path = " + path);

        return path;
    }
#endif
}
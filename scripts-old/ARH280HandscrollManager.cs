using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ARH280HandscrollManager : MonoBehaviour
{
#if UNITY_EDITOR
    // points to the resource path of where images are located
    // for each image in the given path a prefab will be created
    // all images must be in the Resources path (e.g. "Resources", "Resources\Textures", "Resources\Textures\Images")
    // the image path should vbe entered like so for "Resources" root folder leave empty, for "Resources\Textures" folder enter "Textures", "Resources\Textures\Images")
    // if empty images will be loaded from the "Resources" folder
    public GameObject itemInfoScreenPrefab;

    public string imagePath = "Textures/HandscrollImages"; // this is the default (located in "Resources\Textures")

    public Color scrollColor;
    public GameObject scrollLMesh;
    public GameObject scrollRMesh;

    // points to path of where prefabs will be stored - can be made public if desired
    private string prefabPath = "HandscrollPrefabs"; // this is the default (located in "Resources\Prefabs")

    // points to path of where created materials will be stored - can be made public if desired
    // if left empty, materials will be stored in the same path as the prefab path
    private string materialPath = "HandscrollMaterials"; // this is the default (located in "Resources\Materials")

    // Use this for initialization
    public void CreateHandscrolls()
    {
        // this can only be done in the Unity Editor
        if (AssetDatabase.IsValidFolder("Assets/Resources/" + imagePath))
        {
            string tempPrefabPath = "Assets/Resources/" + prefabPath;
            string tempMaterialPath = "Assets/Resources/" + materialPath;
        
            // create the folders if they do not exist
            if (!AssetDatabase.IsValidFolder(tempMaterialPath))
            {
                AssetDatabase.CreateFolder( "Assets/Resources", materialPath);
                Debug.Log(materialPath);
            }
            if (!AssetDatabase.IsValidFolder(tempPrefabPath))
            {
                AssetDatabase.CreateFolder( "Assets/Resources", prefabPath);
            }
            AssetDatabase.SaveAssets();

            // add path separators
            tempPrefabPath += "/";
            tempMaterialPath += "/";
            
            Debug.Log("prefabPath = " + tempPrefabPath + ", materialPath = " + tempMaterialPath);

            //Create a scroll material (one material for all scrolls)
            Material scrollMaterial = new Material(Shader.Find("Standard"));
            scrollMaterial.SetColor("_Color", scrollColor);
            scrollMaterial.SetFloat("_Glossiness", 0.0f);
            AssetDatabase.CreateAsset(scrollMaterial, tempMaterialPath + "Scroll_Mat.mat");

            List<Texture2D> textures = new List<Texture2D>(Resources.LoadAll<Texture2D>(imagePath));
            if (textures.Count > 0)
            {
                for (int i = 0; i < textures.Count; i++)
                {
                    //create empty object to hold everything
                    GameObject handscrollObject = new GameObject();

                    Material material = new Material(Shader.Find("Standard"));
                    material.SetTexture("_MainTex", textures[i]);
                    AssetDatabase.CreateAsset(material, tempMaterialPath + textures[i].name + "_Mat.mat");

                    // now create the image quad
                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quad.name = textures[i].name;
                    quad.transform.position = Vector3.zero;
                    quad.name = "ImageQuad";
                    DestroyImmediate(quad.GetComponent<Collider>());
                    quad.transform.parent = handscrollObject.transform;

                    //assign material to image quad
                    MeshRenderer meshRenderer = quad.GetComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = material;

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

                    Vector3 insertPos = new Vector3((-quadScale.x / 2), 0, 0);
                    Vector3 insertScale = new Vector3(quadScale.y * .01f, quadScale.y * .01f, quadScale.y * .01f);
                    GameObject cloneL = Instantiate(scrollLMesh);
                    cloneL.transform.parent = handscrollObject.transform;
                    cloneL.transform.position = insertPos;
                    cloneL.transform.localScale = insertScale;
                    meshRenderer = cloneL.GetComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = scrollMaterial;

                    insertPos = new Vector3(quadScale.x / 2, 0, 0);
                    GameObject cloneR = Instantiate(scrollRMesh);
                    cloneR.transform.parent = handscrollObject.transform;
                    cloneR.transform.position = insertPos;
                    cloneR.transform.localScale = insertScale;
                    meshRenderer = cloneR.GetComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = scrollMaterial;

                    //add the scripts and components, and set up links among them
                    handscrollObject.AddComponent<GalleryItem>();
                    handscrollObject.GetComponent<GalleryItem>().itemInfoScreenPrefab = itemInfoScreenPrefab;
                    handscrollObject.AddComponent<BoxCollider>();
                    insertScale = new Vector3(quadScale.x, quadScale.y, .05f);
                    handscrollObject.GetComponent<BoxCollider>().size = insertScale;

                    //Set layer
                    handscrollObject.layer = 8;

                    //rotate the whole thing
                    Vector3 insertRot = new Vector3(60, 0, 0);
                    handscrollObject.transform.rotation = Quaternion.Euler(insertRot);

                    // create the prefab
                    PrefabUtility.SaveAsPrefabAsset(handscrollObject, tempPrefabPath + textures[i].name + ".prefab");

                    // destroy the frameObject GameObject so that it does not appear on the screen after completion
                    UnityEngine.Object.DestroyImmediate(handscrollObject);
                }
                // save all asset changes to disk.
                AssetDatabase.SaveAssets();
            }
        }
    }

#endif
}
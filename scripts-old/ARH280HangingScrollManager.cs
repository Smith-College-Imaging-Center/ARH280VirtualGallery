using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ARH280PrefabManager : MonoBehaviour
{
#if UNITY_EDITOR

    private string imagePath; // this is the default (located in "Resources\Textures")

    // points to path of where prefabs will be stored
    private string prefabPath;
    private string tempPrefabPath;

    // points to path of where created materials will be stored
    // if left empty, materials will be stored in the same path as the prefab path
    private string materialPath;
    private string tempMaterialPath;

    [Header("Handscroll Settings")]
    public string handscrollImagePath = "Textures/HandscrollImages";
    public Color scrollColor;
    public GameObject scrollLMesh;
    public GameObject scrollRMesh;

    [Header("Hanging Scroll Settings")]
    public string hangingScrollImagePath = "Textures/HangingScrollImages";
    //border width as percentage of image width
    public float borderWidth = 0.1f;
    public Color borderColor;
    public float backingWidth = 0.4f;
    public Color backingColor;
    public Color suspenderColor;

    [Header("Picture Frame Settings")]
    public string pictureFrameImagePath = "Textures/PictureFrameImages";
    public float frameDepth = 0.05f;
    public float frameWidth = 0.08f;
    public Color frameColor;

    [Header("Shared Settings")]
    public GameObject itemInfoScreenPrefab;
    public galleryData galleryData;

    public enum ObjectType{
        HAND_SCROLL,
        HANGING_SCROLL,
        PICTURE_FRAME
    }

    public ObjectType type = ObjectType.HAND_SCROLL;

    public void CreatePrefabs()
    {
        if (type == ObjectType.HAND_SCROLL)
        {
            CreateHandscrolls();
        } else if (type == ObjectType.HANGING_SCROLL)
        {
            CreateHangingScrolls();
        } else if (type == ObjectType.PICTURE_FRAME)
        {
            CreatePictureFrames();
        }

        //switch (ot)
        //{
        //    case ObjectType.HAND_SCROLL:
        //        {
        //            CreateHandscrolls();
        //            break;
        //        }
        //    case ObjectType.HANGING_SCROLL:
        //        {
        //            CreateHangingScrolls();
        //            break;
        //        }
        //    case ObjectType.PICTURE_FRAME:
        //        {
        //            CreatePictureFrames();
        //            break;
        //        }
        //    default: break;
                
        //}
    }

    public void CreateHandscrolls()
    {
        imagePath = handscrollImagePath;
        SetupFolders("Handscroll");
        // this can only be done in the Unity Editor

            //Create a scroll material (one material for all scrolls)
            Material scrollMaterial = new Material(Shader.Find("Standard"));
            scrollMaterial.SetColor("_Color", scrollColor);
            scrollMaterial.SetFloat("_Glossiness", 0.0f);
            AssetDatabase.CreateAsset(scrollMaterial, tempMaterialPath + "Scroll_Mat.mat");

            List<Texture2D> textures = new List<Texture2D>(Resources.LoadAll<Texture2D>(imagePath));
        Debug.Log("textures count = " + textures.Count);
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

    public void CreateHangingScrolls()
    {
        imagePath = hangingScrollImagePath;
        SetupFolders("HangingScroll");
        // this can only be done in the Unity Editor

            //Create a border material (one material for all borders)
            Material borderMaterial = new Material(Shader.Find("Standard"));
            borderMaterial.SetColor("_Color", borderColor);
            borderMaterial.SetFloat("_Glossiness", 0.0f);
            AssetDatabase.CreateAsset(borderMaterial, tempMaterialPath + "Border_Mat.mat");

            //Create a backing material (one material for all backings)
            Material backingMaterial = new Material(Shader.Find("Standard"));
            backingMaterial.SetColor("_Color", backingColor);
            backingMaterial.SetFloat("_Glossiness", 0.0f);
            AssetDatabase.CreateAsset(backingMaterial, tempMaterialPath + "Backing_Mat.mat");

            //Create a suspender material (one material for all suspenders)
            Material suspenderMaterial = new Material(Shader.Find("Standard"));
            suspenderMaterial.SetColor("_Color", suspenderColor);
            suspenderMaterial.SetFloat("_Glossiness", 0.0f);
            AssetDatabase.CreateAsset(suspenderMaterial, tempMaterialPath + "Suspender_Mat.mat");

            List<Texture2D> textures = new List<Texture2D>(Resources.LoadAll<Texture2D>(imagePath));
            if (textures.Count > 0)
            {
                for (int i = 0; i < textures.Count; i++)
                {
                    Debug.Log(textures[i].name);
                    //create empty object to use as parent
                    GameObject hangingScrollObject = new GameObject();

                    //create a list
                    List<GameObject> hangingScrollParts = new List<GameObject>();

                    //create the material for the image quad
                    Material material = new Material(Shader.Find("Sprites/Default"));
                    material.SetTexture("_MainTex", textures[i]);
                    AssetDatabase.CreateAsset(material, tempMaterialPath + textures[i].name + "_Mat.mat");

                    // now create the image quad
                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quad.name = textures[i].name;
                    quad.transform.position = Vector3.zero;
                    quad.name = "ImageQuad";
                    quad.transform.parent = hangingScrollObject.transform;

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

                    float borderWidthX = quadScale.x * borderWidth;
                    float borderWidthY = quadScale.y * borderWidth;
                    float tempBackingWidth = quadScale.y * backingWidth;

                    //create border parts, backing parts, suspender parts, rod, roller, knobs,
                    //and set them all as children of hangingScrollObject. Also add them to
                    //hangingScrollParts List

                    //border parts
                    GameObject borderTop = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    borderTop.name = "BorderTop";
                    hangingScrollParts.Add(borderTop);

                    GameObject borderBottom = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    borderBottom.name = "BorderBottom";
                    hangingScrollParts.Add(borderBottom);

                    GameObject borderLeft = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    borderLeft.name = "BorderLeft";
                    hangingScrollParts.Add(borderLeft);

                    GameObject borderRight = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    borderRight.name = "BorderRight";
                    hangingScrollParts.Add(borderRight);

                    //backing parts
                    GameObject backingTop = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    backingTop.name = "BackingTop";
                    hangingScrollParts.Add(backingTop);

                    GameObject backingBottom = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    backingBottom.name = "BackingBottom";
                    hangingScrollParts.Add(backingBottom);

                    //suspender parts
                    GameObject suspenderLeft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    suspenderLeft.name = "SuspenderLeft";
                    hangingScrollParts.Add(suspenderLeft);

                    GameObject suspenderRight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    suspenderRight.name = "SuspenderRight";
                    hangingScrollParts.Add(suspenderRight);

                    //rod
                    GameObject rod = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    rod.name = "Rod";
                    hangingScrollParts.Add(rod);

                    //roller
                    GameObject roller = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    roller.name = "Roller";
                    hangingScrollParts.Add(roller);

                    //parent all parts to hangingScrollObject and get rid of unnecessary colliders
                    foreach(GameObject obj in hangingScrollParts)
                    {
                        obj.transform.parent = hangingScrollObject.transform;
                        DestroyImmediate(obj.GetComponent<Collider>());
                    }

                    //create insertPos and insertScale variables, and use them to set positions and scaling for all hanging scroll parts
                    Vector3 insertPos = new Vector3(0, (quadScale.y / 2) + (borderWidthY / 2), 0);
                    Vector3 insertRot = new Vector3(0, 0, 0);
                    Vector3 insertScale = new Vector3(quadScale.x + (borderWidthX * 2), borderWidthY, 1);
                    meshRenderer = borderTop.GetComponent<MeshRenderer>();
                    borderTop.transform.position = insertPos;
                    borderTop.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = borderMaterial;

                    insertPos = new Vector3(0, (-quadScale.y / 2) - (borderWidthY / 2), 0);
                    meshRenderer = borderBottom.GetComponent<MeshRenderer>();
                    borderBottom.transform.position = insertPos;
                    borderBottom.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = borderMaterial;

                    insertPos = new Vector3((-quadScale.x / 2) - (borderWidthX / 2), 0, 0);
                    insertScale = new Vector3(borderWidthX, quadScale.y, 1);
                    meshRenderer = borderLeft.GetComponent<MeshRenderer>();
                    borderLeft.transform.position = insertPos;
                    borderLeft.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = borderMaterial;

                    insertPos = new Vector3((quadScale.x / 2) + (borderWidthX / 2), 0, 0);
                    meshRenderer = borderRight.GetComponent<MeshRenderer>();
                    borderRight.transform.position = insertPos;
                    borderRight.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = borderMaterial;

                    insertPos = new Vector3(0, (quadScale.y / 2) + (borderWidthY) + (tempBackingWidth / 2), 0);
                    insertScale = new Vector3(quadScale.x + (borderWidthX * 2), tempBackingWidth, 1);
                    meshRenderer = backingTop.GetComponent<MeshRenderer>();
                    backingTop.transform.position = insertPos;
                    backingTop.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = backingMaterial;

                    insertPos = new Vector3(0, (-quadScale.y / 2) - (borderWidthY) - (tempBackingWidth / 2), 0);
                    meshRenderer = backingBottom.GetComponent<MeshRenderer>();
                    backingBottom.transform.position = insertPos;
                    backingBottom.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = backingMaterial;

                    insertPos = new Vector3(0, (quadScale.y / 2) + borderWidthY + tempBackingWidth, 0);
                    insertScale = new Vector3(quadScale.x + (borderWidthX * 2), 0.035f, 0.023f);
                    meshRenderer = rod.GetComponent<MeshRenderer>();
                    rod.transform.position = insertPos;
                    rod.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = backingMaterial;

                    //roller
                    insertPos = new Vector3(0, (-quadScale.y / 2) - borderWidthY - tempBackingWidth, 0);
                    insertRot = new Vector3(0, 0, 90);
                    insertScale = new Vector3(0.05f, (quadScale.x + (borderWidthX * 2)) * 0.55f, 0.05f);
                    meshRenderer = roller.GetComponent<MeshRenderer>();
                    roller.transform.position = insertPos;
                    roller.transform.rotation = Quaternion.Euler(insertRot);
                    roller.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = backingMaterial;

                    //suspenderLeft
                    insertPos = new Vector3(-0.085f, ((quadScale.y / 2) * 1.09f) + borderWidthY + tempBackingWidth, 0);
                    insertRot = new Vector3(0, 0, -60);
                    insertScale = new Vector3(0.01f, 0.1f, 0.01f);
                    meshRenderer = suspenderLeft.GetComponent<MeshRenderer>();
                    suspenderLeft.transform.position = insertPos;
                    suspenderLeft.transform.rotation = Quaternion.Euler(insertRot);
                    suspenderLeft.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = suspenderMaterial;

                    //suspenderRight
                    insertPos = new Vector3(0.085f, ((quadScale.y / 2) * 1.09f) + borderWidthY + tempBackingWidth, 0);
                    insertRot = new Vector3(0, 0, 60);
                    insertScale = new Vector3(0.01f, 0.1f, 0.01f);
                    meshRenderer = suspenderRight.GetComponent<MeshRenderer>();
                    suspenderRight.transform.position = insertPos;
                    suspenderRight.transform.rotation = Quaternion.Euler(insertRot);
                    suspenderRight.transform.localScale = insertScale;
                    meshRenderer.sharedMaterial = suspenderMaterial;

                    //add the scripts and components, and set up links among them
                    hangingScrollObject.AddComponent<GalleryItem>();
                    hangingScrollObject.GetComponent<GalleryItem>().galleryData = galleryData;
                    hangingScrollObject.GetComponent<GalleryItem>().itemInfoScreenPrefab = itemInfoScreenPrefab;
                    hangingScrollObject.GetComponent<GalleryItem>().PopulateGalleryItemInfo(textures[i]);
                    hangingScrollObject.AddComponent<BoxCollider>();
                    insertScale = new Vector3(quadScale.x + (2 * borderWidthX), quadScale.y + (2 * borderWidthY) + (2 * tempBackingWidth), .05f);
                    hangingScrollObject.GetComponent<BoxCollider>().size = insertScale;

                    //Set layer
                    hangingScrollObject.layer = 8;

                    // create the prefab
                    PrefabUtility.SaveAsPrefabAsset(hangingScrollObject, tempPrefabPath + textures[i].name + ".prefab");

                    // destroy the frameObject GameObject so that it does not appear on the screen after completion
                    UnityEngine.Object.DestroyImmediate(hangingScrollObject);
                }
                // save all asset changes to disk.
                AssetDatabase.SaveAssets();
            }
    }

    public void CreatePictureFrames()
    {
        imagePath = pictureFrameImagePath;
        SetupFolders("PictureFrame");
        // this can only be done in the Unity Editor

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

    public void SetupFolders(string prefix)
    {
        //open system dialog, user chooses folder of images
            //imagePath = EditorUtility.OpenFolderPanel("Choose " + prefix + " textures folder", "/Assets/Resources", "");

        //split the path string at "Resources/" to make it relative to "Resources" folder
        //(for use with Resources.LoadAll())
            //string[] splitString = imagePath.Split(new string[] { "Resources/" }, StringSplitOptions.None);
            //foreach (string str in splitString)
            //{
            //    Debug.Log(str);
            //}
            //imagePath = splitString[splitString.Length - 1];
        
        //create prefab and material folder names
        prefabPath = prefix + "_Prefabs";
        materialPath = prefix + "_Materials";

        //create prefab and material folders if they do not exist
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/" + prefabPath))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", prefabPath);
            Debug.Log(prefabPath);
        }
        if (!AssetDatabase.IsValidFolder("Assets/Materials/" + materialPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", materialPath);
        }
        AssetDatabase.SaveAssets();

        // Add path separators. tempPrefabPath and tempMaterialPath are used when
        //generating names for generated prefabs and materials
        tempPrefabPath = "Assets/Prefabs/" + prefabPath + "/";
        tempMaterialPath = "Assets/Materials/" + materialPath + "/";

        Debug.Log("tempPrefabPath = " + tempPrefabPath + ", tempMaterialPath = " + tempMaterialPath);
    }
#endif
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GalleryItem : MonoBehaviour
{
    //Variables containing information about the artwork
    [Header("Artwork Details")]
    public Texture detailImage;
    public string artist = "Default Artist";
    public string artistDates;
    public string title = "Default Title";
    public string date = "Default Date";
    public string medium = "Default Medium";
    public string size;
    public string institution;
    public string labelText;
    public string typeOfObject;
    public string description = "A default description of this item.";

    //Item info screen prefab object
    public GameObject itemInfoScreenPrefab;
    public galleryData galleryDataObject;

    [System.NonSerialized]
    public GameObject clone;
    [System.NonSerialized]
    public GameObject tObj;
    [System.NonSerialized]
    public GameObject playerObject;

    private bool _isSelected = false;
    private bool _infoScreenIsShowing = false;

    Renderer rend;
    Material[] mats;
    private Color color = new Color(.2f, .2f, .2f);

    public bool isSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;
        }
    }

    public bool infoScreenIsShowing
    {
        get
        {
            return _infoScreenIsShowing;
        }
        set
        {
            _infoScreenIsShowing = value;
        }
    }

    void Awake()
    {
        //Let the user know this script won't work with a collider attached to the object
        if (!GetComponent<Collider>())
        {
            Debug.LogError(name + " is missing a collider.");
        }

        playerObject = GameObject.FindGameObjectWithTag("Player");

        //Enable emission keyword on this object
        if (GetComponent<Renderer>() != null)
        {
            rend = GetComponent<Renderer>();
            mats = rend.materials;
            foreach (Material mat in mats)
            {
                mat.EnableKeyword("_EMISSION");
            }
        }

        //Enable emission keyword on all materials on all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Renderer>() != null)
            {
                rend = transform.GetChild(i).GetComponent<Renderer>();
                mats = rend.materials;
                foreach (Material mat in mats)
                {
                    mat.EnableKeyword("_EMISSION");
                }
            }
        }
    }

    private void Update()
    {
        if (isSelected)
        {
            HighlightOn();
        }

        if (!isSelected)
        {
            HighlightOff();
        }
    }

    public void HighlightOn()
    {
        //Highlight all materials on this object
        if (GetComponent<Renderer>() != null)
        {
            rend = GetComponent<Renderer>();
            mats = rend.materials;
            foreach (Material mat in mats)
            {
                mat.SetColor("_EmissionColor", color);
                //Debug.Log("Emission color set for parent");
            }
        }

        //Highlight all materials on all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Renderer>() != null)
            {
                rend = transform.GetChild(i).GetComponent<Renderer>();
                mats = rend.materials;
                foreach (Material mat in mats)
                {
                    mat.SetColor("_EmissionColor", color);
                    //Debug.Log("Emission color set for children");
                }
            }
        }

    }

    public void HighlightOff()
    {
        //Turn off highlights for all materials on this object
        if (GetComponent<Renderer>() != null)
        {
            rend = GetComponent<Renderer>();
            mats = rend.materials;
            foreach (Material mat in mats)
            {
                mat.SetColor("_EmissionColor", Color.black);
            }
        }

        //Turn off highlights for all materials on all child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Renderer>() != null)
            {
                rend = transform.GetChild(i).GetComponent<Renderer>();
                mats = rend.materials;
                foreach (Material mat in mats)
                {
                    mat.SetColor("_EmissionColor", Color.black);
                }
            }
        }
        
    }

    public void PopulateGalleryItemInfo(Texture tex, galleryData gd, GameObject infoScreen)
    {
        itemInfoScreenPrefab = infoScreen;
        galleryDataObject = gd;
        var d = galleryDataObject.responses.Find(item => item.imageFileName == tex.name);
        if (d != null)
        {
            artist = d.artist;
            artistDates = d.artistDates;
            title = d.title;
            date = d.date;
            medium = d.medium;
            size = d.size;
            institution = d.institution;
            labelText = d.labelText;
            detailImage = tex;
        }
    }

    public virtual void BuildInfoScreen()
    {
        //If infoScreenIsShowing = false,
        //instantiate the clone and populate it with details from this object
        //(I'm doing this by getting the name of the UI objects I know are attached to the itemInfoScreenObject prefab)
        if (!infoScreenIsShowing)
        {
            clone = Instantiate(itemInfoScreenPrefab);

            PopulateInfoScreenText();

            //tObj = GameObject.Find("Artist");
            //tObj.GetComponent<Text>().text = artist;
            //tObj = GameObject.Find("ArtistDates");
            //tObj.GetComponent<Text>().text = artistDates + "\n";
            //tObj = GameObject.Find("Title");
            //tObj.GetComponent<Text>().text = title;
            //tObj = GameObject.Find("DateMediumSizeInstitution");
            //tObj.GetComponent<Text>().text = date + "\n" + medium + "\n" + size + "\n" + institution + "\n";
            //tObj = GameObject.Find("Description");
            //tObj.GetComponent<Text>().text = labelText;

            
            //Assign the detail image and display it with correct width/height ratio
            if (detailImage != null)
            {
                PopulateInfoScreenDetailImage();
            }

            infoScreenIsShowing = true;
            playerObject.GetComponent<FirstPersonController>().frozen = true;
        }

    }

    public virtual void PopulateInfoScreenText()
    {
        tObj = GameObject.Find("Artist");
        tObj.GetComponent<Text>().text = artist;
        tObj = GameObject.Find("ArtistDates");
        tObj.GetComponent<Text>().text = artistDates + "\n";
        tObj = GameObject.Find("Title");
        tObj.GetComponent<Text>().text = title;
        tObj = GameObject.Find("DateMediumSizeInstitution");
        tObj.GetComponent<Text>().text = date + "\n" + medium + "\n" + size + "\n" + institution + "\n";
        tObj = GameObject.Find("Description");
        tObj.GetComponent<Text>().text = labelText;
    }

    public virtual void PopulateInfoScreenDetailImage()
    {
        //this is the Scroll View object
        tObj = GameObject.Find("DetailImage");
        ScrollRect tScrollRect = tObj.GetComponent<ScrollRect>();

        //this is the Content object where the image lives, a child of the Scroll View object
        GameObject tContentObj = GameObject.Find("Content");
        tContentObj.GetComponent<RawImage>().texture = detailImage;
        RectTransform tContentRect = tContentObj.GetComponent<RectTransform>();
        //tContentRect.anchorMin = new Vector2(0, 1);
        //tContentRect.anchorMax = new Vector2(0, 1);
        //tContentRect.pivot = new Vector2(1, 1);

        float dH = detailImage.height;
        float dW = detailImage.width;
        float ratio = dW / dH;

        if (detailImage.width > detailImage.height)
        {
            //this is a wide image, scroll horizontally
            //ratio = dW / dH;
            tScrollRect.horizontal = true;
            tScrollRect.vertical = false;

            tContentRect.anchorMin = new Vector2(1f, 0.5f);
            tContentRect.anchorMax = new Vector2(1f, 0.5f);
            tContentRect.pivot = new Vector2(1f, 0.5f);

            tContentObj.GetComponent<AspectRatioFitter>().aspectRatio = ratio;

            //tContentRect.sizeDelta = new Vector2(ratio * tObj.GetComponent<RectTransform>().rect.height, tObj.GetComponent<RectTransform>().rect.height);

            //set it up so user scrolls right to left (for handscrolls)

            //float tWidth = tContentRect.rect.width;
            //Vector3 tPos = tContentRect.position;
            //tPos.x = (tObj.GetComponent<RectTransform>().rect.width - tWidth);
            //Debug.Log("Content rect width: " + tContentRect.rect.width + ", Scroll Rect width: " + tObj.GetComponent<RectTransform>().rect.width + ", tPos.x: " + tPos.x);
            //tContentRect.position = tPos;
            //Debug.Log("tContentRect.position: " + tContentRect.position);
        }
        else if (detailImage.width < detailImage.height || detailImage.width == detailImage.height)
        {
            //this is a tall image, scroll vertically
            //ratio = dH / dW;
            tScrollRect.vertical = true;
            tScrollRect.horizontal = false;

            tContentRect.anchorMin = new Vector2(0.5f, 1f);
            tContentRect.anchorMax = new Vector2(0.5f, 1f);
            tContentRect.pivot = new Vector2(0.5f, 1f);

            tContentObj.GetComponent<AspectRatioFitter>().aspectRatio = ratio;

            //tContentRect.sizeDelta = new Vector2(tObj.GetComponent<RectTransform>().rect.width, ratio * tObj.GetComponent<RectTransform>().rect.width);
        }
    }

    public void DestroyInfoScreen()
    {
        if (infoScreenIsShowing)
        {
            Destroy(clone);
            Destroy(tObj);
            infoScreenIsShowing = false;
            playerObject.GetComponent<FirstPersonController>().frozen = false;
        }

    }
}

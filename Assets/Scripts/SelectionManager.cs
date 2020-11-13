using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Transform selection;
    private Transform _selection;

    public GameObject testObject;
    private GameObject clone;

    private bool _active = true;

    public bool active
    {
        get {
            return _active;
        }
        set
        {
            _active = value;
        }
    }

    void Update()
    {
        if (active)
        {
            //If anything was selected on the last frame, deselect it and set _selection to null
            if (_selection != null)
            {
                if (_selection.gameObject.GetComponent<GalleryItem>())
                {
                    _selection.gameObject.GetComponent<GalleryItem>().isSelected = false;
                }
                _selection = null;
            }

            //Cast a ray, and if it hits an object on the Selectable layer, select that object
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 8;
            if (Physics.Raycast(ray, out hit, 3, layerMask))
            {
                selection = hit.transform;
                if (selection.gameObject.GetComponent<GalleryItem>())
                {
                    selection.gameObject.GetComponent<GalleryItem>().isSelected = true;
                }
                _selection = selection;
            }
        }
    }

    private void LateUpdate()
    {
            if (Input.GetMouseButtonDown(0))
            {
                if (selection == _selection && selection != null && selection.GetComponent<GalleryItem>().infoScreenIsShowing == false)
                {
                    selection.GetComponent<GalleryItem>().BuildInfoScreen();
                    active = false;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

            }

            if (Input.GetKey(KeyCode.X))
            {
                if (selection == _selection && selection != null && selection.GetComponent<GalleryItem>().infoScreenIsShowing == true)
                {
                    selection.GetComponent<GalleryItem>().DestroyInfoScreen();
                    active = true;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
    }
}

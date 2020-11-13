using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResetter : MonoBehaviour
{
    public Transform theTransform;

    void Update()
    {
        if(Input.GetKeyDown("r"))
        {
            this.GetComponent<Transform>().position = theTransform.position;
            this.GetComponent<Transform>().rotation = theTransform.rotation;
        }
    }
}

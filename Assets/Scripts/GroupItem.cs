using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupItem : GalleryItem
{
    public string groupName = "Group Name";
    public string groupMembers = "Group Members";
    public string groupDescription = "Group Description";

    public override void PopulateInfoScreenText()
    {
        tObj = GameObject.Find("GroupName");
        tObj.GetComponent<Text>().text = groupName;

        tObj = GameObject.Find("GroupMembers");
        if (groupMembers != "")
        {
            tObj.GetComponent<Text>().text = groupMembers + "\n";
        } else
        {
            tObj.GetComponent<Text>().text = groupMembers;
        }

        tObj = GameObject.Find("GroupDescription");

        if (this.GetComponent<Text>() != null)
        {
            groupDescription = this.GetComponent<Text>().text;
        }

        tObj.GetComponent<Text>().text = groupDescription;
    }
}

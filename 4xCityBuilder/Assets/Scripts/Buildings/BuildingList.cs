using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingList
{
    public string owner, domain;
    public List<BuildingObj> buildings = new List<BuildingObj>();


    public BuildingList(string owner, string domain)
    {

        // Set the owner and domain for this resource stock.  These will probably need to be things other than strings later
        this.owner = owner;
        this.domain = domain;

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The definition of a single resource
public class ResourceDef
{

    public string name, description, industry, skill;
    public List<string> types;
    int tier;
    public bool hasQuality = true;

    // Constructor
    public ResourceDef(string csvLine)
    {

        types = new List<string>();

        Debug.Log(csvLine);
        string[] values = csvLine.Split(',');

        //Debug.Log("Parsed into " + values.Length + " values");

        // Eventually need these to be loaded better somehow
        name = values[4];
        industry = values[0];
        skill = values[1];

        int t;

        types.Add(values[2]);
        if (!Int32.TryParse(values[5], out t))
            Debug.Log("Failed conversion of " + values[5] + " to integer");
        tier = t;

        if (values[4].Length > 1)
        {
            types.Add(values[3]);
        }
    }


}

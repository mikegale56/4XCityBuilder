using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The definition of a single resource
public class ResourceDef
{

    public string name, description, industry, category;
    public List<string> types;
    public List<int> tiers;
    public bool hasQuality = true;

    // Constructor
    public ResourceDef(string csvLine)
    {

        types = new List<string>();
        tiers = new List<int>();

        //Debug.Log(csvLine);
        string[] values = csvLine.Split(',');

        //Debug.Log("Parsed into " + values.Length + " values");

        // Eventually need these to be loaded better somehow
        name = values[6];
        industry = values[0];
        category = values[1];

        int t;

        types.Add(values[2]);
        if (!Int32.TryParse(values[3], out t))
            Debug.Log("Failed conversion of " + values[3] + " to integer");
        tiers.Add(t);

        if (values[4].Length > 0)
        {
            types.Add(values[4]);
            if (!Int32.TryParse(values[5], out t))
                Debug.Log("Failed conversion of " + values[5] + " to integer");
            tiers.Add(t);
        }
    }


}

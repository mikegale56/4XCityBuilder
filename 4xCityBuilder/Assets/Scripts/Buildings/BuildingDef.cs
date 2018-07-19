using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The definition of a single resource
public class BuildingDef
{

    public int tier;
    public string name, description;
    public string parentName;
    public List<string> industry, skill;
    //public List<BuildingPrerequisites> buildingPrerequisites;
    //public List<JobDef> jobsEnabled;
    //public List<Bonus> baseBonuses;
    //public Dictionary<string, int> jobMaxTier;
    public int maxWorkers;
    public int maxHp;
    public int housing;
    public int maintenanceCost;
    public int defaultMaxDistToWorkTiles;

    // Constructor
    public BuildingDef(List<string> csvLines, Dictionary<string,int> column)
    {

        foreach (string line in csvLines)
        {
            string[] values = line.Split(',');

            // Eventually need these to be loaded better somehow
            /*name = values[4];
            industry = values[0];
            skill = values[1];


            types.Add(values[2]);

            if (values[3].Length > 1)
                types.Add(values[3]);

            int t;
            if (!Int32.TryParse(values[5], out t))
                Debug.Log("Failed conversion of " + values[5] + " to integer");
            tier = t;*/
        }
    }


}

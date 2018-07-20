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
    public List<ResourceTypeQuantityQuality> resourcesToBuild;
    public int maxWorkers;
    public int maxHp;
    public int housing;
    public int maintenanceCost;
    public int defaultMaxDistToWorkTiles;
    public Sprite image;

    // Constructor
    public BuildingDef(List<string> csvLines, Dictionary<string,int> column)
    {

        resourcesToBuild = new List<ResourceTypeQuantityQuality>();
        industry = new List<string>();
        skill = new List<string>();

        foreach (string line in csvLines)
        {
            string[] values = line.Split(',');

            // Current keys into column
            //Debug.Log(line);

            // Name
            if (values[column["Name"]].Length > 0)
                name = values[column["Name"]];

            // Parent Name
            if (values[column["Parent Name"]].Length > 0)
                parentName = values[column["Parent Name"]];

            // Tier
            if (values[column["Tier"]].Length > 0)
                if (!Int32.TryParse(values[column["Tier"]], out tier))
                    Debug.Log("Cannot Parse Building Tier");

            // Description
            if (values[column["Description"]].Length > 0)
                description = values[column["Description"]];

            // Resource Type to Build
            if (values[column["Resource Type to Build"]].Length > 0)
            {
                int c, t;
                if (!Int32.TryParse(values[column["Resource Count"]], out c))
                    Debug.Log("Cannot Parse Resource Count \"" + values[column["Resource Count"]] + "\"");
                if (!Int32.TryParse(values[column["Min Resource Tier"]], out t))
                    Debug.Log("Cannot Parse Min Resource Tier \"" + values[column["Min Resource Tier"]] + "\"");

                ResourceTypeQuantityQuality tqq = new ResourceTypeQuantityQuality(
                    values[column["Resource Type to Build"]], QualityEnum.any, c, t);
            }

            // Max Workers
            if (values[column["Max Workers"]].Length > 0)
                if (!Int32.TryParse(values[column["Max Workers"]], out maxWorkers))
                    Debug.Log("Cannot Parse Max Workers");

            // Max HP
            if (values[column["Max HP"]].Length > 0)
                if (!Int32.TryParse(values[column["Max HP"]], out maxHp))
                    Debug.Log("Cannot Parse Max HP");

            // Housing
            if (values[column["Housing"]].Length > 0)
                if (!Int32.TryParse(values[column["Housing"]], out housing))
            Debug.Log("Cannot Parse Housing");

            // Housing
            if (values[column["Maintenance Cost"]].Length > 0)
                if (!Int32.TryParse(values[column["Maintenance Cost"]], out maintenanceCost))
                    Debug.Log("Cannot Parse Maintenance Cost");

            // Housing
            if (values[column["Default Max Dist"]].Length > 0)
                if (!Int32.TryParse(values[column["Default Max Dist"]], out defaultMaxDistToWorkTiles))
                    Debug.Log("Cannot Parse Default Max Dist");

            //Industries,Skills
            if (values[column["Industries"]].Length > 0)
                industry.Add(values[column["Industries"]]);
            if (values[column["Skills"]].Length > 0)
                skill.Add(values[column["Skills"]]);

            if (image == null)
            { 
                Texture2D tex;
                if (values[column["Image"]].Length == 0)
                    tex = (Resources.Load("Textures/NeedIcon") as Texture2D);
                else
                    tex = (Resources.Load(values[column["Image"]]) as Texture2D);
                image = Sprite.Create(tex,
                            new Rect(0, 0, tex.width, tex.height),
                            new Vector2(0.5f, 0.5f), tex.width);
            }
        }
    }
}

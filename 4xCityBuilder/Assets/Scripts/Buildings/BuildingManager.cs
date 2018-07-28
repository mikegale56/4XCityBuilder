using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SLS.Widgets.Table;
using System.Linq;

public class BuildingManager : MonoBehaviour {

    public Canvas buildingUiCanvas;
    public BuildingUI buildingUI;
    public Dictionary<string, int> buildingNameToDefIndexDictionary;
    public List<string> buildingCategories;
    public List<BuildingDef> buildingDefinitions;
    public BuildingList domainBuildings;
    public JobManager jobManager;
    //public BuildingUI buildingUI;


    // Use this for initialization
    void Awake()
    {

        buildingDefinitions = new List<BuildingDef>();
        buildingNameToDefIndexDictionary = new Dictionary<string, int>();
        string m_Path = Application.dataPath;
        List<string> lines = new List<string>();
        using (var reader = new StreamReader(m_Path + "/Definitions/Buildings.csv"))
        {
            // Parse the header
            Dictionary<string, int> column = ParseHeader(reader.ReadLine());
            while (!reader.EndOfStream)
            {
                string newLine = reader.ReadLine();

                if (lines.Count > 0 && !(newLine[0] == ','))
                {
                    buildingDefinitions.Add(new BuildingDef(lines, column));
                    buildingNameToDefIndexDictionary.Add(buildingDefinitions[buildingDefinitions.Count - 1].name, buildingDefinitions.Count - 1);
                    lines.Clear();
                }
                lines.Add(newLine);
            }

        }
        // Initialize the domain's building list
        domainBuildings = new BuildingList("Aster", "Aster");

        buildingUI.buildingNameSpriteDict = new Dictionary<string, Sprite>();
        foreach (BuildingDef bd in buildingDefinitions)
            buildingUI.buildingNameSpriteDict.Add(bd.name, bd.image);

        buildingUI.enabled = false;
        buildingUiCanvas.enabled = false;

        // Temp: add some fake bldgs

        for (int i = 0; i<buildingDefinitions.Count; i++)
            domainBuildings.buildings.Add(new BuildingObj(new Vector2Int(40, 40+i), buildingDefinitions[i], QualityEnum.normal, jobManager));

        buildingCategories = new List<string>();
        foreach (BuildingDef def in buildingDefinitions)
            buildingCategories.Add(def.category);
        buildingCategories = buildingCategories.Distinct().ToList();
    }

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update () {
		
	}

    Dictionary<string, int> ParseHeader(string header)
    {
        Dictionary<string, int> column = new Dictionary<string, int>();
        //Name,Tier,Description,Resource Type to Build,Min Resource Tier,Resource Count,Parent Name,Prerequisite To Build,Job Name,Job Max Tier,Bonus To,Bonus Value,Max HP,Housing,Max Workers,Industries,Skills,Maintenance Cost,Default Max Dist

        string[] values = header.Split(',');
        int ind = 0;
        foreach (string val in values)
            column.Add(val, ind++);

        return column;
    }

}
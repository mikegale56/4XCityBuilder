using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SLS.Widgets.Table;

public class BuildingManager : MonoBehaviour {

    public Canvas buildingUiCanvas;
    public List<BuildingDef> buildingDefinitions;
    public BuildingList domainBuildings;
    //public BuildingUI buildingUI;


    // Use this for initialization
    void Awake()
    {

        buildingDefinitions = new List<BuildingDef>();
        string m_Path = Application.dataPath;
        List<string> lines = new List<string>();
        using (var reader = new StreamReader(m_Path + "/Definitions/Buildings.csv"))
        {
            // Parse the header
            Dictionary<string, int> column = ParseHeader(reader.ReadLine());
            while (!reader.EndOfStream)
            {
                string newLine = reader.ReadLine();

                if (lines.Count > 0 && !lines[0].Equals(","))
                {
                    buildingDefinitions.Add(new BuildingDef(lines, column));
                    Debug.Log("Reading in building " + buildingDefinitions[buildingDefinitions.Count - 1].name);
                    lines.Clear();
                }
                lines.Add(newLine);
            }

        }
        // Initialize the domain's building list
        domainBuildings = new BuildingList("Aster", "Aster");

        //buildingUI.domainResources = buildingUI;
        //buildingUI.enabled = false;
        //buildingUiCanvas.enabled = false;
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
        //Name,Tier,Description,Resource Category to Build, Min Resource Tier, Resource Count to Build,Parent Name, Prerequisite To Build, Job Name,Job Max Tier,Bonus To, Bonus Value,Max HP, Housing, Max Workers,Industries,Skills,Maintenance Cost, Default Max Dist

        return column;
    }

}
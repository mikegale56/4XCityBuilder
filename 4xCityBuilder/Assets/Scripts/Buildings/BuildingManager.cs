using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SLS.Widgets.Table;
using System.Linq;
using UnityEngine.Tilemaps;

public class BuildingManager : ManagerBase {

    public Canvas buildingUiCanvas;
    public BuildingUI buildingUI;
    public List<string> buildingCategories;
    
    // Use this for initialization
    void Awake()
    {
        buildingDefinitions = new List<BuildingDef>();
        ManagerBase.buildingIndexOf = new Dictionary<string, int>();
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
                    ManagerBase.buildingIndexOf.Add(buildingDefinitions[buildingDefinitions.Count - 1].name, buildingDefinitions.Count - 1);
                    lines.Clear();
                }
                lines.Add(newLine);
            }

        }
        // Initialize the domain's building list
        if (ManagerBase.domain == null)
            ManagerBase.domain = new Domain();
        ManagerBase.domain.buildings = new List<BuildingObj>();

        buildingUI.buildingNameSpriteDict = new Dictionary<string, Sprite>();
        foreach (BuildingDef bd in buildingDefinitions)
            buildingUI.buildingNameSpriteDict.Add(bd.name, bd.image);

        buildingUI.enabled = false;
        buildingUiCanvas.enabled = false;

        // Temp: add some fake bldgs

        for (int i = 0; i<buildingDefinitions.Count; i++)
            ManagerBase.domain.buildings.Add(new BuildingObj(new Vector2Int(40, 40+i), buildingDefinitions[i], QualityEnum.normal));

        buildingCategories = new List<string>();
        foreach (BuildingDef def in buildingDefinitions)
            buildingCategories.Add(def.category);
        buildingCategories = buildingCategories.Distinct().ToList();

        // Add the buildings as surface tiles
        foreach (BuildingDef def in buildingDefinitions)
        {
            Tile newTile = ScriptableObject.CreateInstance<Tile>();
            newTile.sprite = def.image;
            surfaceTiles.Add(newTile);
            surfaceValueDictionary.Add(def.name, (short)(surfaceTiles.Count - 1));
        }
        
        //Update the surface tile

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
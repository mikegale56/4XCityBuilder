using UnityEngine;
using SLS.Widgets.Table;
using System.Collections.Generic;

public class BuildingUI : MonoBehaviour
{

    public Table buildingTable;
    public Table jobTable;
    public BuildingManager buildingManager;
    public Dictionary<string, Sprite> buildingNameSpriteDict;

    void Start()
    {

        buildingTable.ResetTable();

        // Add the columns
        buildingTable.AddImageColumn(""); // Icon
        buildingTable.AddTextColumn("Name");
        buildingTable.AddTextColumn("Tier");
        buildingTable.AddTextColumn("Quality");
        buildingTable.AddTextColumn("Maintenance");
        //buildingTable.AddImageColumn(""); // HP Bar
        buildingTable.AddTextColumn("Workers");
        buildingTable.AddTextColumn("Active Jobs");
        // Initialize Your Table
        buildingTable.Initialize(onTableSelected, buildingNameSpriteDict);

        // Populate Rows 
        int ind = 0;
        foreach (BuildingObj building in ManagerBase.domain.buildings)
        {
            Datum d = Datum.Body(ind.ToString());
            ind++;

            d.elements.Add(building.name);
            d.elements.Add(building.name);
            d.elements.Add(ManagerBase.buildingDefinitions[ManagerBase.buildingIndexOf[building.name]].tier);
            d.elements.Add(building.quality.ToString());
            d.elements.Add(ManagerBase.buildingDefinitions[ManagerBase.buildingIndexOf[building.name]].maintenanceCost);
            //d.elements.Add(""); // HP Bar
            d.elements.Add(building.NumberOfWorkers().ToString());
            d.elements.Add(building.NumberOfActiveJobs().ToString());

            buildingTable.data.Add(d);
        }

        // Draw Table
        buildingTable.StartRenderEngine();

    }

    // Handle the row selection however you wish
    private void onTableSelected(Datum datum)
    {
        
    }

}
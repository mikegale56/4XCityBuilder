using UnityEngine;
using SLS.Widgets.Table;
using System.Collections.Generic;

public class BuildingUI : MonoBehaviour
{

    public Table buildingTable;
    public BuildingList domainBuildings;

    void Start()
    {

        buildingTable.ResetTable();

        // Add the columns
        buildingTable.AddImageColumn(" "); // Icon
        buildingTable.AddTextColumn("Name");
        buildingTable.AddTextColumn("Tier");
        buildingTable.AddTextColumn("Quality");
        buildingTable.AddTextColumn("Maintenance");
        buildingTable.AddImageColumn(" "); // HP Bar
        buildingTable.AddTextColumn("Workers");
        buildingTable.AddTextColumn("Active Jobs");
       // Initialize Your Table
        buildingTable.Initialize(onTableSelected);

        // Populate Rows 
        int ind = 0;
        foreach (BuildingObj building in domainBuildings.buildings)
        {
            Datum d = Datum.Body(ind.ToString());
            ind++;

            //print("Printing Name");
            d.elements.Add(building.name);
            
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
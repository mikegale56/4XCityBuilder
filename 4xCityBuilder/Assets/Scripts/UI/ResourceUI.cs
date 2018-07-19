using UnityEngine;
using SLS.Widgets.Table;
using System.Collections.Generic;

public class ResourceUI : MonoBehaviour
{

    public Table resourceTable;
    //public ResourceStock domainResources;
    public ResourceManager resourceManager;

    void Start()
    {

        resourceTable.ResetTable();

        // Add the columns
        // Add the name column
        //print("Adding Name Column");
        resourceTable.AddTextColumn("Resource");

        // Add in the quantity columns, one per quality
        for (int i = 0; i < (int)QualityEnum.any; i++)
        {
            //print("Adding Quality Column");
            resourceTable.AddTextColumn(((QualityEnum)i).ToString() + " quality", null);
        }
        

        // Initialize Your Table
        resourceTable.Initialize(onTableSelected);

        // Populate Your Rows (obviously this would be real data here)
        int ind = 0;
        foreach (KeyValuePair<string, int> entry in resourceManager.domainResources.nameToIndexDictionary)
        {
            Datum d = Datum.Body(ind.ToString());
            ind++;

            //print("Printing Name");
            d.elements.Add(entry.Key);
            int[] quantity = resourceManager.domainResources.quantity[entry.Value];
            foreach (int q in quantity)
            {
                //print("Printing Quantity");
                d.elements.Add(q.ToString());
            }
            resourceTable.data.Add(d);
        }

        // Draw Your Table
        resourceTable.StartRenderEngine();

    }

    // Handle the row selection however you wish
    private void onTableSelected(Datum datum)
    {
        
    }

}
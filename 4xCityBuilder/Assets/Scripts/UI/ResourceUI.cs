﻿using UnityEngine;
using SLS.Widgets.Table;
using System.Collections.Generic;

public class ResourceUI : MonoBehaviour
{

    public Table resourceTable;
    public Domain domain;

    public Dictionary<string, Sprite> resourceNameSpriteDict;

    void Start()
    {

        // Initialize Your Table
        resourceTable.ResetTable();

        // Add the columns
        Column c;
        c = resourceTable.AddImageColumn(" "); // Icon
        c.horAlignment = Column.HorAlignment.CENTER;

        resourceTable.AddTextColumn("Resource");

        // Add in the quantity columns, one per quality
        for (int i = 0; i < (int)QualityEnum.any; i++)
        {
            //print("Adding Quality Column");
            resourceTable.AddTextColumn(((QualityEnum)i).ToString() + " quality", null);
        }

        // Initialize Your Table
        resourceTable.Initialize(onTableSelected, resourceNameSpriteDict);

        // Populate Rows
        int ind = 0;
        foreach (KeyValuePair<string, int> entry in ManagerBase.resourceIndexOf)
        {
            Datum d = Datum.Body(ind.ToString());
            ind++;

            //print("Printing Name");
            d.elements.Add(entry.Key); // The name is the key into the sprite dictionary 
            d.elements.Add(entry.Key);
            int[] quantity = domain.stock.quantity[entry.Value];
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
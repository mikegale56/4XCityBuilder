using UnityEngine;
using SLS.Widgets.Table;

public class ResourceUI : MonoBehaviour
{

    public ResourceManager resourceManager;
    public Table resourceTable;

    void Start()
    {

        //resourceTable = GetComponent<Table>();

        resourceTable.ResetTable();

        resourceTable.AddTextColumn();
        resourceTable.AddTextColumn();
        resourceTable.AddTextColumn();

        // Initialize Your Table
        resourceTable.Initialize(onTableSelected);

        // Populate Your Rows (obviously this would be real data here)
        for (int i = 0; i < 100; i++)
        {
            Datum d = Datum.Body(i.ToString());
            d.elements.Add("Col1:Row" + i.ToString());
            d.elements.Add("Col2:Row" + i.ToString());
            d.elements.Add("Col3:Row" + i.ToString());
            resourceTable.data.Add(d);
        }

        // Draw Your Table
        resourceTable.StartRenderEngine();

    }

    // Handle the row selection however you wish
    private void onTableSelected(Datum datum)
    {
        print("You Clicked: " + datum.uid);
    }

}
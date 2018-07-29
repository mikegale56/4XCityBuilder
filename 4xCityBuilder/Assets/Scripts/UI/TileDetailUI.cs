using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public delegate void BuildingButtonCallback(string s);

public class TileDetailUI : MonoBehaviour {


    public RectTransform surfacePanel;
    public RectTransform spText;
    public RectTransform groundPanel;
    public RectTransform gpText;
    public RectTransform undergroundPanel;
    public RectTransform ugpText;
    public RectTransform specialPanel;
    public RectTransform sppText;
    public RectTransform jobsPanel;
    public RectTransform militaryPanel;
    public MapManager mapManager;
    public BuildingManager buildingManager;

    public BuildingDropdownCreator buildingDC;
    public ResourceDropdownCreator resourceDC;

    public BuildingButtonCallback bbcb;

    private Text buildingDescriptionText;

    public void FocusOnTile(int i, int j)
    {

        if (buildingDescriptionText != null)
            buildingDescriptionText.text = "";

        if (resourceDC.resourceDropdown != null)
            resourceDC.ClearResourceList();

        string newText;
        // Surface Panel
        if (mapManager.GetSurfaceValue(i, j) >= 0)
        {
            if (buildingDC.buildingDropdown != null)
                buildingDC.buildingDropdown.HideAll();
            short surfaceValue = mapManager.GetSurfaceValue(i, j);
            Tile surfaceTile = mapManager.surfaceTiles[surfaceValue];
            surfacePanel.GetComponent<Image>().sprite = surfaceTile.sprite;
            surfacePanel.GetComponent<Image>().type = Image.Type.Filled;
            surfacePanel.GetComponent<Image>().color = surfaceTile.color;
            Debug.Log(spText.GetComponent<Text>().text);
            // Create the new string
            newText = mapManager.surfaceValueDictionary.FirstOrDefault(x => x.Value == surfaceValue).Key;
            newText += "\n";
            newText += "No Description Yet";
            spText.GetComponent<Text>().text = newText;

            if (buildingDC.demolishDropdown == null)
                buildingDC.CreateDemolishDropdown(new Vector3(-468, -140), 300, 45);
            else
                buildingDC.demolishDropdown.ShowAll();

        } else
        {
            if (buildingDC.demolishDropdown != null)
                buildingDC.demolishDropdown.HideAll();
            surfacePanel.GetComponent<Image>().sprite = null;
            surfacePanel.GetComponent<Image>().color = Color.gray;
            spText.GetComponent<Text>().text = "Build a Structure:\n";

            if (buildingDC.buildingDropdown == null)
                buildingDC.CreateDropdown(new Vector3(-468, -70), 300, 45);
            else
                buildingDC.buildingDropdown.ShowAll();
        }

        // Ground Panel
        byte groundValue = mapManager.GetGroundValue(i, j);
        Tile groundTile = mapManager.groundTiles[groundValue];
        groundPanel.GetComponent<Image>().sprite = groundTile.sprite;
        groundPanel.GetComponent<Image>().type = Image.Type.Tiled;
        groundPanel.GetComponent<Image>().color = groundTile.color;
        newText = mapManager.groundValueDictionary.FirstOrDefault(x => x.Value == groundValue).Key;
        newText += "\n";
        newText += "No Description Yet\n";
        newText += "Worked by:";
        gpText.GetComponent<Text>().text = newText;

        // Underground Panel 
        byte ugv = mapManager.GetUndergroundValue(i, j);
        byte sv  = mapManager.GetStoneValue(i, j);
        Tile undergroundTile = mapManager.undergroundTiles[ugv][sv];
        undergroundPanel.GetComponent<Image>().sprite = undergroundTile.sprite;
        undergroundPanel.GetComponent<Image>().type = Image.Type.Tiled;
        undergroundPanel.GetComponent<Image>().color = undergroundTile.color;
        if (ugv == 0)
            newText = mapManager.stoneValueDictionary.FirstOrDefault(x => x.Value == sv).Key;
        else
            newText = mapManager.undergroundValueDictionary.FirstOrDefault(x => x.Value == ugv).Key;
        newText += "\n";
        newText += "No Description Yet\n";
        newText += "Worked by:";
        ugpText.GetComponent<Text>().text = newText;

    }

    public void ActionSelected(string s)
    {
        Debug.Log("Action Selected: " + s);

        BuildingDef defToBuild = buildingManager.buildingDefinitions[buildingManager.buildingNameToDefIndexDictionary[s]];

        if (defToBuild == null)
            Debug.LogError("Should not have an unbuildable building here: " + s);

        string txt = defToBuild.name + "\n" + defToBuild.description + "\n";
        if (buildingDescriptionText == null)
            buildingDescriptionText = NewTextBox(jobsPanel.transform, txt, new Vector3(180, -350), new Vector2(300, 100));
        else
            buildingDescriptionText.text = txt;
        // Add prerequisites, colored, here

        // Figure out what the building needs
        resourceDC.CreateResourceChoiceDropdown(new Vector3(-200, -485), defToBuild.resourcesToBuild);

        int ind = 0;
        foreach (DropdownBase db in resourceDC.resourceDropdown)
        {
            string quantityString = defToBuild.resourcesToBuild.rqqList[ind].quantity.ToString();
            Text temp = NewTextBox(db.thisGo.transform, quantityString, new Vector3(0, resourceDC.imageSize-20), new Vector2(resourceDC.imageSize, 20F));
            temp.alignment = TextAnchor.MiddleCenter;
            ind++;
        }

    }

    // Use this for initialization
    void Awake ()
    {
        buildingDC.bbcb = new BuildingButtonCallback(ActionSelected);
        resourceDC.resourceDropdown = null;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public static Text NewTextBox(Transform parent, string text, Vector3 localPosition, Vector2 size)
        {
        RectTransform rect = new GameObject().AddComponent<RectTransform>();
        rect.SetParent(parent);
        rect.name = "TileDetailTextBox";
        rect.gameObject.layer = 9;
        rect.localScale = new Vector3(1, 1, 1);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.localPosition = localPosition;
        rect.sizeDelta = size;
        Text t = rect.gameObject.AddComponent<Text>();
        t.text = text;
        t.fontSize = 16;
        t.color = Color.black;
        t.alignment = TextAnchor.MiddleLeft;
        t.gameObject.layer = 11;
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        return t;
    }
}

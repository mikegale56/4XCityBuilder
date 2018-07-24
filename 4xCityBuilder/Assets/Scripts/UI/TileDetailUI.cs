using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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

    public void FocusOnTile(int i, int j)
    {
        string newText;
        // Surface Panel
        if (mapManager.GetSurfaceValue(i, j) >= 0)
        {
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
        } else
        {
            surfacePanel.GetComponent<Image>().sprite = null;
            surfacePanel.GetComponent<Image>().color = Color.gray;
            spText.GetComponent<Text>().text = "No Surface Structure\n";
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

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

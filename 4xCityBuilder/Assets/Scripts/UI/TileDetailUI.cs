using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileDetailUI : MonoBehaviour {


    public RectTransform surfacePanel;
    public RectTransform groundPanel;
    public RectTransform undergroundPanel;
    public RectTransform specialPanel;
    public RectTransform jobsPanel;
    public RectTransform militaryPanel;
    public MapManager mapManager;

    public void FocusOnTile(int i, int j)
    {
        // Surface Panel
        //mapManager.GetSurfaceValue(i,j)
        //mapManager.surfaceTiles[mapManager.GetSurfaceValue(i, j)];

        // Ground Panel
        Tile groundTile = mapManager.groundTiles[mapManager.GetGroundValue(i, j)];
        groundPanel.GetComponent<Image>().sprite = groundTile.sprite;
        groundPanel.GetComponent<Image>().type = Image.Type.Tiled;
        groundPanel.GetComponent<Image>().color = groundTile.color;

        // Underground Panel 
        byte ugv = mapManager.GetUndergroundValue(i, j);
        byte sv  = mapManager.GetStoneValue(i, j);
        Tile undergroundTile = mapManager.undergroundTiles[ugv][sv];
        undergroundPanel.GetComponent<Image>().sprite = undergroundTile.sprite;
        undergroundPanel.GetComponent<Image>().type = Image.Type.Tiled;
        undergroundPanel.GetComponent<Image>().color = undergroundTile.color;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDetailManager : MonoBehaviour {

    public Canvas tileDetailCanvas;
    public TileDetailUI tileDetailUI;
    public MainUIManager mainUiManager;

    // Use this for initialization
    void Start ()
    {

        // Subscribe to tile change updates
        WorldEventHandler pmc = new WorldEventHandler(TileUpdated);
        WorldEventHandlerManager.AddListener(worldEventChannels.map, mapChannelEvents.change, pmc);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    public void TileUpdated(WorldEventArg we)
    {
        // If not on the tile detail panel, ignore this
        if (!mainUiManager.currentUi.Equals("TileDetail"))
            return;

        // Get i and j from e
        Vector3Int loc = we.location;
        int iLoc = loc.x;
        int jLoc = loc.y;

        // If looking at this tile, update this tile
        if (iLoc == tileDetailUI.iLoc && jLoc == tileDetailUI.jLoc)
        {
            tileDetailUI.FocusOnTile(iLoc, jLoc);
        }
    }
}

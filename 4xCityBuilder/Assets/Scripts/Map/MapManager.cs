using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;
using UnityEngine.UI;

public class MapManager : ManagerBase
{
	public MapGenerator mapGenerator;
	
	public Tilemap groundTileMap;
	public Tilemap undergroundTileMap;
	public Tilemap surfaceTileMap;

    private int N;

    void Start()
    {
        // Initialize the Domain and MapData
        if (domain == null)
            domain = new Domain();
        N = mapGenerator.GetN();
        domain.mapData = new MapData(N);
		
        // Load in the tiles
        LoadTiles();

        //Generate the map 
        mapGenerator.GenerateMap(domain.mapData);
		
		// Draw the ground & surface tiles
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                groundTileMap.SetTile(new Vector3Int(i, j, 0), groundTiles[domain.mapData.GetGroundValue(i, j)]);
                if (domain.mapData.GetSurfaceValue(i, j) >= 0)
                    surfaceTileMap.SetTile(new Vector3Int(i, j, 0), surfaceTiles[domain.mapData.GetSurfaceValue(i, j)]);                
            } 
        }
		
		// Draw the underground tiles
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                undergroundTileMap.SetTile(new Vector3Int(i, j, 0), undergroundTiles[domain.mapData.GetUndergroundValue(i, j)][domain.mapData.GetStoneValue(i,j)]);
            }
        }
        undergroundTileMap.GetComponent<Renderer>().enabled = false;

        WorldEventHandler pmc = new WorldEventHandler(ProcessMapChange);
        WorldEventHandlerManager.AddListener(worldEventChannels.map, mapChannelEvents.change, pmc);
    }

    public void ProcessMapChange(WorldEventArg we)
    {
        // Get i and j from e
        Vector3Int loc = we.location;
        int i = loc.x;
        int j = loc.y;

        // Delete the old tiles
        groundTileMap.SetTile(loc, null);
        surfaceTileMap.SetTile(loc, null);
        undergroundTileMap.SetTile(loc, null);

        // Draw the ground & surface tiles
        groundTileMap.SetTile(loc, groundTiles[domain.mapData.GetGroundValue(i, j)]);
        if (domain.mapData.GetSurfaceValue(i, j) >= 0)
            surfaceTileMap.SetTile(loc, surfaceTiles[domain.mapData.GetSurfaceValue(i, j)]);

        // Draw the underground tiles
        undergroundTileMap.SetTile(loc, undergroundTiles[domain.mapData.GetUndergroundValue(i, j)][domain.mapData.GetStoneValue(i, j)]);
    }

    private void LoadTiles()
    {

        // Eventually build an asset bundle
        // https://docs.unity3d.com/Manual/LoadingResourcesatRuntime.html

        // Ground Tiles
        groundTiles.Add(Resources.Load("Tiles/Ground/Ocean") as Tile);
        groundValueDictionary.Add("Ocean", (byte)(groundTiles.Count-1));

        groundTiles.Add(Resources.Load("Tiles/Ground/Water") as Tile);
        groundValueDictionary.Add("Water", (byte)(groundTiles.Count - 1));

        groundTiles.Add(Resources.Load("Tiles/Ground/Plain") as Tile);
        groundValueDictionary.Add("Plain", (byte)(groundTiles.Count - 1));

        groundTiles.Add(Resources.Load("Tiles/Ground/Hill") as Tile);
        groundValueDictionary.Add("Hill", (byte)(groundTiles.Count - 1));

        groundTiles.Add(Resources.Load("Tiles/Ground/Mountain") as Tile);
        groundValueDictionary.Add("Mountain", (byte)(groundTiles.Count - 1));

        groundTiles.Add(Resources.Load("Tiles/Ground/Beach") as Tile);
        groundValueDictionary.Add("Beach", (byte)(groundTiles.Count - 1));

        // Tree Tiles
        surfaceTiles.Add(Resources.Load("Tiles/Trees/Oak") as Tile);
        surfaceValueDictionary.Add("Oak", (byte)(surfaceTiles.Count - 1));

        surfaceTiles.Add(Resources.Load("Tiles/Trees/Pine") as Tile);
        surfaceValueDictionary.Add("Pine", (byte)(surfaceTiles.Count - 1));

        surfaceTiles.Add(Resources.Load("Tiles/Trees/TreeAsh") as Tile);
        surfaceValueDictionary.Add("Ash", (byte)(surfaceTiles.Count - 1));

        surfaceTiles.Add(Resources.Load("Tiles/Trees/TreeRedwood") as Tile);
        surfaceValueDictionary.Add("Redwood", (byte)(surfaceTiles.Count - 1));

        // Stone Tiles
        undergroundTiles.Add(new List<Tile>());
        undergroundTiles[0].Add(Resources.Load("Tiles/Stone/Sandstone") as Tile);
        stoneValueDictionary.Add("Sandstone", (byte)(undergroundTiles[0].Count - 1));

        undergroundTiles[0].Add(Resources.Load("Tiles/Stone/Limestone") as Tile);
        stoneValueDictionary.Add("Limestone", (byte)(undergroundTiles[0].Count - 1));

        undergroundTiles[0].Add(Resources.Load("Tiles/Stone/Marble") as Tile);
        stoneValueDictionary.Add("Marble", (byte)(undergroundTiles[0].Count - 1));

        undergroundTiles[0].Add(Resources.Load("Tiles/Stone/Granite") as Tile);
        stoneValueDictionary.Add("Granite", (byte)(undergroundTiles[0].Count - 1));

        CreateOreTiles cot = new CreateOreTiles();
        cot.RockOreMix(undergroundValueDictionary, stoneValueDictionary, undergroundTiles);
     
    }

}
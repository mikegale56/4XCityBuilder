using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;

public class MapManager : MonoBehaviour
{

    [Range(4, 11)]
    public int nFac;

    // Terrain
    [Range(0.0F, 1.0F)]
    public float waterFraction = 0.20F;
    [Range(0.0F, 1.0F)]
    public float hillFraction = 0.15F;
    [Range(0.0F, 1.0F)]
    public float mountainFraction = 0.05F;
    [Range(0, 1000)]
    public int nRivers = 50;

    // Trees
    [Range(0.0F, 1.0F)]
    public float oakTreeFraction = 0.1F;
    [Range(0.0F, 1.0F)]
    public float pineTreeFraction = 0.15F;

    // Stone
    [Range(0.0F, 1.0F)]
    public float sandstoneFrac = 0.4F;
    [Range(0.0F, 1.0F)]
    public float limestoneFrac = 0.3F;
    [Range(0.0F, 1.0F)]
    public float marbleFrac = 0.2F;
    [Range(0.0F, 1.0F)]
    public float graniteFrac = 0.1F;

    public Dictionary<string, byte> groundValueDictionary = new Dictionary<string, byte>();
    public Dictionary<string, byte> stoneValueDictionary = new Dictionary<string, byte>();
    public Dictionary<string, byte> undergroundValueDictionary = new Dictionary<string, byte>();
    public Dictionary<string, byte> specialValueDictionary = new Dictionary<string, byte>();
    public Dictionary<string, short> surfaceValueDictionary = new Dictionary<string, short>();

    private float[,] height;
    private byte[,]  groundValue;
    private byte[,]  undergroundValue;
    private byte[,]  stoneValue;
    private byte[,]  specialValue;
    private short[,] surfaceValue;

    public Tilemap groundTileMap;
    public Tilemap undergroundTileMap;
    public List<Tile> groundTiles = new List<Tile>();
    public List<Tile> surfaceTiles = new List<Tile>();
    public List<List<Tile>> undergroundTiles = new List<List<Tile>>();

    // Use this for initialization
    void Start()
    {

        // Load in the tiles
        LoadTiles();

        //Choose to generate vs load


        //Generate the map 
        GenerateMap();
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

    private void GenerateMap()
    {
        int N = (int)(Mathf.Pow(2.0F, nFac) + 1.0F);
        height           = new float[N, N];
        groundValue      = new byte[N, N];
        surfaceValue     = new short[N, N];
        undergroundValue = new byte[N, N];
        stoneValue       = new byte[N, N];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                undergroundValue[i, j] = 0;
                stoneValue[i, j] = 99; //Everything stone will be replaced
            }
        }

        MapGenFunctions mapGenFunctions = new MapGenFunctions();
        
        /*
         *Generate the ground and surface map
        */

        // Generate the height map
        mapGenFunctions.GenWithDiamondSquare(N, height);

        // Turn the height map into a terrain map
        mapGenFunctions.HeightToTerrain(N, height, groundValue, this);

        // Turn water into Ocean if it touches the edge
        mapGenFunctions.WaterToOcean(groundValue, N, this);

        // Add rivers
        mapGenFunctions.AddRivers(N, nRivers, height, groundValue, this);

        // Add coast
        mapGenFunctions.AddCoasts(N, groundValue, this);

        // Initialize the surface values
        mapGenFunctions.InitSurfaceValues(N, surfaceValue);

        // Generate Trees
        mapGenFunctions.PlaceTrees(N, oakTreeFraction, surfaceValueDictionary["Oak"], surfaceValue, groundValue, this);
        mapGenFunctions.PlaceTrees(N, pineTreeFraction, surfaceValueDictionary["Pine"], surfaceValue, groundValue, this);

        // Draw the ground & surface tiles
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (surfaceValue[i, j] >= 0)
                {
                    //Debug.Log(surfaceTiles[0].sprite);
                    groundTileMap.SetTile(new Vector3Int(i, j, 0), surfaceTiles[surfaceValue[i, j]]);
                }
                else
                    groundTileMap.SetTile(new Vector3Int(i, j, 0), groundTiles[groundValue[i, j]]);
            }
        }
        // Hide this
        groundTileMap.GetComponent<Renderer>().enabled = false;

        // Generate the underground map

        // Divide into stones
        mapGenFunctions.GenerateStone(stoneValue, stoneValueDictionary, N,
            sandstoneFrac, limestoneFrac, marbleFrac, graniteFrac);

        // Add ores
        mapGenFunctions.AddOreVein(undergroundValue, undergroundValueDictionary["CopperOre"], N, 0.50F, "snake");
        mapGenFunctions.AddOreVein(undergroundValue, undergroundValueDictionary["TinOre"],    N, 0.25F, "snake");
        mapGenFunctions.AddOreVein(undergroundValue, undergroundValueDictionary["SilverOre"], N, 0.20F, "snake");
        mapGenFunctions.AddOreVein(undergroundValue, undergroundValueDictionary["IronOre"],   N, 0.25F, "snake");
        mapGenFunctions.AddOreVein(undergroundValue, undergroundValueDictionary["GoldOre"],   N, 0.10F, "snake");
        //mapGenFunctions.AddOreVein(undergroundValue, undergroundValueDictionary["CoalOre"], N, 0.5F, "circle");


        // Draw the underground tiles
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                //Debug.Log(undergroundValue[i, j].ToString() + ", " + stoneValue[i, j].ToString());
                undergroundTileMap.SetTile(new Vector3Int(i, j, 0), undergroundTiles[undergroundValue[i, j]][stoneValue[i,j]]);
            }
        }
        //undergroundTileMap.enabled = false;


    }

    /* TIME A FUNCTION
    DateTime before = DateTime.Now;
    <function>
    DateTime after = DateTime.Now;
    TimeSpan duration = after.Subtract(before);
    Debug.Log("Duration in milliseconds: " + duration.Milliseconds);
    */

    // Need a function to combine a tile from surface tiles and ground tiles to create a merged one
    // Maintain a tuple map thing into it
    //https://answers.unity.com/questions/915606/combining-2-textures-into-one.html

    //worldTiles.SetTile(new Vector3Int(i, j, 0), groundTiles[groundValue[i, j]]);

    /*public void GenerateTextures()
    {
        https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
        rend = GetComponent<Renderer>();
        noiseTex = new Texture2D(pixWidth, pixHeight);

    }*/


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class MapManager : MonoBehaviour
{

    [Range(4, 11)]
    public int nFac;

    [Range(0.0F, 1.0F)]
    public float waterFraction = 0.20F;

    [Range(0.0F, 1.0F)]
    public float hillFraction = 0.15F;

    [Range(0.0F, 1.0F)]
    public float mountainFraction = 0.05F;

    [Range(0, 1000)]
    public int nRivers = 50;

    public Dictionary<string, byte> groundValueDictionary = new Dictionary<string, byte>();
    public Dictionary<string, byte> undergroundValueDictionary = new Dictionary<string, byte>();
    public Dictionary<string, byte> specialValueDictionary = new Dictionary<string, byte>();
    public Dictionary<string, uint> surfaceValueDictionary = new Dictionary<string, uint>();

    private float[,] height;
    private byte[,] groundValue;
    private byte[,] undergroundValue;
    private byte[,] specialValue;
    private uint[,] surfaceValue;

    public Tilemap worldTiles;
    public List<Tile> groundTiles = new List<Tile>();

    private Noise noiseGen = new Noise();

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


        //Debug.Log(groundValueDictionary["Water"].ToString());
        //Debug.Log(groundTiles.Count.ToString());


    }

    private void GenerateMap()
    {

        int N = (int)(Mathf.Pow(2.0F, nFac) + 1.0F);
        height = new float[N, N];
        groundValue = new byte[N, N];

        MapGenFunctions mapGenFunctions = new MapGenFunctions();

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

        // Add trees
        //Tile treeTile = ScriptableObject.CreateInstance<Tile>();
        //var treeSprite = Resources.Load("Sprites/trees/Tree1") as Sprite;
        //treeTile.sprite = treeSprite;

        // Draw the tiles

        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                worldTiles.SetTile(new Vector3Int(i, j, 0), groundTiles[groundValue[i, j]]);
                
    }

    //worldTiles.SetTile(new Vector3Int(i, j, 0), groundTiles[groundValue[i, j]]);

    /*public void GenerateTextures()
    {
        https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
        rend = GetComponent<Renderer>();
        noiseTex = new Texture2D(pixWidth, pixHeight);

    }*/


}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ManagerBase : MonoBehaviour
{
    // Every manager can access all the information about the domain
    public static Domain domain;
	
	// And all of the definitions
	public static List<ResourceDef> resourceDefinitions;
	public static Dictionary<string, int> resourceIndexOf;
    public static List<BuildingDef> buildingDefinitions;
	public static Dictionary<string, int> buildingIndexOf;
    public static List<JobDef> jobDefinitions;
	
	// Including the tile definitions
	public static List<Tile> groundTiles = new List<Tile>();
    public static List<Tile> surfaceTiles = new List<Tile>();
    public static List<List<Tile>> undergroundTiles = new List<List<Tile>>();
    public static Dictionary<string, byte> groundValueDictionary = new Dictionary<string, byte>();
    public static Dictionary<string, byte> stoneValueDictionary = new Dictionary<string, byte>();
    public static Dictionary<string, byte> undergroundValueDictionary = new Dictionary<string, byte>();
    public static Dictionary<string, byte> specialValueDictionary = new Dictionary<string, byte>();
    public static Dictionary<string, short> surfaceValueDictionary = new Dictionary<string, short>();
	
}
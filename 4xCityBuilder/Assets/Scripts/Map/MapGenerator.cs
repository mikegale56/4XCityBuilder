using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;

public class MapGenerator : MonoBehaviour
{

    [Range(4, 11)]
    public int nFac;
	
	private int N;

    // Terrain
    [Range(0.0F, 1.0F)]
    public float waterFraction = 0.20F;
    [Range(0.0F, 1.0F)]
    public float hillFraction = 0.15F;
    [Range(0.0F, 1.0F)]
    public float mountainFraction = 0.05F;
    [Range(0, 1000)]
    public int nRivers = 5;

    // Trees
    [Range(0.0F, 1.0F)]
    public float oakTreeFraction = 0.1F;
    [Range(0.0F, 1.0F)]
    public float pineTreeFraction = 0.15F;
    [Range(0.0F, 1.0F)]
    public float ashTreeFraction = 0.075F;
    [Range(0.0F, 1.0F)]
    public float redwoodTreeFraction = 0.05F;

    // Stone
    [Range(0.0F, 1.0F)]
    public float sandstoneFrac = 0.4F;
    [Range(0.0F, 1.0F)]
    public float limestoneFrac = 0.3F;
    [Range(0.0F, 1.0F)]
    public float marbleFrac = 0.2F;
    [Range(0.0F, 1.0F)]
    public float graniteFrac = 0.1F;

    private int edgeWater;

    private Noise noiseGen = new Noise();
    public float[] perlinFraction = new float[20];
    public float[] perlinBound = new float[20];
    
    public void Awake()
	{
		N = (int)(Mathf.Pow(2.0F, nFac) + 1.0F);
        perlinFraction[0] = 0;
        perlinFraction[1] = 1; // 0.05 - 0.1
        perlinFraction[2] = 2;
        perlinFraction[3] = 3; // 0.15 - 0.2
        perlinFraction[4] = 4;
        perlinFraction[5] = 5; // 0.3
        perlinFraction[6] = 5;
        perlinFraction[7] = 6; // 0.4
        perlinFraction[8] = 8;
        perlinFraction[9] = 8; // 0.45 - 0.5
        perlinFraction[10] = 8;
        perlinFraction[11] = 8; // 0.6
        perlinFraction[12] = 8;
        perlinFraction[13] = 7; // 0.7
        perlinFraction[14] = 6;
        perlinFraction[15] = 5; // 0.8
        perlinFraction[16] = 4;
        perlinFraction[17] = 2; // 0.85 - 0.9
        perlinFraction[18] = 1;
        perlinFraction[19] = 0; // 0.95 - 1.0

        for (int i = 0; i < perlinBound.Length; i++)
            perlinBound[i] = 0.05F * (i + 1);
    }
	public int GetN()
	{ return N;}
    
	public void GenerateMap(MapData mapData)
    {

		// Generate the height map
        float[,] height = GenWithDiamondSquare();

		// Ground
        // Turn the height map into a terrain map
        byte[,] groundValue = HeightToTerrain(height);

        // Turn water into Ocean if it touches the edge
        WaterToOcean(groundValue);

        // Add rivers
        AddRivers(nRivers, height, groundValue);

        // Add coast
        AddCoasts(groundValue);
		
		// Set the values
		mapData.SetGround(groundValue);

		// Surface
		short[,] surfaceValue = new short[N,N];
		for (int i=0; i<N; i++)
			for (int j=0; j<N; j++)
				surfaceValue[i,j] = -1; // No structure is -1
		// Generate Trees
        PlaceTrees(oakTreeFraction, ManagerBase.surfaceValueDictionary["Oak"], groundValue, surfaceValue);
        PlaceTrees(pineTreeFraction, ManagerBase.surfaceValueDictionary["Pine"], groundValue, surfaceValue);
        PlaceTrees(ashTreeFraction, ManagerBase.surfaceValueDictionary["Ash"], groundValue, surfaceValue);
        PlaceTrees(redwoodTreeFraction, ManagerBase.surfaceValueDictionary["Redwood"], groundValue, surfaceValue);
		
		// Set the values
		mapData.SetSurface(surfaceValue);

        // Underground
		byte[,] undergroundValue = new byte[N,N];
        byte[,] stoneValue = new byte[N, N];
        for (int i=0; i<N; i++)
			for (int j=0; j<N; j++)
			{
				undergroundValue[i,j] = 0; // No ore is 0
				stoneValue[i,j] = 99; // Will all be replaced
			}
        // Divide into stones
        GenerateStone(stoneValue, ManagerBase.stoneValueDictionary);

        // Add ores
        AddOreVein(undergroundValue, ManagerBase.undergroundValueDictionary["CopperOre"], N, 0.50F, "snake");
        AddOreVein(undergroundValue, ManagerBase.undergroundValueDictionary["TinOre"],    N, 0.25F, "snake");
        AddOreVein(undergroundValue, ManagerBase.undergroundValueDictionary["SilverOre"], N, 0.20F, "snake");
        AddOreVein(undergroundValue, ManagerBase.undergroundValueDictionary["IronOre"],   N, 0.25F, "snake");
        AddOreVein(undergroundValue, ManagerBase.undergroundValueDictionary["GoldOre"],   N, 0.10F, "snake");
        //AddOreVein(undergroundValue, undergroundValueDictionary["CoalOre"], N, 0.5F, "circle");
		
		// Set the values
		mapData.SetUnderground(undergroundValue);
		mapData.SetStone(stoneValue);

    }
	
	public float[,] GenWithDiamondSquare()
    {
		float[,] height = new float[N,N];
		
        float dsFac = N / 4F;
        float dsFacOriginal = dsFac;

        int i = 0;
        int j = 0;

        for (i = 0; i < N; i++)
            for (j = 0; j < N; j++)
                height[i, j] = 0.0F;

        //Enforced water on the edges of the map, this wide
        edgeWater = (int)Mathf.Floor((N - 1) / Mathf.Pow(2F, 6F));
        for (i = 0; i < edgeWater; i++)
        {
            for (j = 0; j < N; j++)
            {
                height[j, i]         = -dsFacOriginal;
                height[j, N - i - 1] = -dsFacOriginal;
                height[i, j]         = -dsFacOriginal;
                height[N - i - 1, j] = -dsFacOriginal;
            }
        }

        // Modified Diamond Square Method
        int delta = N - 1;
        i = 0;
        j = 0;
        while (delta >= 1)
        {
            // Diamond Step
            i = delta;
            while (i < N - delta)
            {
                j = delta;
                while (j < N - delta)
                {
                    if (height[i, j] == 0.0F)
                    {
                        float r = UnityEngine.Random.Range(0.0F, 1.0F);
                        height[i, j] = (height[i - delta, j - delta] + height[i - delta, j + delta] + 
                            height[i + delta, j - delta] + height[i + delta, j + delta]) / 4 + 
                            dsFac * (r - 0.5F * (1 - dsFac / dsFacOriginal));
                    }
                    j = j + 2 * delta;
                }
                i = i + 2 * delta;
            }
                
            // Square Step
            i = 0;
            while (i < N)
            {
                j = 0;
                while (j < N)
                {
                    if (height[i, j] == 0.0F)
                    {
                        float s = 0;
                        int ns = 0;
                        if (i - delta >= 0)
                        {
                            s = s + height[i - delta, j];
                            ns = ns + 1;
                        }
                        if (i + delta < N)
                        {
                            s = s + height[i + delta, j];
                            ns = ns + 1;
                        }
                        if (j - delta >= 1)
                        {
                            s = s + height[i, j - delta];
                            ns = ns + 1;
                        }
                        if (j + delta < N)
                        {
                            s = s + height[i, j + delta];
                            ns = ns + 1;
                        }
                        float r = UnityEngine.Random.Range(0.0F, 1.0F);
                        height[i, j] = s / ns + dsFac * (r - 0.25F);
                    }
                    j = j + delta;
                }
                i = i + delta;
            }

            // Reduce delta, fac
            delta = delta / 2;
            dsFac = dsFac / 1.75F;
        }
        return height;

    }

    public byte[,] HeightToTerrain(float[,] height)
    {
		byte[,] groundValue = new byte[N,N];
        // Put the heights into a List, sort it, and find the percent markers that way
        List<float> sortedHeight = new List<float>();
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                sortedHeight.Add(height[i, j]);
        sortedHeight.Sort();

        // Get the height values at each of the percentages
        float waterValue    = sortedHeight[Mathf.RoundToInt(N * N * waterFraction)];
        float hillValue     = sortedHeight[Mathf.RoundToInt(N * N * (1-hillFraction-mountainFraction))];
        float mountainValue = sortedHeight[Mathf.RoundToInt(N * N * (1-mountainFraction))];

        // Assign ground values:
        // 0 = ocean
        // 1 = water
        // 2 = plain
        // 3 = hill
        // 4 = mountain
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (height[i, j] <= waterValue)
                    groundValue[i, j] = ManagerBase.groundValueDictionary["Water"];
                else if (height[i, j] >= mountainValue)
                    groundValue[i, j] = ManagerBase.groundValueDictionary["Mountain"];
                else if (height[i, j] >= hillValue)
                    groundValue[i, j] = ManagerBase.groundValueDictionary["Hill"];
                else
                    groundValue[i, j] = ManagerBase.groundValueDictionary["Plain"];
            }
        }


        //Init the edge water to ocean, all but the last entry
        for (int i = 0; i < edgeWater-1; i++)
        {
            for (int j = 0; j < N; j++)
            {
                groundValue[j, i] = ManagerBase.groundValueDictionary["Ocean"];
                groundValue[j, N - i - 1] = ManagerBase.groundValueDictionary["Ocean"]; ;
                groundValue[i, j] = ManagerBase.groundValueDictionary["Ocean"]; ;
                groundValue[N - i - 1, j] = ManagerBase.groundValueDictionary["Ocean"]; ;
            }
        }

		return groundValue;
    }

    public void WaterToOcean(byte[,] groundValue)
    {
        Stack<int> iValues = new Stack<int>();
        Stack<int> jValues = new Stack<int>();
        iValues.Push(edgeWater);
        jValues.Push(edgeWater);
        while (iValues.Count > 0)
        {
            //Debug.Log("i = " + iValues.Peek().ToString() + "; j = " + jValues.Peek().ToString());
            SpreadValueToAdjacent(groundValue, N, iValues.Pop(), jValues.Pop(), 
                ManagerBase.groundValueDictionary["Water"], 
                ManagerBase.groundValueDictionary["Ocean"], iValues, jValues);
        }

    }

    public void SpreadValueToAdjacent(byte[,] groundValue, int N, int startI, int startJ, byte fromValue, byte toValue, Stack<int> iValues, Stack<int> jValues)
    {
        groundValue[startI, startJ] = toValue;
        if (startI + 1 < N)
        {
            if (groundValue[startI + 1, startJ] == fromValue)
            {
                iValues.Push(startI + 1);
                jValues.Push(startJ);
                groundValue[startI + 1, startJ] = toValue;
            }
        }
        if (startI - 1 > 0)
        {
            if (groundValue[startI - 1, startJ] == fromValue)
            {
                iValues.Push(startI - 1);
                jValues.Push(startJ);
                groundValue[startI - 1, startJ] = toValue;
            }
        }
        if (startJ + 1 < N)
        {
            if (groundValue[startI, startJ + 1] == fromValue)
            {
                iValues.Push(startI);
                jValues.Push(startJ + 1);
                groundValue[startI, startJ + 1] = toValue;
            }
        }
        if (startJ - 1 > 0)
        {
            if (groundValue[startI, startJ - 1] == fromValue)
            {
                iValues.Push(startI);
                jValues.Push(startJ - 1);
                groundValue[startI, startJ - 1] = toValue;
            }
        }
    }

    public void AddRivers(int nRivers, float[,] height, byte[,] groundValue)
    {
        int M = 8; // Number of destinations along each coast
        int nDestinationOptions = 4 * M - 4;
        float[,] destinationOptions = new float[nDestinationOptions, 3];
        IdentifyRiverDestinations(destinationOptions, N, M, height, groundValue, ManagerBase.groundValueDictionary);

        // Calculate the gradients ahead of time
        float[,] iGrad = new float[N-1,N];
        float[,] jGrad = new float[N,N-1];
        for (int i = 0; i < N - 1; i++)
            for (int j = 0; j < N; j++)
                iGrad[i, j] = height[i + 1, j] - height[i, j];
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N-1; j++)
                jGrad[i, j] = height[i, j + 1] - height[i, j];

        // Initialize a river volume for each tile
        float[,] riverVolume = new float[N, N];
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                riverVolume[i, j] = 0;

        // Create nRivers rivers
        for (int i = 0; i < nRivers; i++)
        {
            // Pick an origin
            int currI = UnityEngine.Random.Range(edgeWater + 1, N - edgeWater);
            int currJ = UnityEngine.Random.Range(edgeWater + 1, N - edgeWater);

            //Debug.Log("River Origin @ [" + currI.ToString() + ", " + currJ.ToString() + "]");

            // Find the closest destination
            float[] distances = new float[nDestinationOptions];
            float minDistance = N*N;
            int minIndex = 0;
            for (int j=0; j < nDestinationOptions; j++)
            {
                distances[j] = Mathf.Sqrt(Mathf.Pow(currI - destinationOptions[j, 0], 2.0F) + Mathf.Pow(currJ - destinationOptions[j, 1], 2.0F));
                //Debug.Log("Distance[" + j.ToString() + "] = " + distances[j].ToString());
                if (distances[j] < minDistance)
                {
                    minDistance = distances[j];
                    minIndex = j;
                    //Debug.Log("Update Min Distance");
                }
            }

            //Debug.Log("River Destination @ [" + destinationOptions[minIndex, 0].ToString() + ", " + destinationOptions[minIndex, 1].ToString() + "]");

            //Generate the river
            RiverCreator(currI, currJ, (int)destinationOptions[minIndex, 0], (int)destinationOptions[minIndex, 1], 
                iGrad, jGrad, groundValue, riverVolume, ManagerBase.groundValueDictionary);
        }

        // Spread the river
        // TO DO

        // Convert the terrain
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                if (riverVolume[i, j] > 0)
                    groundValue[i, j] = ManagerBase.groundValueDictionary["Water"];

    }

    public void IdentifyRiverDestinations(float[,] destinationOptions, int N, int M, float[,] height, byte[,] groundValue, Dictionary<string, byte> groundValueDictionary)
    {

        // Initialize destinationOptions[i, 2], the height of the point, to something high
        for (int i = 0; i < (4 * M - 4); i++)
        {
            destinationOptions[i, 0] = -10000;
            destinationOptions[i, 1] = -10000;
            destinationOptions[i, 2] = 10000;
        }

        // Get the distance from the edge that rivers should go to
        int binSizes = (N - 1) / M;
        int index = 0;
        // Iterate over an MxM grid
        BoundsInt mBounds = new BoundsInt(0, 0, 0, M, M, 1);
        foreach (var p in mBounds.allPositionsWithin)
        {
            // If this is on the edge of the map
            if (p.x == 0 || p.y == 0 || p.x == M || p.y == M)
            {
                index += 1;  // Increment the counter
                for (int i = p.x * binSizes; i < (p.x + 1) * binSizes - 1; i++)
                {
                    // Don't count the hard-coded edge water locations
                    if (i < edgeWater || i > (N-edgeWater))
                        continue;
                    // Loop over all points in the box
                    for (int j = p.y * binSizes; j < (p.y + 1) * binSizes - 1; j++)
                    {
                        // Don't count the hard-coded edge water locations
                        if (j < edgeWater || j > (N - edgeWater))
                            continue;
                        if (height[i, j] < destinationOptions[index, 2] && //Spot is lower
                            (groundValue[i, j] == groundValueDictionary["Ocean"] | groundValue[i, j] == groundValueDictionary["Water"])) //and is water
                        {
                            destinationOptions[index, 0] = i;
                            destinationOptions[index, 1] = j;
                            destinationOptions[index, 2] = height[i, j];
                        }
                    }
                }
            }
        }
    }

    public void GetGradients(float[] thisIGrad, float[] thisJGrad, int currI, int currJ, float[,] riverVolume, float[,] iGrad, float[,] jGrad)
    {
        thisIGrad[0] = -iGrad[currI - 1, currJ] + riverVolume[currI - 1, currJ];
        thisIGrad[1] =  iGrad[currI, currJ]     + riverVolume[currI + 1, currJ];
        thisJGrad[0] = -jGrad[currI, currJ - 1] + riverVolume[currI, currJ - 1];
        thisJGrad[1] =  jGrad[currI, currJ]     + riverVolume[currI, currJ + 1];
    }
    
    public void RiverCreator(int currI, int currJ, int destI, int destJ, float[,] iGrad, float[,] jGrad, byte[,] groundValue, float[,] riverVolume, Dictionary<string, byte> groundValueDictionary)
    {
        float thisRiverVolume = 1.0F;
        float evaporation = thisRiverVolume / 2000;
        long nUpdates = 0;

        while (groundValue[currI, currJ] != groundValueDictionary["Ocean"] &  // Not ocean
            thisRiverVolume > 0.1 &             // Not out of water
            !(currI == destI & currJ == destJ))  // Not at the destination
        {
            nUpdates += 1;
            // If this is a new river tile, add evaporation
            if (riverVolume[currI,currJ] == 0)
                thisRiverVolume -= evaporation;

            // Add flow volume to the tile
            riverVolume[currI, currJ] += thisRiverVolume;

            // Get the gradients
            float[] thisIGrad = new float[2];
            float[] thisJGrad = new float[2];
            GetGradients(thisIGrad, thisJGrad, currI, currJ, riverVolume, iGrad, jGrad);

            // Check for the direction of flow
            if (riverVolume[currI - 1, currJ] == 0 && thisIGrad[0] <= thisRiverVolume &&
                thisIGrad[0] <= thisIGrad[1] && thisIGrad[0] <= thisJGrad[0] && thisIGrad[0] <= thisJGrad[1])
                currI = currI - 1;
            else if (riverVolume[currI + 1, currJ] == 0 && thisIGrad[1] <= thisRiverVolume &&
                thisIGrad[1] <= thisJGrad[0] && thisIGrad[1] <= thisJGrad[1])
                currI = currI + 1;
            else if (riverVolume[currI, currJ - 1] == 0 && thisJGrad[0] <= thisRiverVolume &&
                thisJGrad[0] <= thisJGrad[1])
                currJ = currJ - 1;
            else if (riverVolume[currI, currJ + 1] == 0 && thisJGrad[1] <= thisRiverVolume)
                currJ = currJ + 1;
            else
            {
                // Propogate it towards the destination
                float deltaI = destI - currI;
                float deltaJ = destJ - currJ;

                if (Mathf.Abs(deltaI) >= Mathf.Abs(deltaJ))
                    currI = currI + (int)Mathf.Sign(deltaI);
                else
                    currJ = currJ + (int)Mathf.Sign(deltaJ);
            }
            if (nUpdates > 50000)
                break;
        }
        /*if (groundValue[currI, currJ] == 0)
            Debug.Log("River Complete (Reached Ocean) at " + nUpdates.ToString() + " updates");
        else if (thisRiverVolume < 0.1)
            Debug.Log("River Complete (Volume) at " + nUpdates.ToString() + " updates");
        else if (!(currI == destI & currJ == destJ))
            Debug.Log("River Complete (Reached Destination) at " + nUpdates.ToString() + " updates");*/

    }

    public void AddCoasts(byte[,] groundValue)
    {
        // Look at all points
        for (int i = 1; i < N-1; i++)
        {
            for (int j = 1; j < N-1; j++)
            {
                // If this is plains
                if (groundValue[i, j] == ManagerBase.groundValueDictionary["Plain"])
                {
                    // If water is adjacent and this is regular ground, make it coast
                    if (groundValue[i + 1, j] == ManagerBase.groundValueDictionary["Water"] |
                        groundValue[i + 1, j] == ManagerBase.groundValueDictionary["Ocean"])
                        groundValue[i, j] = 5;
                    if (groundValue[i - 1, j] == ManagerBase.groundValueDictionary["Water"] |
                        groundValue[i - 1, j] == ManagerBase.groundValueDictionary["Ocean"])
                        groundValue[i, j] = 5;
                    if (groundValue[i, j + 1] == ManagerBase.groundValueDictionary["Water"] |
                        groundValue[i, j + 1] == ManagerBase.groundValueDictionary["Ocean"])
                        groundValue[i, j] = 5;
                    if (groundValue[i, j - 1] == ManagerBase.groundValueDictionary["Water"] |
                        groundValue[i, j - 1] == ManagerBase.groundValueDictionary["Ocean"])
                        groundValue[i, j] = 5;
                }
            }
        }
                
    }

    public void PlaceTrees(float frac, short value, byte[,] groundValue, short[,] surfaceValue)
    {

        // Turn frac into a value based on the distribution of Perlin Noise set in the constructor
        float actualFrac = 0;
        int ind = perlinFraction.Length-1;
        while (ind >= 0 & actualFrac <= frac)
        {
            actualFrac = actualFrac + perlinFraction[ind]/100F;
            ind--;
        }
        float thisBound = perlinBound[ind+1];
        //Debug.Log("Perlin Noise Bound = " + thisBound.ToString());
        
        for (int i = edgeWater; i < N-edgeWater; i++)
        {
            for (int j = edgeWater; j < N- edgeWater; j++)
            {
                float noise = noiseGen.GetNoise((double)i / (double)N * 8, (double)j / (double)N * 8, value);
                if (noise >= thisBound && // Noise is over the threshold
                    (groundValue[i,j] == ManagerBase.groundValueDictionary["Plain"] | // And it's on a plain
                    groundValue[i, j] == ManagerBase.groundValueDictionary["Hill"]))  // Or hills
                    surfaceValue[i, j] = value;
            }
        }

    }

    public void AddOreVein(byte[,] undergroundValue, byte toValue, int N, float fracOfDomainsWithOre, string snakeOrCircle)
    {
        // Frac is "this fraction of domains have this resource"
        // Use that to figure out how many ore deposits to have
        float prob = 0;
        float p, nk;
        float n = 0;
        while (prob < fracOfDomainsWithOre)
        {
            p = Mathf.PI * 100 * 100 / N / N;
            n = n + 1;
            prob = 0;
            for (int k = 1; k < Mathf.Min(n,4); k++)
            {
                nk = MyFactorial((int)n) / (MyFactorial(k) * MyFactorial((int)n - k));
                prob = prob + nk * Mathf.Pow(p, k) * Mathf.Pow(1 - p, n - k);
            }
            if (n > 20) break;
        }
        //Debug.Log("number of veins selected as " + n.ToString() + " with probability of being in a domain of >" + prob.ToString());
        int numberOfVeins = Mathf.RoundToInt(n);
        // Generate either a snaking ore or a circle of ore
        float branchProbability;
        int travelLength;
        if (snakeOrCircle == "snake") // Snake
        {
            branchProbability = 0.01F;
            travelLength = 150;
            // .01, 100-> ~108 ores
            // ***.01, 150-> ~225 ores ***
            // .01, 200-> ~400 ores
            // .01, 300-> ~1300 ores
            // .01, 400-> ~3300 ores
            // .02, 100-> ~200 ores
            // .02, 200-> ~1560 ores
            // .03, 100-> ~375 ores
        } else // Circle
        {
            branchProbability = 0.1F;
            travelLength = 25;
            // .05, 50-> ~135 ores per
            // .08, 25-> ~92 ores per
            // .08, 50-> ~230 ores per
            // ***.10, 25-> ~100 ores per***
        }

        // Generate this many veins
        for (int i = 0; i < numberOfVeins; i++)
        {
            GenerateOreVeins.CreateVein(new Vector2Int(UnityEngine.Random.Range(0,N-1), UnityEngine.Random.Range(0, N - 1)),
                PoissonRandomGenerator.GetPoisson(travelLength), undergroundValue, toValue, branchProbability, N);
        }
    }

    public void GenerateStone(byte[,] stoneValue, Dictionary<string, byte> stoneValueDictionary)
    {
        // Generate this many divider lines
        int numberOfDividers = 10 * Mathf.CeilToInt(N / Mathf.Pow(2, 6));
        for (int i = 0; i < numberOfDividers; i++)
        {
            float theta = UnityEngine.Random.Range(0.0F, 2.0F * Mathf.PI);
            Vector2 aimDir = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            //Debug.Log((theta * 180 / Mathf.PI).ToString());
            GenerateOreVeins.CreateVein(new Vector2Int(UnityEngine.Random.Range(0, N - 1), UnityEngine.Random.Range(0, N - 1)),
                100 * N, stoneValue, 100, 0.0F, N, aimDir);
        }

        // Fill in the holes (val 99) with a random type of stone between the divider lines (val 100)
        float roll;
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (stoneValue[i, j] == 99)
                {
                    // Figure out the type of stone
                    byte stone;
                    roll = UnityEngine.Random.Range(0.0F, 1.0F);
                    if (roll <= sandstoneFrac)
                        stone = stoneValueDictionary["Sandstone"];
                    else if (roll <= (sandstoneFrac + limestoneFrac))
                        stone = stoneValueDictionary["Limestone"];
                    else if (roll <= (sandstoneFrac + limestoneFrac + marbleFrac))
                        stone = stoneValueDictionary["Marble"];
                    else
                        stone = stoneValueDictionary["Granite"];
                    EmptyToStone(i, j, stoneValue, N, stone);
                }

            }
        }

        // Fill in the dividers (val 100) with the last type of stone seen
        byte lastStone = 0;
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                if (stoneValue[i, j] == 100)
                    stoneValue[i, j] = lastStone;
                else
                    lastStone = stoneValue[i, j];

    }

    public void EmptyToStone(int i, int j, byte[,] stoneValue, int N, byte toValue)
    {
        Stack<int> iValues = new Stack<int>();
        Stack<int> jValues = new Stack<int>();
        iValues.Push(i);
        jValues.Push(j);
        while (iValues.Count > 0)
            SpreadValueToAdjacent(stoneValue, N, iValues.Pop(), jValues.Pop(), 99, toValue, iValues, jValues);
    }

    public int MyFactorial(int number)
    {
        if (number <= 1)
            return 1;
        else
            return number * MyFactorial(number - 1);
    }
}
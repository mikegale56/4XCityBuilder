﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenFunctions
{

    public int edgeWater;

	// Use this for initialization
	public void GenWithDiamondSquare(int N, float[,] height)
    {
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
                        float r = Random.Range(0.0F, 1.0F);
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
                        float r = Random.Range(0.0F, 1.0F);
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

    }

    public void HeightToTerrain(int N, float[,] height, byte[,] groundValue, MapManager mapManager)
    {
        float waterFraction    = mapManager.waterFraction;
        float hillFraction     = mapManager.hillFraction;
        float mountainFraction = mapManager.mountainFraction;
        float tolerance = 100 * 1 / N;

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
                    groundValue[i, j] = mapManager.groundValueDictionary["Water"];
                else if (height[i, j] >= mountainValue)
                    groundValue[i, j] = mapManager.groundValueDictionary["Mountain"];
                else if (height[i, j] >= hillValue)
                    groundValue[i, j] = mapManager.groundValueDictionary["Hill"];
                else
                    groundValue[i, j] = mapManager.groundValueDictionary["Plain"];
            }
        }


        //Init the edge water to ocean, all but the last entry
        for (int i = 0; i < edgeWater-1; i++)
        {
            for (int j = 0; j < N; j++)
            {
                groundValue[j, i] = mapManager.groundValueDictionary["Ocean"];
                groundValue[j, N - i - 1] = mapManager.groundValueDictionary["Ocean"]; ;
                groundValue[i, j] = mapManager.groundValueDictionary["Ocean"]; ;
                groundValue[N - i - 1, j] = mapManager.groundValueDictionary["Ocean"]; ;
            }
        }


    }

    public void WaterToOcean(byte[,] groundValue, int N, MapManager mapManager)
    {
        Stack<int> iValues = new Stack<int>();
        Stack<int> jValues = new Stack<int>();
        iValues.Push(edgeWater);
        jValues.Push(edgeWater);
        while (iValues.Count > 0)
        {
            //Debug.Log("i = " + iValues.Peek().ToString() + "; j = " + jValues.Peek().ToString());
            SpreadValueToAdjacent(groundValue, N, iValues.Pop(), jValues.Pop(), 
                mapManager.groundValueDictionary["Water"], 
                mapManager.groundValueDictionary["Ocean"], iValues, jValues);
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

    public void AddRivers(int N, int nRivers, float[,] height, byte[,] groundValue, MapManager mapManager)
    {
        int M = 8; // Number of destinations along each coast
        int nDestinationOptions = 4 * M - 4;
        float[,] destinationOptions = new float[nDestinationOptions, 3];
        IdentifyRiverDestinations(destinationOptions, N, M, height, groundValue, mapManager.groundValueDictionary);

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
            int currI = Random.Range(edgeWater + 1, N - edgeWater);
            int currJ = Random.Range(edgeWater + 1, N - edgeWater);

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
                iGrad, jGrad, groundValue, riverVolume, mapManager.groundValueDictionary);
        }

        // Spread the river
        // TO DO

        // Convert the terrain
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                if (riverVolume[i, j] > 0)
                    groundValue[i, j] = mapManager.groundValueDictionary["Water"];

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

    public void AddCoasts(int N, byte[,] groundValue, MapManager mapManager)
    {
        // Look at all points
        for (int i = 1; i < N-1; i++)
        {
            for (int j = 1; j < N-1; j++)
            {
                // If this is plains
                if (groundValue[i, j] == mapManager.groundValueDictionary["Plain"])
                {
                    // If water is adjacent and this is regular ground, make it coast
                    if (groundValue[i + 1, j] == mapManager.groundValueDictionary["Water"] |
                        groundValue[i + 1, j] == mapManager.groundValueDictionary["Ocean"])
                        groundValue[i, j] = 5;
                    if (groundValue[i - 1, j] == mapManager.groundValueDictionary["Water"] |
                        groundValue[i - 1, j] == mapManager.groundValueDictionary["Ocean"])
                        groundValue[i, j] = 5;
                    if (groundValue[i, j + 1] == mapManager.groundValueDictionary["Water"] |
                        groundValue[i, j + 1] == mapManager.groundValueDictionary["Ocean"])
                        groundValue[i, j] = 5;
                    if (groundValue[i, j - 1] == mapManager.groundValueDictionary["Water"] |
                        groundValue[i, j - 1] == mapManager.groundValueDictionary["Ocean"])
                        groundValue[i, j] = 5;
                }
            }
        }
                
    }


        public void GeneratePerlinNoise()
    {
        /*
        float maxH = 0;
        float minH = 1;

        List<float> allHeights = new List<float>();

        float perlinFac = (float)N / 32.0F;

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                float noise = noiseGen.GetNoise((double)i / (double)N * 8, (double)j / (double)N * 8, 0.0);
                //float noise = Mathf.PerlinNoise((float)(i / perlinFac), (float)(j / perlinFac));
                allHeights.Add(noise);
                height[i, j] = noise;
                if (noise > maxH) maxH = noise;
                if (noise < minH) minH = noise;
            }
        }
        
        */
    }
	
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStock
{
    public Dictionary<string, int> nameToIndexDictionary = new Dictionary<string, int>();
    public Dictionary<string, List<int>> typeToIndexDictionary = new Dictionary<string, List<int>>();
    public List<int[]> quantity = new List<int[]>();
    public Dictionary<QualityEnum, float> qualityMultiplier;
    public bool[] canConsume;

    public ResourceStock(List<ResourceDef> resourceDefinitions)
    {

        // Copy the quality multiplier list
        this.qualityMultiplier = ResourceManager.qualityMultiplier;

        // Number of qualities
        int enumLength = Enum.GetNames(typeof(QualityEnum)).Length - 1; //-1 because of the any

        // Loop over each resource and set the stock to 0
        foreach (var resource in resourceDefinitions)
        {
            int ind = quantity.Count;
            quantity.Add(new int[enumLength]);
            nameToIndexDictionary.Add(resource.name, ind);
            for (int i = 0; i < enumLength; i++)
                quantity[ind][i] = 0;

            // Add the type as a key
            foreach (string type in resource.types)
            {
                if (typeToIndexDictionary.ContainsKey(type))
                {
                    // This type exists as a key, add this index to the list
                    typeToIndexDictionary[type].Add(ind);
                }
                else
                {
                    // This type has not been seen yet, add a new key
                    List<int> indices = new List<int>();
                    indices.Add(ind);
                    typeToIndexDictionary[type] = indices;
                }
            }

        }
        canConsume = new bool[quantity.Count];
        for (int i = 0; i < quantity.Count; i++)
            canConsume[i] = true;
    }
}
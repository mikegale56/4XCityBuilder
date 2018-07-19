using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStock
{
    public string owner, domain;
    public Dictionary<string, int> nameToIndexDictionary = new Dictionary<string, int>();
    public Dictionary<string, List<int>> typeToIndexDictionary = new Dictionary<string, List<int>>();
    public List<int[]> quantity = new List<int[]>();
    public Dictionary<QualityEnum, float> qualityMultiplier;
    public bool[] canConsume;

    public ResourceStock(string owner, string domain, List<ResourceDef> resourceDefinitions)
    {

        // Copy the quality multiplier list
        this.qualityMultiplier = ResourceManager.qualityMultiplier;

        // Set the owner and domain for this resource stock.  These will probably need to be things other than strings later
        this.owner = owner;
        this.domain = domain;

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

    // Function to Add resources to the stock by name
    public void AddResources(List<ResourceExactChange> nameQualityQuantity)
    {
        foreach (var nqq in nameQualityQuantity)
        {
            if (nqq.quality == QualityEnum.any)
                Debug.LogError("Error: no quality specified when adding resources to the stock");
            quantity[nameToIndexDictionary[nqq.name]][(int)nqq.quality] += nqq.quantity;
        }
    }

    // Check if there are enough resources of the chosen name, and, optionally, quality
    public bool[] CheckResources(List<ResourceExactChange> nameQualityQuantity)
    {
        bool[] hasEnough = new bool[nameQualityQuantity.Count];
        // Loop over all queries
        int ind = 0;
        foreach (var nqq in nameQualityQuantity)
        {
            int numInStock = 0;

            // if quality is not selected
            if (nqq.quality == QualityEnum.any)
            {
                // Check for any quality
                foreach (QualityEnum qVal in Enum.GetValues(typeof(QualityEnum)))
                {
                    if (qVal == QualityEnum.any) // Skip "any"
                        continue;
                    else
                        numInStock += quantity[nameToIndexDictionary[nqq.name]][(int)qVal];
                }
            }
            else // Get the number of that specific quality
                numInStock = quantity[nameToIndexDictionary[nqq.name]][(int)nqq.quality];

            hasEnough[ind] = numInStock >= nqq.quantity;
            ind++;
        }
        return hasEnough;
    }

    // Check if there are enough resources of the chosen name, and, optionally, quality
    public bool[] CheckResources(List<ResourceTypeChange> tyoeQualityQuantity)
    {
        bool[] hasEnough = new bool[tyoeQualityQuantity.Count];
        // Loop over all queries
        int ind = 0;
        foreach (var tqq in tyoeQualityQuantity)
        {
            int numInStock = 0;

            // Find the resource indices with this type and loop over them
            foreach (int index in typeToIndexDictionary[tqq.type])
            {
                // if quality is not selected
                if (tqq.quality == QualityEnum.any)
                {
                    // Check for any quality
                    foreach (QualityEnum qVal in Enum.GetValues(typeof(QualityEnum)))
                    {
                        if (qVal == QualityEnum.any) // Skip "any"
                            continue;
                        else
                            numInStock += quantity[index][(int)qVal];
                    }
                }
                else // Get the number of that specific quality
                    numInStock = quantity[index][(int)tqq.quality];
            }
            hasEnough[ind] = numInStock >= tqq.quantity;
            ind++;
        }
        return hasEnough;
    }

    // Remove resources of a certain quantity and quality
    public float[] RemoveResources(List<ResourceExactChange> nameQualityQuantity)
    {

        float[] averageQualityMultiplierOfRemoved = new float[nameQualityQuantity.Count];
        int ind = 0;
        foreach (var nqq in nameQualityQuantity)
        {
            int leftToRemove = nqq.quantity;
            float totalMultOfRemoved = 0;

            // If quality is not selected
            if (nqq.quality == QualityEnum.any)
            {
                int numEnumValues = Enum.GetValues(typeof(QualityEnum)).Length;

                // Start at highest quality, which is numEnumValues-1, as the highest is "any"
                int qVal = numEnumValues - 1;
                while (leftToRemove > 0) // Keep removing until there are none left to remove
                {
                    // How many are there in this quality bin?
                    int quant = quantity[nameToIndexDictionary[nqq.name]][qVal];
                    if (quant >= leftToRemove) // If there are more than needed
                    {
                        // Remove them
                        quantity[nameToIndexDictionary[nqq.name]][qVal] -= leftToRemove;
                        // Add to the average quality
                        totalMultOfRemoved += leftToRemove * qualityMultiplier[(QualityEnum)qVal];
                        // Set the number to remove to zero                           
                        leftToRemove = 0;
                    } else // There are not enough
                    {
                        // Remove what is left
                        int numRemoved = quantity[nameToIndexDictionary[nqq.name]][qVal];
                        quantity[nameToIndexDictionary[nqq.name]][qVal] = 0;
                        // Add to the average quality
                        totalMultOfRemoved += numRemoved * qualityMultiplier[(QualityEnum)qVal];
                        // Decrement the number left to remove
                        leftToRemove -= numRemoved;
                    }
                    qVal++;
                    if (qVal >= (int)QualityEnum.any)
                    {
                        Debug.LogError("Cannot remove enough of a resource from stock - this shouldn't happen, did I forget a check");
                        break;
                    }
                }
                averageQualityMultiplierOfRemoved[ind] = totalMultOfRemoved / nqq.quantity;
            }
            else
            {
                // How many are there in this quality bin?
                int quant = quantity[nameToIndexDictionary[nqq.name]][(int)nqq.quality];
                if (quant >= leftToRemove) // If there are more than needed
                {
                    quantity[nameToIndexDictionary[nqq.name]][(int)nqq.quality] -= nqq.quantity;
                    averageQualityMultiplierOfRemoved[ind] = qualityMultiplier[nqq.quality];
                } else
                    Debug.LogError("Cannot remove enough of a resource from stock - this shouldn't happen, did I forget a check");
            }

            ind++;
        }
        return averageQualityMultiplierOfRemoved;
    }
}

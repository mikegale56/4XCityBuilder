using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceTypeQuantityQuality : ResourceQuantityQuality
{
    public string type;

    public ResourceTypeQuantityQuality(string type, QualityEnum quality, int quantity)
    {
        this.type = type;
        this.quantity = quantity;
        this.quality = quality;
    }

    public ResourceTypeQuantityQuality(string type, QualityEnum quality, int quantity, int minTier)
    {
        this.type = type;
        this.quantity = quantity;
        this.quality = quality;
        this.minTier = minTier;
    }

    public override void AddResource(ResourceStock stock)
    {
        Debug.LogError("Cannot add a ResourceTypeQuantityQuality to a stock");
    }

    public override bool CheckResource(ResourceStock stock)
    {
        int numInStock = 0;

        // Find the resource indices with this type and loop over them
        foreach (int index in stock.typeToIndexDictionary[type])
        {
            // if quality is not selected
            if (quality == QualityEnum.any)
            {
                // Check for any quality
                foreach (QualityEnum qVal in Enum.GetValues(typeof(QualityEnum)))
                {
                    if (qVal == QualityEnum.any) // Skip "any"
                        continue;
                    else
                        numInStock += stock.quantity[index][(int)qVal];
                }
            }
            else // Get the number of that specific quality
                numInStock = stock.quantity[index][(int)quality];
        }
        return numInStock >= quantity;
    }

    public override float CheckResourceQuality(ResourceStock stock)
    {
        Debug.LogError("Cannot check the quality of a ResourceTypeQuantityQuality with a float output yet");
        return 0.0F;
    }

    public override float RemoveResource(ResourceStock stock)
    {
        Debug.LogError("Cannot remove a ResourceTypeQuantityQuality");
        return 0.0F;
    }

    public override Dictionary<string, Sprite> GetImageOptions(ResourceManager resourceManager)
    {
        Dictionary<string, Sprite> nameSpriteDict = new Dictionary<string, Sprite>();

        // Get the resourceDefs with this type
        IEnumerable<ResourceDef> resourcesOfType = ResourceQueries.ByTypeSortedByTier(resourceManager.resourceDefinitions, type);
        foreach (ResourceDef def in resourcesOfType)
            nameSpriteDict.Add(def.name, def.image);

        return nameSpriteDict;
    }

    public override Dictionary<string, Sprite> GetImageOptions(ResourceManager resourceManager, int minTier)
    {
        Dictionary<string, Sprite> nameSpriteDict = new Dictionary<string, Sprite>();

        // Get the resourceDefs with this type
        IEnumerable<ResourceDef> resourcesOfType = ResourceQueries.ByTypeSortedByTierMinTier(resourceManager.resourceDefinitions, type, minTier);
        foreach (ResourceDef def in resourcesOfType)
            nameSpriteDict.Add(def.name, def.image);

        return nameSpriteDict;
    }
}
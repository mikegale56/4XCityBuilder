using System;
using UnityEngine;

public class ResourceNameQuantityQuality : ResourceQuantityQuality
{
    public string name;

    public ResourceNameQuantityQuality(string name, QualityEnum quality, int quantity)
    {
        this.name = name;
        this.quantity = quantity;
        this.quality = quality;
    }
	
	public override void AddResource(ResourceStock stock)
	{
		if (quality == QualityEnum.any)
			Debug.LogError("Error: no quality specified when adding resources to the stock");
		stock.quantity[stock.nameToIndexDictionary[name]][(int)quality] += quantity;
	}
	
	public override bool CheckResource(ResourceStock stock)
	{
		int numInStock = 0;

		// if quality is not selected
		if (quality == QualityEnum.any)
		{
			// Check for any quality
			foreach (QualityEnum qVal in Enum.GetValues(typeof(QualityEnum)))
			{
				if (qVal == QualityEnum.any) // Skip "any"
					continue;
				else
					numInStock += stock.quantity[stock.nameToIndexDictionary[name]][(int)qVal];
			}
		}
		else // Get the number of that specific quality
			numInStock = stock.quantity[stock.nameToIndexDictionary[name]][(int)quality];

		return numInStock >= quantity;
	}
	
	public override float CheckResourceQuality(ResourceStock stock)
	{
		int leftToRemove = quantity;
		float totalMult = 0;
        float averageQualityMultiplierOfRemoved = 0.0F;

        // If quality is not selected
        if (quality == QualityEnum.any)
		{
			int numEnumValues = Enum.GetValues(typeof(QualityEnum)).Length;

			// Start at highest quality, which is numEnumValues-1, as the highest is "any"
			int qVal = numEnumValues - 1;
			while (leftToRemove > 0) // Keep removing until there are none left to remove
			{
				// How many are there in this quality bin?
				int quant = stock.quantity[stock.nameToIndexDictionary[name]][qVal];
				if (quant >= leftToRemove) // If there are more than needed
				{
                    // Add to the average quality
                    totalMult += leftToRemove * stock.qualityMultiplier[(QualityEnum)qVal];
					// Set the number to remove to zero                           
					leftToRemove = 0;
				} else // There are not enough
				{
					// Count what is left
					int numRemoved = stock.quantity[stock.nameToIndexDictionary[name]][qVal];
                    // Add to the average quality
                    totalMult += numRemoved * stock.qualityMultiplier[(QualityEnum)qVal];
					// Decrement the number left to remove
					leftToRemove -= numRemoved;
				}
				qVal++;
				if (qVal >= (int)QualityEnum.any)
				{
					Debug.LogError("Cannot check to remove enough of a resource from stock - this shouldn't happen, did I forget a check");
					break;
				}
			}
            averageQualityMultiplierOfRemoved = totalMult / quantity;
        }
		else
		{
			// How many are there in this quality bin?
			int quant = stock.quantity[stock.nameToIndexDictionary[name]][(int)quality];
			if (quant >= leftToRemove) // If there are more than needed
			{
                averageQualityMultiplierOfRemoved = stock.qualityMultiplier[quality];
            } else
				Debug.LogError("Cannot remove enough of a resource from stock - this shouldn't happen, did I forget a check");
		}

		return averageQualityMultiplierOfRemoved;
	}
	
	public override float RemoveResource(ResourceStock stock)
	{
		int leftToRemove = quantity;
		float totalMultOfRemoved = 0;
		float averageQualityMultiplierOfRemoved = 0.0F;

		// If quality is not selected
		if (quality == QualityEnum.any)
		{
			int numEnumValues = Enum.GetValues(typeof(QualityEnum)).Length;

			// Start at highest quality, which is numEnumValues-1, as the highest is "any"
			int qVal = numEnumValues - 1;
			while (leftToRemove > 0) // Keep removing until there are none left to remove
			{
				// How many are there in this quality bin?
				int quant = stock.quantity[stock.nameToIndexDictionary[name]][qVal];
				if (quant >= leftToRemove) // If there are more than needed
				{
					// Remove them
					stock.quantity[stock.nameToIndexDictionary[name]][qVal] -= leftToRemove;
					// Add to the average quality
					totalMultOfRemoved += leftToRemove * stock.qualityMultiplier[(QualityEnum)qVal];
					// Set the number to remove to zero                           
					leftToRemove = 0;
				} else // There are not enough
				{
					// Remove what is left
					int numRemoved = stock.quantity[stock.nameToIndexDictionary[name]][qVal];
					stock.quantity[stock.nameToIndexDictionary[name]][qVal] = 0;
					// Add to the average quality
					totalMultOfRemoved += numRemoved * stock.qualityMultiplier[(QualityEnum)qVal];
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
			averageQualityMultiplierOfRemoved = totalMultOfRemoved / quantity;
		}
		else
		{
			// How many are there in this quality bin?
			int quant = stock.quantity[stock.nameToIndexDictionary[name]][(int)quality];
			if (quant >= leftToRemove) // If there are more than needed
			{
				stock.quantity[stock.nameToIndexDictionary[name]][(int)quality] -= quantity;
				averageQualityMultiplierOfRemoved = stock.qualityMultiplier[quality];
			} else
				Debug.LogError("Cannot remove enough of a resource from stock - this shouldn't happen, did I forget a check");
		}
		
		return averageQualityMultiplierOfRemoved;
	}	
}
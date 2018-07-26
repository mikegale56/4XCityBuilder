using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The definition of a single resource
public class JobDef
{

    public string name, description, industry, skill, tileRequired;
	public int tier;
	public List<string> outputName;
	public List<int> defaultOutputQuantity;
    public float defaultPMUs;
    public ResourceQuantityQualityList inputResources;

    // Constructor
    public JobDef(string csvLine, Dictionary<string, List<int>> column)
    {

        inputResources = new ResourceQuantityQualityList();
		outputName = new List<string>();
		defaultOutputQuantity = new List<int>();

		string[] values = csvLine.Split(',');

        // Job Name
        if (values[column["Job Name"][0]].Length > 0)
			name = values[column["Job Name"][0]];
		
		// Description
		if (values[column["Description"][0]].Length > 0)
			description = values[column["Description"][0]];
		
		//Industry,Skill
		if (values[column["Industry"][0]].Length > 0)
			industry = values[column["Industry"][0]];
		
		if (values[column["Skill"][0]].Length > 0)
			skill = values[column["Skill"][0]];

		// Tier
		if (values[column["Tier"][0]].Length > 0)
			if (!Int32.TryParse(values[column["Tier"][0]], out tier))
				Debug.Log("Cannot Parse Job Tier");

		// Tiles Required
		if (values[column["Tiles Required"][0]].Length > 0)
			tileRequired = values[column["Tiles Required"][0]];
		
		// Max Workers
		if (values[column["Work PMU"][0]].Length > 0)
			if (!Single.TryParse(values[column["Work PMU"][0]], out defaultPMUs))
				Debug.Log("Cannot Parse Work PMU");
			
		// Resources to Craft
		List<int> inputName  = column["Input Name"];
		List<int> inputType  = column["Input Type"];
		List<int> inputTier  = column["Input Tier"];
		List<int> inputQuant = column["Input Quantity"];

		for (int i=0; i<inputName.Count; i++ )
		{
			// Check for NQQ
			if (values[inputName[i]].Length > 0)
			{
				string name = values[inputName[i]];
				int quantity;
				if (!Int32.TryParse(values[inputQuant[i]], out quantity))
					Debug.Log("Cannot Parse Input Quantity: " + values[inputQuant[i]]);
                ResourceNameQuantityQuality nqq = new ResourceNameQuantityQuality(name, QualityEnum.any, quantity);
				inputResources.rqqList.Add(nqq);
				// Error check
				if (values[inputType[i]].Length > 0)
					Debug.LogWarning("Job Definition file should not have both Name and Type of a Job Input Resource");
			} 
			// check for TQQ
			else if (values[inputType[i]].Length > 0)
			{
				string type = values[inputType[i]];
				int quantity, minTier;
				if (!Int32.TryParse(values[inputQuant[i]], out quantity))
					Debug.Log("Cannot Parse Input Quantity: " + values[inputQuant[i]]);
				if (!Int32.TryParse(values[inputTier[i]], out minTier))
					Debug.Log("Cannot Parse Input Tier: " + values[inputTier[i]]);
                ResourceTypeQuantityQuality tqq = new ResourceTypeQuantityQuality(name, QualityEnum.any, quantity, minTier);
				inputResources.rqqList.Add(tqq);
			}
		}
		
		// Output Resources
		List<int> outputNameC      = column["Output Name"];
		List<int> outputQuantityC  = column["Output Quantity"];
		
		for (int i=0; i<outputNameC.Count; i++ )
		{
			// Output Name
			if (values[outputNameC[i]].Length > 0) // There is an output in this column
			{
				outputName.Add(values[column["Output Name"][0]]);
				int q = 0;
				if (!Int32.TryParse(values[outputQuantityC[i]], out q))
					Debug.Log("Cannot Parse Output Quantity");
                defaultOutputQuantity.Add(q);
			}
		}		
    }
}
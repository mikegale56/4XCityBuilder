using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The definition of a single resource
public class JobObj
{

	public bool isSet;
	public bool isActive;
    public bool hasStarted;
    private int numWorkers;
	public JobDef jobDef;
	public BuildingObj buildingObj;
	public Guid buildingGuid;
	
	private float currentPMURate;
	private float workPMUsRemaining;
	
    private float accumulatedQualityPoints;

    public float leaderBonusSpeedMultiplier;
    public float buildingBonusSpeedMultiplier;

    public float leaderBonusQualityMultiplier;
    public float buildingBonusQualityMultiplier;
    private float inputMaterialQualityMultiplier;

    public float leaderBonusQuantityMultiplier;
    public float buildingBonusQuantityMultiplier;
		
	private ResourceQuantityQualityList inputResources;
	
	public ResourceStock stock;
	
    // Constructor
	// In the tile detail screen, user selects a new job.  That instantiates a JobObj.
	// Then they user chooses the materials and hits go, that sets the job.  Once it has workers
	// and is set, it can start and takes resources
    public JobObj(JobDef def, BuildingObj bldg)
    {
		this.isSet = false; // Has been set by the UI
		this.isActive = false; // Has people working it
		this.hasStarted = false; // Is being worked
		this.numWorkers = 0;
		this.jobDef = def;
		this.buildingObj = bldg;
		this.buildingGuid = bldg.guid;
		
		// PMU work
		this.currentPMURate = 0.0F; //Will be calculated
		this.workPMUsRemaining = this.jobDef.defaultPMUs;
		
		// Quality
		this.inputMaterialQualityMultiplier = 1; //Will be set by the materials
		this.accumulatedQualityPoints = 0;

        // Add in Building Bonuses
        foreach (JobBonus jb in bldg.buildingBonuses)
            jb.ApplyBonusToJob(this);
        
    }
	
	public void UpdateBonuses(List<JobBonus> bonusList)
	{
		this.leaderBonusQuantityMultiplier   = 1;
		this.buildingBonusQuantityMultiplier = 1;
		this.leaderBonusSpeedMultiplier      = 1;
		this.buildingBonusSpeedMultiplier    = 1;
		this.leaderBonusQualityMultiplier    = 1;
		this.buildingBonusQualityMultiplier  = 1;
		
		foreach (JobBonus bonus in bonusList)
			bonus.ApplyBonusToJob(this);
	}
	
	public void UpdateJob(float deltaTimeSeconds)
	{
		
		// Don't update inactive or unset jobs
		if (!this.isSet || !this.isActive) 
			return;
		
		// Check if it has started
		if (!this.hasStarted)
		{ // Not started:
			// Check that there are enough resources
			if (!inputResources.CheckResources(stock))
			{
				// This will eventually need to be an event
				Debug.Log("Not enough resources to start job - waiting");
				return;
			}
			
			// There are enough resources
			// Remove the resources and recalculate the qualityMultiplier
			float qualityMultiplier = inputResources.RemoveResources(stock);
			float tierMultiplier    = 1; // Needs to be updated
			// Update the quality multiplier
			this.inputMaterialQualityMultiplier = qualityMultiplier * tierMultiplier;
			// Reset the PMUs
			this.workPMUsRemaining = this.jobDef.defaultPMUs;
			// Set it to started!
			this.hasStarted = true;
		} else
		{ // Has started
			// Add PMUs
			float addedPMUs = this.currentPMURate*deltaTimeSeconds/60; // div by 60 to convert to minutes (PMUs)
			this.workPMUsRemaining -= addedPMUs; 
			// Accumulate quality
			this.accumulatedQualityPoints += this.buildingBonusQuantityMultiplier * this.leaderBonusQuantityMultiplier * this.inputMaterialQualityMultiplier*
				(addedPMUs/this.jobDef.defaultPMUs); // Quality multipliers normalized by work fraction
				// Probably need to normalize this by tier of the product as well, but this needs a formula
			
			// If it's done?
			if (this.workPMUsRemaining <= 0)
			{
				this.hasStarted = false;
				ResourceQuantityQualityList products = new ResourceQuantityQualityList();
				
				// Allow for multiple outputs
				for (int i = 0; i < this.jobDef.outputName.Count; i++)
				{
					int outputQuantity = Mathf.RoundToInt(this.jobDef.defaultOutputQuantity[i] * this.buildingBonusQuantityMultiplier * this.leaderBonusQuantityMultiplier);
					// Add the resource
					products.rqqList.Add(new ResourceNameQuantityQuality(
						this.jobDef.outputName[i], QualityEnum.normal,
						outputQuantity));
				}
				products.AddResources(stock);
				// Need to trigger an event here for display of done
			}
		}
	}
	
	public void StartJob()
	{
		// Set isSet
		this.isSet 	  = true;
	}
	
	// Set the resource list
	public bool SetResources(ResourceQuantityQualityList input)
	{
		// Check that they're all nqqs
		foreach (ResourceQuantityQuality rqq in input.rqqList)
			if (!(rqq is ResourceNameQuantityQuality))
				return false;
		
		float qualityMultiplier = input.CheckResourcesQuality(stock);
		float tierMultiplier    = 1; // Needs to be updated
		
		this.inputMaterialQualityMultiplier = qualityMultiplier * tierMultiplier;
	
		this.inputResources = input;	
		
		return true;
	}
	
	// Add a worker
	public void AddWorker()
	{
		// Increment the number of workers
		this.numWorkers += 1;
		
		// If it has been set, make active
		if (this.numWorkers > 0)
			if (this.isSet)
				this.isActive = true;
		
		// Modify the work rate
		this.currentPMURate = this.leaderBonusSpeedMultiplier * this.buildingBonusSpeedMultiplier * this.numWorkers;
	}
	
	// Remove a worker
	public void RemoveWorker()
	{
		
		if (this.numWorkers > 0)
			this.numWorkers -= 1;
		else
			Debug.LogWarning("Attempting to go under 0 workers");
		
		if (this.numWorkers == 0)
			this.isActive = false;
		
		// Modify the work rate
		this.currentPMURate = this.leaderBonusSpeedMultiplier * this.buildingBonusSpeedMultiplier * this.numWorkers;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The definition of a single resource
public abstract class JobObj
{

    public int iLoc, jLoc;
	public bool isSet;
	public bool isActive;
    public bool hasStarted;
    public bool toDelete;
    protected int numWorkers;
	public JobDef jobDef;
	public BuildingObj buildingObj;
	public Guid buildingGuid;

    protected float currentPMURate;
    protected float workPMUsRemaining;

    protected float accumulatedQualityPoints;

    public float leaderBonusSpeedMultiplier;
    public float buildingBonusSpeedMultiplier;

    public float leaderBonusQualityMultiplier;
    public float buildingBonusQualityMultiplier;
    protected float inputMaterialQualityMultiplier;

    public float leaderBonusQuantityMultiplier;
    public float buildingBonusQuantityMultiplier;
		
	protected ResourceQuantityQualityList inputResources;
	
	public ResourceStock stock;
	
    // Constructor
	// In  the tile detail screen, user selects a new job.  That instantiates a JobObj.
	// Then they user chooses the materials and hits go, that sets the job.  Once it has workers
	// and is set, it can start and takes resources
    public JobObj(JobDef def)
    {
		this.isSet = false; // Has been set by the UI
		this.isActive = false; // Has people working it
		this.hasStarted = false; // Is being worked
        this.toDelete = false; // Trigger the JobManager to delete this job

        this.numWorkers = 0;
		this.jobDef = def;
  	
		// PMU work
		this.currentPMURate = 0.0F; //Will be calculated
		this.workPMUsRemaining = this.jobDef.defaultPMUs;
		
		// Quality
		this.inputMaterialQualityMultiplier = 1; //Will be set by the materials
		this.accumulatedQualityPoints = 0;

        this.stock = ManagerBase.domain.stock;
    }

    abstract public Sprite GetSprite();

    abstract public void UpdateJob(float deltaTimeSeconds);

    public void UpdateBonuses(List<JobBonus> bonusList)
    {
        this.leaderBonusQuantityMultiplier = 1;
        this.buildingBonusQuantityMultiplier = 1;
        this.leaderBonusSpeedMultiplier = 1;
        this.buildingBonusSpeedMultiplier = 1;
        this.leaderBonusQualityMultiplier = 1;
        this.buildingBonusQualityMultiplier = 1;

        foreach (JobBonus bonus in bonusList)
            bonus.ApplyBonusToJob(this);
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

    public ResourceQuantityQualityList GetInputResources()
    {
        return inputResources;
    }

    public void StartJob()
    {
        // Set isSet
        this.isSet = true;
    }

    // Set the resource list
    public bool SetResources(ResourceQuantityQualityList input)
    {
        // Check that they're all nqqs
        foreach (ResourceQuantityQuality rqq in input.rqqList)
            if (!(rqq is ResourceNameQuantityQuality))
                return false;

        float qualityMultiplier = input.CheckResourcesQuality(stock);
        float tierMultiplier = 1; // Needs to be updated

        this.inputMaterialQualityMultiplier = qualityMultiplier * tierMultiplier;

        this.inputResources = input;

        return true;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The definition of a single resource
public class ResourceJobObj : JobObj
{

    public ResourceJobObj(JobDef def, BuildingObj bldg) : base(def)
    {
        this.buildingObj = bldg;
        this.buildingGuid = bldg.guid;
        // Add in Building Bonuses
        foreach (JobBonus jb in bldg.buildingBonuses)
            jb.ApplyBonusToJob(this);
        this.iLoc = bldg.ijLocation.x;
        this.jLoc = bldg.ijLocation.y;
    }

    override public Sprite GetSprite()
    {
        // Need Multiple resource outputs!!!!
        return ManagerBase.resourceDefinitions[ManagerBase.resourceIndexOf[jobDef.outputName[0]]].image;
    }

    override public void UpdateJob(float deltaTimeSeconds)
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
            float tierMultiplier = 1; // Needs to be updated
                                      // Update the quality multiplier
            this.inputMaterialQualityMultiplier = qualityMultiplier * tierMultiplier;
            // Reset the PMUs
            this.workPMUsRemaining = this.jobDef.defaultPMUs;
            // Set it to started!
            this.hasStarted = true;
            Debug.Log("Resource Job: " + jobDef.name + " being set to started");
        }
        else
        { // Has started
          // Add PMUs
            float addedPMUs = this.currentPMURate * deltaTimeSeconds / 60; // div by 60 to convert to minutes (PMUs)
            this.workPMUsRemaining -= addedPMUs;
            // Accumulate quality
            this.accumulatedQualityPoints += this.buildingBonusQuantityMultiplier * this.leaderBonusQuantityMultiplier * this.inputMaterialQualityMultiplier *
                (addedPMUs / this.jobDef.defaultPMUs); // Quality multipliers normalized by work fraction
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
            Debug.Log("Resource Job: " + jobDef.name + " going with " + this.workPMUsRemaining.ToString() + " PMUs left");
        }
    }
}
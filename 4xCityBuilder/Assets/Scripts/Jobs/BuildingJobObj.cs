using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The definition of a single resource
public class BuildingJobObj : JobObj
{
    // Constructor
    public BuildingJobObj(JobDef def, int iLoc, int jLoc) : base(def)
    {
        this.iLoc = iLoc;
        this.jLoc = jLoc;
    }

    override public Sprite GetSprite()
    {
        return ManagerBase.buildingDefinitions[ManagerBase.buildingIndexOf[jobDef.outputName[0]]].sprite;
    }

    override public void UpdateJob(float deltaTimeSeconds)
    {
        //if (!this.isSet)
        //    Debug.Log("Construction Job: " + jobDef.name + " is NOT set");
        //if (!this.isActive)
        //    Debug.Log("Construction Job: " + jobDef.name + " is NOT active");

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
            Debug.Log("Construction Job: " + jobDef.name + " being set to started");
        }
        else
        {   // Has started
            // Add PMUs

            float addedPMUs = this.currentPMURate * deltaTimeSeconds / 60.0F; // div by 60 to convert to minutes (PMUs)
            this.workPMUsRemaining -= addedPMUs;
            // Accumulate quality
            this.accumulatedQualityPoints += this.buildingBonusQuantityMultiplier * this.leaderBonusQuantityMultiplier * this.inputMaterialQualityMultiplier *
                (addedPMUs / this.jobDef.defaultPMUs); // Quality multipliers normalized by work fraction
                                                       // Probably need to normalize this by tier of the product as well, but this needs a formula

            // If it's done?
            if (this.workPMUsRemaining <= 0)
            {
                // Need to trigger an event here for display of done
                
                // Change surface type
                ManagerBase.domain.mapData.SetSurfaceValue(iLoc, jLoc, ManagerBase.surfaceValueDictionary[jobDef.outputName[0]]);

                // Remove workers, clear the job
                while (this.numWorkers > 0)
                    this.RemoveWorker();
                this.toDelete = true;

                // Reload the map
                WorldEventHandlerManager.Broadcast(worldEventChannels.map, mapChannelEvents.change, new WorldEventArg(iLoc, jLoc));

                // Complete the job
                string message = jobDef.name + " at (" + iLoc.ToString() + "," + jLoc.ToString() + ")";
                ManagerBase.domain.eventManager.Broadcast(domainEventChannels.job, jobChannelEvents.constructionComplete, new DomainEventArg(message, iLoc, jLoc));

            }
            Debug.Log("Construction Job: " + jobDef.name + " going with " + this.workPMUsRemaining.ToString() + " PMUs left");
            Debug.Log("   PMU Rate: " + this.currentPMURate.ToString() + ", just added: " + addedPMUs.ToString());
        }
    }
}
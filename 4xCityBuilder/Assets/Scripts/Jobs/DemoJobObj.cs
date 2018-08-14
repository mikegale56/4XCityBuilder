using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoJobObj : JobObj
{
    // Constructor
    public DemoJobObj(JobDef def, int iLoc, int jLoc) : base(def)
    {
        this.iLoc = iLoc;
        this.jLoc = jLoc;
    }

    override public Sprite GetSprite()
    {
		//ManagerBase.groundValueDictionary[] // I don't think this is needed
        return ManagerBase.groundTiles[ManagerBase.domain.mapData.GetGroundValue(iLoc, jLoc)].sprite;
    }

    override public void UpdateJob(float deltaTimeSeconds)
    {
        //if (!this.isSet)
        //    Debug.Log("Demo Job: " + jobDef.name + " is NOT set");
        //if (!this.isActive)
        //    Debug.Log("Demo Job: " + jobDef.name + " is NOT active");

        // Don't update inactive or unset jobs
        if (!this.isSet || !this.isActive)
            return;

        // Check if it has started
        if (!this.hasStarted)
        { 
            // Reset the PMUs
            this.workPMUsRemaining = this.jobDef.defaultPMUs;
            // Set it to started!
            this.hasStarted = true;
            Debug.Log("Demo Job: " + jobDef.name + " being set to started");
        }
        else
        {   // Has started
            // Add PMUs

            float addedPMUs = this.currentPMURate * deltaTimeSeconds / 60.0F; // div by 60 to convert to minutes (PMUs)
            this.workPMUsRemaining -= addedPMUs;
            // Accumulate quality

            // If it's done?
            if (this.workPMUsRemaining <= 0)
            {
                // Change surface type
                ManagerBase.domain.mapData.SetSurfaceValue(iLoc, jLoc, ManagerBase.surfaceValueDictionary["Nothing"]);

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
            Debug.Log("Demo Job: " + jobDef.name + " going with " + this.workPMUsRemaining.ToString() + " PMUs left");
            Debug.Log("   PMU Rate: " + this.currentPMURate.ToString() + ", just added: " + addedPMUs.ToString());
        }
    }
}
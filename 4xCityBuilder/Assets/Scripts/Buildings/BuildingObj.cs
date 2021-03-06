using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObj
{
    public Guid guid;
    public string name;
    public QualityEnum quality;
    public Vector2Int ijLocation;
    public List<JobBonus> buildingBonuses;
    public float calculatedMaxDistToWorkTiles;
    public int maxHp;
    public int currentHp;
    public int calculatedHousing;
    public List<Vector2Int> workedTiles;
    private JobManager jobManager;

    public BuildingObj(Vector2Int ij, BuildingDef buildingDef, QualityEnum quality)
    {
        guid = System.Guid.NewGuid();
        ijLocation = ij;
        this.quality = quality;
        name = buildingDef.name;
        calculatedMaxDistToWorkTiles = ResourceManager.qualityMultiplier[quality] * buildingDef.defaultMaxDistToWorkTiles;
        calculatedHousing = Mathf.RoundToInt(ResourceManager.qualityMultiplier[quality] * buildingDef.housing);
        maxHp = buildingDef.maxHp;
        currentHp = maxHp;

        // Need to add worked tiles

        // Need to add calculatedBonuses
        buildingBonuses = new List<JobBonus>();

        //this.jobManager = jobManager;

    }

    // Function to see how many active jobs there are
    public int NumberOfActiveJobs()
    { return 0; }

    // Function to see how many people work here total
    public int NumberOfWorkers()
    { return 1; }

    // Function to take damage
    public void TakeDamage()
    { }

    // Remove the building and remove the jobs
    public void RemoveBuilding()
    { }

    // Add a new worked tile
    public void AddWorkedTile(Vector2Int newTile)
    {
        if (!workedTiles.Contains(newTile))
            workedTiles.Add(newTile);
        // Changes stuff here!
    }

    // Add a new worked tile
    public void RemoveWorkedTile(Vector2Int tileToRemove)
    {
        if (workedTiles.Contains(tileToRemove))
            workedTiles.Remove(tileToRemove);
        // Changes stuff here!
    }



}
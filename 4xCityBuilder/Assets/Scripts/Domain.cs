using System.Collections.Generic;
using UnityEngine;

public class Domain
{
	public MapData mapData;
	public ResourceStock stock;
	public List<BuildingObj> buildings;
	private List<JobObj> jobs;
	private List<JobBonus> activeLeaderJobBonusList;

    public DomainEventHandlerManager eventManager = new DomainEventHandlerManager();

    public string domainName;
	public string domainOwner;
	
	public int population = 0;
	public int freePopulation = 0;
	public float money;
	public float happiness;
	public float maintenanceCosts;

    public Domain()
    {
        
    }
	
}
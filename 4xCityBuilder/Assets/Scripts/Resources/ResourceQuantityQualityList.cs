using System.Collections.Generic;

public class ResourceQuantityQualityList
{
    public List<ResourceQuantityQuality> rqqList;

	public ResourceQuantityQualityList()
	{
		rqqList = new List<ResourceQuantityQuality>();
	}
	
	public void AddResources(ResourceStock stock)
	{
		foreach (ResourceQuantityQuality rqq in rqqList)
			rqq.AddResource(stock);
	}
	
	public bool CheckResources(ResourceStock stock)
	{
        bool response = true;
		for (int i = 0; i<rqqList.Count; i++)
			response = response & rqqList[i].CheckResource(stock);
		return response;
	}

    public float CheckResourcesQuality(ResourceStock stock)
	{
		float totalMultiplier = 0;
		float resourceCount   = 0;
		for (int i = 0; i<rqqList.Count; i++)
		{
			float thisMultiplier = rqqList[i].CheckResourceQuality(stock);
			totalMultiplier += thisMultiplier*rqqList[i].quantity;
			resourceCount += rqqList[i].quantity;
		}
		return totalMultiplier / resourceCount;
	}

    public float RemoveResources(ResourceStock stock)
	{
		float totalMultiplier = 0;
		float resourceCount   = 0;
		for (int i = 0; i<rqqList.Count; i++)
		{
			float thisMultiplier = rqqList[i].RemoveResource(stock);
			totalMultiplier += thisMultiplier*rqqList[i].quantity;
			resourceCount += rqqList[i].quantity;
		}
		return totalMultiplier / resourceCount;
	}
}
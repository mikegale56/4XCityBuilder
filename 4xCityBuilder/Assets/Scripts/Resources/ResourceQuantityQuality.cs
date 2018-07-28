public abstract class ResourceQuantityQuality
{
    public QualityEnum quality;
    public int quantity;
    public int minTier = 0;

	abstract public void AddResource(ResourceStock stock);
	
	abstract public bool CheckResource(ResourceStock stock);
	
	abstract public float CheckResourceQuality(ResourceStock stock);
	
	abstract public float RemoveResource(ResourceStock stock);

    abstract public System.Collections.Generic.Dictionary<string, UnityEngine.Sprite> GetImageOptions(ResourceManager resourceManager);

}
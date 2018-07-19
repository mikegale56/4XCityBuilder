public class ResourceTypeQuantityQuality
{
    public string type;
    public QualityEnum quality;
    public int quantity;
    public int minTier = 0;

    public ResourceTypeQuantityQuality(string type, QualityEnum quality, int quantity)
    {
        this.type = type;
        this.quantity = quantity;
        this.quality = quality;
    }

    public ResourceTypeQuantityQuality(string type, QualityEnum quality, int quantity, int minTier)
    {
        this.type = type;
        this.quantity = quantity;
        this.quality = quality;
        this.minTier = minTier;
    }

}

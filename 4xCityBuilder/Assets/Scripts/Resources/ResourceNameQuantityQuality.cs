public class ResourceNameQuantityQuality
{
    public string name;
    public QualityEnum quality;
    public int quantity;

    public ResourceNameQuantityQuality(string name, QualityEnum quality, int quantity)
    {
        this.name = name;
        this.quantity = quantity;
        this.quality = quality;
    }
}

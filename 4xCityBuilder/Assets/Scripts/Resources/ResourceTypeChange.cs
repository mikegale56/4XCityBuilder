public class ResourceTypeChange
{
    public string type;
    public QualityEnum quality;
    public int quantity;

    public ResourceTypeChange(string type, QualityEnum quality, int quantity)
    {
        this.type = type;
        this.quantity = quantity;
        this.quality = quality;
    }

}

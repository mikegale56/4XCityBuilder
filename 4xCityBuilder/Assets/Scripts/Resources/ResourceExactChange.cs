public class ResourceExactChange
{
    public string name;
    public QualityEnum quality;
    public int quantity;

    public ResourceExactChange(string name, QualityEnum quality, int quantity)
    {
        this.name = name;
        this.quantity = quantity;
        this.quality = quality;
    }
}

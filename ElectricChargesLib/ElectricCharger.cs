namespace ElectricChargesLib;

public class ElectricCharger
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string AdmArea { get; init; }
    public string District { get; init; }
    public string Address { get; init; }
    public double Longitude { get; init; }
    public double Latitude { get; init; }
    public int GlobalId { get; init; }
    public string GeoDataCenter { get; init; }
    public string GeoArea { get; init; }

    public ElectricCharger()
    {
        throw new ArgumentException("Wrong parameters.");
    }
}
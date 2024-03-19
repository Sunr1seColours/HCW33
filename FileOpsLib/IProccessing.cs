using ElectricChargesLib;

namespace FileOpsLib;

public interface IProccessing
{
    public ElectricCharger[] Read(Stream stream);

    public Stream Write(ElectricCharger[] chargers);
}
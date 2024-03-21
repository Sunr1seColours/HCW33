using ElectricChargesLib;

namespace FileOpsLib;

public interface IProcessing
{
    public ElectricCharger[] Read(Stream stream);

    public Stream Write(ElectricCharger[] chargers);
}
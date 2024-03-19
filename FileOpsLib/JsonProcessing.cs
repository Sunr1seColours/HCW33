using System.Text;
using System.Text.Json;
using ElectricChargesLib;

namespace FileOpsLib;

public class JsonProcessing : IProccessing
{
    public ElectricCharger[] Read(Stream stream)
    {
        if (stream is { Length: > 0 })
            return JsonSerializer.Deserialize<ElectricCharger[]>(stream);
        throw new ArgumentException("Hollow file.");
    }

    public Stream Write(ElectricCharger[] chargers)
    {
        StringBuilder json = new StringBuilder();
        json.Append("[\n");
        foreach (ElectricCharger charger in chargers)
        {
            json.Append(charger.ToJson());
            json.Append(",\n");
        }

        json.Remove(json.Length - 2, 1);
        json.Append(']');
        using FileStream stream = new FileStream("../../../../outputFiles/output.json", FileMode.Create);
        File.WriteAllText("../../../../outputFiles/output.json",json.ToString());
        return stream;
    }
}
using System.Text;
using System.Text.Json;
using ElectricChargesLib;

namespace FileOpsLib;

public class JsonProcessing : IProcessing
{
    public ElectricCharger[] Read(Stream stream)
    {
        if (stream is { Length: > 0 })
            return JsonSerializer.Deserialize<ElectricCharger[]>(stream);
        throw new ArgumentException("Пустой файл.");
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
        MemoryStream memoryStream = new MemoryStream();
        using (StreamWriter writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            writer.Write(json);    
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}
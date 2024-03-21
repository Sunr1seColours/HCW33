using System.Text;
using System.Text.Json;
using ElectricChargesLib;

namespace FileOpsLib;

/// <summary>
/// Releases operations on csv files.
/// </summary>
public class JSONProcessing : IProcessing
{
    /// <summary>
    /// Reads stream of json file and converts it to array of ElectricCharger objects.
    /// </summary>
    /// <param name="stream">Stream which represents json file.</param>
    /// <returns>Array of ElectricCharger objects.</returns>
    /// <exception cref="ArgumentException">File doesn't have any info about electric chargers.</exception>
    public ElectricCharger[] Read(Stream stream)
    {
        if (stream is { Length: > 0 })
            return JsonSerializer.Deserialize<ElectricCharger[]>(stream);
        throw new ArgumentException("Пустой файл.");
    }

    /// <summary>
    /// Converts array of ElectricCharger objects to stream.
    /// </summary>
    /// <param name="chargers">Array of ElectricCharger objects.</param>
    /// <returns>MemoryStream which represents this collection in json format.</returns>
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
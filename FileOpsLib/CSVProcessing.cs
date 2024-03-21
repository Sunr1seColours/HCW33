using System.Text;
using ElectricChargesLib;
using CsvHelper;
using CsvHelper.Configuration;
using static System.Globalization.CultureInfo;

namespace FileOpsLib;

/// <summary>
/// Releases operations on csv files.
/// </summary>
public class CSVProcessing : IProcessing
{
    /// <summary>
    /// Reads stream of csv file and converts it to array of ElectricCharger objects.
    /// </summary>
    /// <param name="stream">Stream which represents csv file.</param>
    /// <returns>Array of ElectricCharger objects.</returns>
    /// <exception cref="ArgumentException">File doesn't have any info about electric chargers.</exception>
    public ElectricCharger[] Read(Stream stream)
    {
        CsvConfiguration config = new CsvConfiguration(InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            DetectDelimiter = true
        };
        using StreamReader streamReader = new StreamReader(stream);
        ElectricCharger[] chargers;
        using (CsvReader reader = new CsvReader(streamReader, config))
        {
            chargers = reader.GetRecords<ElectricCharger>().ToArray();
        }

        if (chargers.Length > 1)
        {
            if (string.Equals(chargers[0].Name, "Наименование"))
            {
                return chargers[1..];
            }
            else
            {
                return chargers;
            }
        }
        else if (chargers.Length == 1 && chargers[0].Name != "Наименование")
        {
            return chargers;
        }

        throw new ArgumentException("Файл пуст.");
    }

    /// <summary>
    /// Converts array of ElectricCharger objects to stream.
    /// </summary>
    /// <param name="chargers">Array of ElectricCharger objects.</param>
    /// <returns>MemoryStream which represents this collection in csv format.</returns>
    public Stream Write(ElectricCharger[] chargers)
    {
        StringBuilder csv = new StringBuilder();
        csv.Append(
            "\"object_category_Id\";\"ID\";\"Name\";\"AdmArea\";\"District\";\"Address\";\"Longitude_WGS84\";\"Latitude_WGS84\";\"global_id\";\"geodata_center\";\"geoarea\";\n");
        csv.Append(
            "\"object_category_Id\";\"Код\";\"Наименование\";\"Административный округ\";\"Район\";\"Адрес\";\"Долгота в WGS-84\";\"Широта в WGS-84\";\"global_id\";\"geodata_center\";\"geoarea\";\n");
        foreach (ElectricCharger charger in chargers)
        {
            csv.Append(charger.ToCsv());
            csv.Append('\n');
        }

        csv.Remove(csv.Length - 1, 1);
        MemoryStream memoryStream = new MemoryStream();
        using (StreamWriter writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            writer.Write(csv.ToString());    
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}
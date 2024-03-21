using System.Text;
using ElectricChargesLib;
using CsvHelper;
using CsvHelper.Configuration;
using Telegram.Bot.Types;
using static System.Globalization.CultureInfo;
using File = System.IO.File;

namespace FileOpsLib;

/// <summary>
/// Releases operations on csv files.
/// </summary>
public class CsvProcessing : IProcessing
{
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

        throw new ArgumentException("Hollow file.");
    }

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
using System.Text;
using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace ElectricChargesLib;


/// <summary>
/// Class which represents info about charger for electric auto.
/// </summary>
public class ElectricCharger
{
    
    /// <summary>
    /// ID of charger. Integer number.
    /// </summary>
    [JsonPropertyName("id")]
    [Name("id")]
    public string Id { get; init; }
    
    /// <summary>
    /// Charger's name. 
    /// </summary>
    [JsonPropertyName("name")]
    [Name("name")]
    public string Name { get; init; }
    
    /// <summary>
    /// Administration area where charger is.
    /// </summary>
    [JsonPropertyName("admarea")]
    [Name("admarea")]
    public string AdmArea { get; init; }
    
    /// <summary>
    /// District where charger is.
    /// </summary>
    [JsonPropertyName("district")]
    [Name("district")]
    public string District { get; init; }
    
    /// <summary>
    /// Address where charger is.
    /// </summary>
    [JsonPropertyName("address")]
    [Name("address")]
    public string Address { get; init; }
    
    /// <summary>
    /// Longitude of charger. Double number.
    /// </summary>
    [JsonPropertyName("longitude_wgs84")]
    [Name("longitude_wgs84")]
    public string Longitude { get; init; }
    
    /// <summary>
    /// Latitude of charger. Double number.
    /// </summary>
    [JsonPropertyName("latitude_wgs84")]
    [Name("latitude_wgs84")]
    public string Latitude { get; init; }
    
    /// <summary>
    /// Global ID of charger. Integer number.
    /// </summary>
    [JsonPropertyName("global_id")]
    [Name("global_id")]
    public string GlobalId { get; init; }
    
    /// <summary>
    /// Information about geo data center.
    /// </summary>
    [JsonPropertyName("geodata_center")]
    [Name("geodata_center")]
    public string GeoDataCenter { get; init; }
    
    /// <summary>
    /// Information about geo area.
    /// </summary>
    [JsonPropertyName("geoarea")]
    [Name("geoarea")]
    public string GeoArea { get; init; }
    
    /// <summary>
    /// Default constructor with no parameters. When it is called, throws new exception.
    /// </summary>
    /// <exception cref="ArgumentException">On call.</exception>
    public ElectricCharger() { }

    /// <summary>
    /// Converts object's information into csv line.
    /// </summary>
    /// <returns>String which represents csv line of object.</returns>
    public string ToCsv()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(
            $"\"\";\"{Id}\";\"{Name}\";\"{AdmArea}\";\"{District}\";\"{Address}\";\"{Longitude}\";\"{Latitude}\";\"{GlobalId}\";\"{GeoDataCenter}\";\"{GeoArea}\";\"\"");
        return sb.ToString();
    }
    
    /// <summary>
    /// Converts object's information into json format. 
    /// </summary>
    /// <returns>String which represents object as a json object.</returns>
    public string ToJson()
    {
        StringBuilder json = new StringBuilder();
        json.Append("  {\n");
        json.Append("    \"object_category_id\": \"\",\n");
        json.Append($"    \"id\": \"{Id}\",\n");
        json.Append($"    \"name\": \"{Name}\",\n");
        json.Append($"    \"admarea\": \"{AdmArea}\",\n");
        json.Append($"    \"district\": \"{District}\",\n");
        json.Append($"    \"longitude_wgs84\": \"{Longitude}\",\n");
        json.Append($"    \"latitude_wgs84\": \"{Latitude}\",\n");
        json.Append($"    \"global_id\": \"{GlobalId}\",\n");
        json.Append($"    \"geodata_center\": \"{GeoDataCenter}\",\n");
        json.Append($"    \"geoarea\": \"{GeoArea}\"\n");
        json.Append("  }");

        return json.ToString();
    }
}
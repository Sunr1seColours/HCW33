using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CsvHelper;
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
    [Index(1)]
    public string Id { get; init; }
    
    /// <summary>
    /// Charger's name. 
    /// </summary>
    [JsonPropertyName("name")]
    [Index(2)]
    public string Name { get; init; }
    
    /// <summary>
    /// Administration area where charger is.
    /// </summary>
    [JsonPropertyName("admarea")]
    [Index(3)]
    public string AdmArea { get; init; }
    
    /// <summary>
    /// District where charger is.
    /// </summary>
    [JsonPropertyName("district")]
    [Index(4)]
    public string District { get; init; }
    
    /// <summary>
    /// Address where charger is.
    /// </summary>
    [JsonPropertyName("address")]
    [Index(5)]
    public string Address { get; init; }
    
    /// <summary>
    /// Longitude of charger. Double number.
    /// </summary>
    [JsonPropertyName("longitude_wgs84")]
    [Index(6)]
    public string Longitude { get; init; }
    
    /// <summary>
    /// Latitude of charger. Double number.
    /// </summary>
    [JsonPropertyName("latitude_wgs84")]
    [Index(7)]
    public string Latitude { get; init; }
    
    /// <summary>
    /// Global ID of charger. Integer number.
    /// </summary>
    [JsonPropertyName("global_id")]
    [Index(8)]
    public string GlobalId { get; init; }
    
    /// <summary>
    /// Information about geo data center.
    /// </summary>
    [JsonPropertyName("geodata_center")]
    [Index(9)]
    public string GeoDataCenter { get; init; }
    
    /// <summary>
    /// Information about geo area.
    /// </summary>
    [JsonPropertyName("geoarea")]
    [Index(10)]
    public string GeoArea { get; init; }
    
    /// <summary>
    /// Default constructor with no parameters. When it is called, throws new exception.
    /// </summary>
    /// <exception cref="ArgumentException">On call.</exception>
    public ElectricCharger() { }

    public string ToCsv()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(
            $"\"\";\"{Id}\";\"{Name}\";\"{AdmArea}\";\"{District}\";\"{Address}\";\"{Longitude}\";\"{Latitude}\";\"{GlobalId}\";\"{GeoDataCenter}\";\"{GeoArea}\";\"\"");
        return sb.ToString();
    }
    
    public string ToJson()
    {
        StringBuilder json = new StringBuilder();
        json.Append("  {\n");
        json.Append($"    \"object_category_id\": \"\",\n");
        json.Append($"    \"id\": \"{Id}\",\n");
        json.Append($"    \"name\": \"{Name}\",\n");
        json.Append($"    \"admarea\": {AdmArea},\n");
        json.Append($"    \"district\": {District},\n");
        json.Append($"    \"longitude_wgs84\": {Longitude},\n");
        json.Append($"    \"latitude_wgs84\": \"{Latitude}\",\n");
        json.Append($"    \"global_id\": {GlobalId},\n");
        json.Append($"    \"geodata_center\": {GeoDataCenter},\n");
        json.Append($"    \"geoarea\": {GeoArea}\n");
        json.Append("  }");

        return json.ToString();
    }
}
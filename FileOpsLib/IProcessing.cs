using ElectricChargesLib;

namespace FileOpsLib;

/// <summary>
/// Interface with methods for converting from information from Stream to array of ElectricCharger objects and back.
/// </summary>
public interface IProcessing
{
    /// <summary>
    /// Converts Stream to array of ElectricCharger objects.
    /// </summary>
    /// <param name="stream">Stream to convert.</param>
    /// <returns>Collection with ElectricCharger objects.</returns>
    public ElectricCharger[] Read(Stream stream);

    /// <summary>
    /// Converts array of ElectricCharger objects to Stream.
    /// </summary>
    /// <param name="chargers">Array of ElectricCharger objects to convert.</param>
    /// <returns>Stream which represents info about ElectricCharger objects.</returns>
    public Stream Write(ElectricCharger[] chargers);
}
namespace ElectricChargesLib;

/// <summary>
/// Class which makes sorting.
/// </summary>
public class Sorter
{
    /// <summary>
    /// Sorts collection of objects.
    /// </summary>
    /// <param name="chargers">Array of ElectricCharger objects which will be sorted.</param>
    /// <param name="isAlphabetical">True if sorting will be in alphabetical order.</param>
    /// <returns>Sorted collection of ElectricCharger objects.</returns>
    public ElectricCharger[] Sort(ElectricCharger[] chargers, bool isAlphabetical)
    {
        ElectricCharger[] sorted = chargers.OrderBy(charger => charger.AdmArea).ToArray();
        if (!isAlphabetical)
        {
            Array.Reverse(sorted);
        }

        return sorted;
    }
}
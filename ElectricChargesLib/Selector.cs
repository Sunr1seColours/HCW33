namespace ElectricChargesLib;

/// <summary>
/// Class which makes selection.
/// </summary>
public class Selector
{
    /// <summary>
    /// Select some objects from collection.
    /// </summary>
    /// <param name="chargers">Array of ElectricCharger objects.</param>
    /// <param name="selectionType">Number which represents by which parameter selection will be.</param>
    /// <param name="valueToSelect">Array of values for selection.</param>
    /// <returns>Array of ElectricCharger objects which were selected.</returns>
    /// <exception cref="ArgumentException">There aren't any object which was selected.
    /// Means that value for selection was wrong.</exception>
    public ElectricCharger[] Select(ElectricCharger[] chargers, int selectionType, string[] valueToSelect)
    {
        List<ElectricCharger> selected = new List<ElectricCharger>();
        if (valueToSelect.Length == 1)
        {
            switch (selectionType)
            {
                case 1:
                    selected = chargers.Where(charger => charger.AdmArea == valueToSelect[0]).ToList();
                    break;
                case 2:
                    selected = chargers.Where(charger => charger.District == valueToSelect[0]).ToList();
                    break;
            }
        }
        else if (valueToSelect.Length == 3)
        {
            selected = chargers.Where(charger => charger.AdmArea == valueToSelect[0] && 
                                                  charger.Longitude == valueToSelect[1] &&
                                                  charger.Latitude == valueToSelect[2]).ToList();
        }
        else
        {
            throw new ArgumentException("Неправильные параметры для выборки.");
        }

        if (selected.Count == 0)
        {
            throw new ArgumentException("Ничего не найдено. Начни заново");
        }

        return selected.ToArray();
    }
}
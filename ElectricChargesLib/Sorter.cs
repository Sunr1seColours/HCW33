namespace ElectricChargesLib;

public class Sorter
{
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
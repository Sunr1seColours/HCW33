namespace ElectricChargesLib;

public class Selector
{
    public ElectricCharger[] Select(ElectricCharger[] chargers, int selectionParameter, string[] valueToSelect)
    {
        List<ElectricCharger> selected = new List<ElectricCharger>();
        if (valueToSelect.Length == 1)
        {
            switch (selectionParameter)
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
            throw new ArgumentException("Wrong parameters for selection.");
        }

        if (selected.Count == 0)
        {
            throw new ArgumentException("There is no data you're looking for.");
        }

        return selected.ToArray();
    }
}
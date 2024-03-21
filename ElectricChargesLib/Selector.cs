namespace ElectricChargesLib;

public class Selector
{
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
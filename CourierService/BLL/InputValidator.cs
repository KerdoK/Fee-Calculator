namespace BLL;

public class InputValidator
{
    public static bool ValidateCity(string city)
    {
        List<string> validCityNames = new List<string> {"TALLINN", "TALLINN-HARKU", 
            "TARTU", "TARTU-TÕRAVERE", "TARTU-TORAVERE", "PÄRNU", "PARNU"};
        
        if (string.IsNullOrWhiteSpace(city))
        {
            return false;
        }

        if (validCityNames.Contains(city.Trim().ToUpper()))
        {
            return true;
        }
        return false;
    }
    public static bool ValidateVehicle(string vehicle)
    {
        List<string> validVehicleTypes = new List<string> {"CAR", "SCOOTER", "BIKE"};
        
        if (string.IsNullOrWhiteSpace(vehicle))
        {
            return false;
        }

        if (validVehicleTypes.Contains(vehicle.Trim().ToUpper()))
        {
            return true;
        }
        return false;
    }
}
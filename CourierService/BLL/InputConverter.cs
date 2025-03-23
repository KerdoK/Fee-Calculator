namespace BLL;

public class InputConverter
{
    public static ECity GetECity(string city)
    {
        List<string> tallinn = new List<string> { "TALLINN", "TALLINN-HARKU" };
        List<string> tartu = new List<string> { "TARTU", "TARTU-TÕRAVERE", "TARTU-TORAVERE" };
        List<string> parnu = new List<string> { "PÄRNU", "PARNU" };

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty");
        }

        if (tallinn.Contains(city.Trim().ToUpper()))
        {
            return ECity.TALLINN;
        }

        if (tartu.Contains(city.Trim().ToUpper()))
        {
            return ECity.TARTU;
        }

        if (parnu.Contains(city.Trim().ToUpper()))
        {
            return ECity.PARNU;
        }

        throw new ArgumentException("Invalid city");
    }

    public static string GetStringCity(ECity city)
    {
        if (ECity.TALLINN.Equals(city))
        {
            return "Tallinn-Harku";
        }

        if (ECity.TARTU.Equals(city))
        {
            return "Tartu-Tõravere";
        }

        if (ECity.PARNU.Equals(city))
        {
            return "Pärnu";
        }

        throw new ArgumentException("Invalid city");
    }

    public static EVehicle GetEVehcile(string vehicle)
    {
        if (string.IsNullOrWhiteSpace(vehicle))
        {
            throw new ArgumentException("vehicle cannot be empty");
        }

        if ("CAR".Equals(vehicle.Trim().ToUpper()))
        {
            return EVehicle.CAR;
        }

        if ("SCOOTER".Equals(vehicle.Trim().ToUpper()))
        {
            return EVehicle.SCOOTER;
        }

        if ("BIKE".Equals(vehicle.Trim().ToUpper()))
        {
            return EVehicle.BIKE;
        }

        throw new ArgumentException("Invalid vehicle");
    }


    public static string GetStringVehicle(EVehicle vehicle)
    {
        if (EVehicle.CAR.Equals(vehicle))
        {
            return "CAR";
        }

        if (EVehicle.SCOOTER.Equals(vehicle))
        {
            return "SCOOTER";
        }

        if (EVehicle.BIKE.Equals(vehicle))
        {
            return "BIKE";
        }

        throw new ArgumentException("Invalid vehicle");
    }
}
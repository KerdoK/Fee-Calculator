namespace BLL;

public class FeeCalculator
{
    public static double CalculateFee(ECity city, EVehicle vehicle, double? airTemp, double? windSpeed,
        string? phenomenon)
    {
        var rbf = CalculateRbf(city, vehicle);
        var atef = CalculateAtef(vehicle, airTemp);
        var wsef = CalcuteWsef(vehicle, windSpeed);
        var wpef = CalculateWpef(vehicle, phenomenon);

        var fee = rbf + atef + wsef + wpef;
        return fee;
    }

    private static double CalculateWpef(EVehicle vehicle, string? phenomenon)
    {
        if (!string.IsNullOrWhiteSpace(phenomenon) &&
            (EVehicle.SCOOTER.Equals(vehicle) || EVehicle.BIKE.Equals(vehicle)))
        {
            if ("GLAZE".Equals(phenomenon.ToUpper()) || "HAIL".Equals(phenomenon.ToUpper()) ||
                "THUNDER".Equals(phenomenon.ToUpper()))
            {
                throw new Exception("Usage of selected vehicle type is forbidden");
            }

            if ("SNOW".Equals(phenomenon.ToUpper()) || "sleet".Equals(phenomenon.ToUpper()))
            {
                return 1;
            }

            if ("RAIN".Equals(phenomenon.ToUpper()))
            {
                return 0.5;
            }
        }

        return 0.0;
    }

    private static double CalcuteWsef(EVehicle vehicle, double? windSpeed)
    {
        if (windSpeed is not null && EVehicle.BIKE.Equals(vehicle))
        {
            if (10 <= windSpeed && windSpeed <= 20)
            {
                return 0.5;
            }

            if (20 < windSpeed)
            {
                throw new Exception("Usage of selected vehicle type is forbidden");
            }
        }

        return 0.0;
    }

    private static double CalculateAtef(EVehicle vehicle, double? airTemp)
    {
        if (airTemp is not null && (EVehicle.SCOOTER.Equals(vehicle) || EVehicle.BIKE.Equals(vehicle)))
        {
            if (airTemp < -10)
            {
                return 1;
            }

            if (-10 <= airTemp && airTemp <= 0)
            {
                return 0.5;
            }
        }

        return 0.0;
    }

    private static double CalculateRbf(ECity city, EVehicle vehicle)
    {
        if (ECity.TALLINN.Equals(city))
        {
            if (EVehicle.CAR.Equals(vehicle))
            {
                return 4;
            }

            if (EVehicle.SCOOTER.Equals(vehicle))
            {
                return 3.5;
            }

            if (EVehicle.BIKE.Equals(vehicle))
            {
                return 3;
            }
        }

        if (ECity.TARTU.Equals(city))
        {
            if (EVehicle.CAR.Equals(vehicle))
            {
                return 3.5;
            }

            if (EVehicle.SCOOTER.Equals(vehicle))
            {
                return 3;
            }

            if (EVehicle.BIKE.Equals(vehicle))
            {
                return 2.5;
            }
        }

        if (ECity.PARNU.Equals(city))
        {
            if (EVehicle.CAR.Equals(vehicle))
            {
                return 3;
            }

            if (EVehicle.SCOOTER.Equals(vehicle))
            {
                return 2.5;
            }

            if (EVehicle.BIKE.Equals(vehicle))
            {
                return 2;
            }
        }
        else
        {
            throw new ArgumentException("Invalid city.");
        }

        return -1;
    }
}
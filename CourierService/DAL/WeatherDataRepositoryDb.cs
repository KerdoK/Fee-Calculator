using Domain;
using System.Globalization;
using System.Xml;

namespace DAL;

public class WeatherDataRepositoryDb
{
    private readonly AppDbContext _context;
    
    public WeatherDataRepositoryDb(AppDbContext context)
    {
        _context = context;
    }
    
    public void FetchWeatherData()
    {
        string xmlPath = "https://www.ilmateenistus.ee/ilma_andmed/xml/observations.php";

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);

        XmlNode? observationsNode = xmlDoc.SelectSingleNode("/observations");
        if (observationsNode == null)
        {
            Console.WriteLine("No observations found");
        }
        else
        {
            var timestampString = observationsNode.Attributes?["timestamp"]?.Value;
            long timestamp = long.TryParse(timestampString, out long timestampValue) ? timestampValue : 0;
            DateTime occurrenceTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
            
            XmlNodeList? stationNodes = xmlDoc.SelectNodes("/observations/station");
            if (stationNodes != null)
            {
                foreach (XmlNode stationNode in stationNodes)
                {
                    string? stationName = stationNode["name"]?.InnerText;
                    if (stationName == "Tallinn-Harku" || stationName == "Tartu-Tõravere" || stationName == "Pärnu")
                    {
                        int? wmocode = Convert.ToInt32(stationNode["wmocode"]?.InnerText);
                        double? airTemperature = Convert.ToDouble(stationNode["airtemperature"]?.InnerText, CultureInfo.InvariantCulture);
                        double? windSpeed = Convert.ToDouble(stationNode["windspeed"]?.InnerText, CultureInfo.InvariantCulture);
                        string? phenomenon = stationNode["phenomenon"]?.InnerText;

                        Console.WriteLine("Station Name: " + stationName);
                        Console.WriteLine("Wmocode: " + wmocode);
                        Console.WriteLine("Air Temperature: " + airTemperature);
                        Console.WriteLine("Wind Speed: " + windSpeed);
                        Console.WriteLine("Phenomenon: " + phenomenon);
                        Console.WriteLine("Occurrence time: " + occurrenceTime);
                        Console.WriteLine();

                        var entry = new WeatherData()
                        {
                            Name = stationName,
                            WmoCode = wmocode,
                            AirTemperature = airTemperature,
                            WindSpeed = windSpeed,
                            WeatherPhenomenon = phenomenon,
                            OccurrenceTime = occurrenceTime,
                        };
                        SaveData(entry);
                    }
                }
            }
            else
            {
                Console.WriteLine("Station data not found in XML.");
            }
        }
    }
    
    private void SaveData(WeatherData weatherData)
    {
        _context.WeatherDataCollection.Add(weatherData);
        _context.SaveChanges();
    }

    public List<WeatherData> GetWeatherDataFromDb()
    {
        return _context.WeatherDataCollection.ToList();
    }
    public WeatherData GetRecentWeatherDataByCity(string city)
    {
        var recentData = _context.WeatherDataCollection
            .Where(w => w.Name != null && w.Name.Equals(city))
            .OrderByDescending(w => w.OccurrenceTime)
            .FirstOrDefault();
        if (recentData != null)
        {
            return recentData;
        }
        throw new ArgumentException($"No entry found for city: {city}");
    }
    
    
}


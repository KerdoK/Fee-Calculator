using BLL;
using DAL;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.EntityFrameworkCore;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;

builder.Services.AddHangfire(config => { config.UseSQLiteStorage(connectionString); });

int workerCount = 1;
builder.Services.AddHangfireServer(options => { options.WorkerCount = workerCount; });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddOpenApi();

var app = builder.Build();


app.UseHangfireDashboard("/hangfire");

var recurringJobId = "Recurring-Job-For-Weather-Data";
var cronExpression = "15 * * * *";
var timeZone = TimeZoneInfo.Local;

RecurringJob.AddOrUpdate<WeatherDataRepositoryDb>(
    recurringJobId,
    job => job.FetchWeatherData(),
    cronExpression,
    new RecurringJobOptions
    {
        TimeZone = timeZone
    }
);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

WeatherDataRepositoryDb repo = new WeatherDataRepositoryDb(new AppDbContext(contextOptions));
// repo.FetchWeatherData(); // to get the most recent weather data

/*
 * Endpoint: GET /weatherdata
 *
 * Shows all the collected weather data from the database.
 *
 * returns: 200 OK - this response contains info about the weather data.
 */
app.MapGet("/weatherdata", () =>
    {
        var data = repo.GetWeatherDataFromDb();
        return data;
    })
    .WithName("GetWeatherData");

/*
 * Endpoint: GET /courierfee
 *
 * Calculates the delivery fee based on weather data from the specified city and the vehicle type
 *
 * param: city - the name of the city we want to deliver in.
 * param: vehicle - the type of vehicle we want to use for deliveries.
 *
 * returns: 200 OK - this response contains info about the fee amount, currency used and a success message.
 * returns: 400 Bad Request - this response contains info about what went wrong by giving an error message.
 */
app.MapGet("/courierfee", (string city, string vehicle) =>
    {
        if (!InputValidator.ValidateCity(city))
        {
            return Results.BadRequest("Invalid city");
        }

        if (!InputValidator.ValidateVehicle(vehicle))
        {
            return Results.BadRequest("Invalid vehicle");
        }

        var eCity = ECity.EMPTY;
        var eVehicle = EVehicle.EMPTY;
        try
        {
            eCity = InputConverter.GetECity(city);
            eVehicle = InputConverter.GetEVehcile(vehicle);
        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(e.Message);
        }

        try
        {
            var data = repo.GetRecentWeatherDataByCity(InputConverter.GetStringCity(eCity));

            var fee = FeeCalculator.CalculateFee(eCity, eVehicle, data.AirTemperature, data.WindSpeed,
                data.WeatherPhenomenon);

            var feeResponse = new FeeResponse
            {
                Fee = fee,
                Currency = "€",
                Message = $"Successfully calculated fee based on city: {city} and vehicle type: {vehicle}."
            };
            return Results.Ok(feeResponse);
        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(e.Message);
        }
    })
    .WithName("GetCourierFee");

app.Run();
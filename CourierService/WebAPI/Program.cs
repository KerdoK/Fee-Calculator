using BLL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web.Resource;
using DAL;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.EntityFrameworkCore;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);


// database connection
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


// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire");

// Step 4: Define a cron job (e.g., fetching weather data every hour)
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

WeatherDataRepositoryDb repo = new WeatherDataRepositoryDb(new AppDbContext(contextOptions));
// repo.FetchWeatherData();


app.MapGet("/weatherdata", () =>
    {
        var data = repo.GetWeatherDataFromDb();
        return data;
    })
    .WithName("GetWeatherData");

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
   // Install or update tools
   dotnet tool install --global dotnet-ef
   dotnet tool install --global dotnet-aspnet-codegenerator
   dotnet tool update --global dotnet-ef
   dotnet tool update --global dotnet-aspnet-codegenerator

   // Create migration
   dotnet ef migrations add FirstMig --project DAL --startup-project WebAPI

   // Remove migration. Works before updating.
   dotnet ef migrations remove --project DAL --startup-project WebAPI

   // Update database
   dotnet ef database update --project DAL --startup-project WebAPI


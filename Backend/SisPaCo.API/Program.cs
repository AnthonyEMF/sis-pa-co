using ExamenLenguajes2.API;
using ExamenLenguajes2.API.Database;
using ExamenLenguajes2.API.Database.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

// Configuración para cargar el Seeder
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var loggerFactory = services.GetRequiredService<ILoggerFactory>();

	try
	{
		var context = services.GetRequiredService<SisPaCoContext>();
		var userManager = services.GetRequiredService<UserManager<UserEntity>>();
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		await SisPaCoSeeder.LoadDataAsync(context, userManager, roleManager, loggerFactory);
	}
	catch (Exception e)
	{
		var logger = loggerFactory.CreateLogger<Program>();
		logger.LogError(e, "Error al ejecutar el Seed de datos");
	}
}

app.Run();

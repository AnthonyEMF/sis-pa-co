using ExamenLenguajes2.API.Database;
using ExamenLenguajes2.API.Database.Entities;
using ExamenLenguajes2.API.Helpers;
using ExamenLenguajes2.API.Middlewares;
using ExamenLenguajes2.API.Services;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExamenLenguajes2.API
{
	public class Startup
	{
		private IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			services.AddHttpContextAccessor();

			var name = Configuration.GetConnectionString("DefaultConnection");

			// DbContext
			services.AddDbContext<SisPaCoContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
			services.AddDbContext<SisPaCoLogsContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("LogsConnection")));

			// Custom Services
			services.AddTransient<IAuthService, AuthService>();
			services.AddTransient<IAuditService, AuditService>();
			services.AddTransient<IBalancesService, BalancesService>();
			services.AddTransient<ITransactionsService, TransactionsService>();
			services.AddTransient<IAccountsService, AccountsService>();

			// Logs Service
			//services.AddTransient<ILogsService, LogsService>();
			services.AddScoped<ILogsService, LogsService>();

			// Security Identity
			services.AddIdentity<UserEntity, IdentityRole>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
			}).AddEntityFrameworkStores<SisPaCoContext>()
			  .AddDefaultTokenProviders();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = false,
					ValidAudience = Configuration["JWT:ValidAudience"],
					ValidIssuer = Configuration["JWT:ValidIssuer"],
					ClockSkew = TimeSpan.Zero,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
				};
			});

			// AutoMapper
			services.AddAutoMapper(typeof(AutoMapperProfile));

			// CORS Configuration
			services.AddCors(opt =>
			{
				var allowURLS = Configuration.GetSection("AllowURLS").Get<string[]>();

				opt.AddPolicy("CorsPolicy", builder => builder
				.WithOrigins(allowURLS)
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials());
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			//app.UseMiddleware<LoggingMiddleware>();

			app.UseRouting();

			app.UseCors("CorsPolicy");

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

	}
}

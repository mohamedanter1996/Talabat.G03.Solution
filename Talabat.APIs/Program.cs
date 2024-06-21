using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Repository;
using Talabat.Repository._Identity;
using Talabat.Repository.Data;
using Talabat.Service.AuthService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Talabat.APIs
{
	public class Program
	{
		public static async Task Main(string[] args)
		{

			//StoreContext dbContext = new StoreContext();

			//await dbContext.Database.MigrateAsync(); // Update Database

			var WebApplicationBuilder = WebApplication.CreateBuilder(args);

			#region Configure Services
			// Add services to the DI container.

			WebApplicationBuilder.Services.AddControllers().AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			});
			// Register Required Web APIs Services to the DI Container
		    
			WebApplicationBuilder.Services.AddSwaggerServices();

			WebApplicationBuilder.Services.AddApplicationServices();
			WebApplicationBuilder.Services.AddDbContext<StoreContext>(options => { options.UseSqlServer(WebApplicationBuilder.Configuration.GetConnectionString("DefaultConnection")); });

			WebApplicationBuilder.Services.AddDbContext<ApplicationIdentityDbContext>(options => { options.UseSqlServer(WebApplicationBuilder.Configuration.GetConnectionString("IdentityConnection")); });
			WebApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((ServiseProvider) => {
				var connection = WebApplicationBuilder.Configuration.GetConnectionString("Redis");
				return ConnectionMultiplexer.Connect(connection); }
			);

			WebApplicationBuilder.Services.AddAuthServices(WebApplicationBuilder.Configuration);

			WebApplicationBuilder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			WebApplicationBuilder.Services.AddCors(options =>
			{
				options.AddPolicy("myPolicy", policyOptions =>
				{
					policyOptions.AllowAnyHeader().AllowAnyMethod().WithOrigins(WebApplicationBuilder.Configuration["FrontBaseUrl"]);
				});
			});

			#endregion


			var app = WebApplicationBuilder.Build();

			#region Apply All Pending Migrations [Update-Database] and Data Seeding
			using var scope = app.Services.CreateScope();

			var services = scope.ServiceProvider;

			var _dbContext = services.GetRequiredService<StoreContext>();

			var _IdentityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();

			var loggerFactory = services.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger<Program>();


			try
			{
				await _dbContext.Database.MigrateAsync(); //Update-Database
				await _IdentityDbContext.Database.MigrateAsync();
				await StoreContextSeed.SeedAsync(_dbContext); //Data Seeding
				var _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
				await ApplicationIdentityContextSeed.SeedUserAsync(_userManager);
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex);

				logger.LogError(ex, "an error has been occured during apply the migration");
			} 
			#endregion
			#region Configure Kestrel Middlewares
			// Configure the HTTP request pipeline.
			//	app.UseMiddleware<ExceptionMiddleware>();

		app.Use(async (httpContext, _next) =>
		{
			try
			{
		
				// Take an Action With the Request
				await _next.Invoke(httpContext); // Go To the next Middleware
												 // Take an Action with The Response
			}
			catch (Exception ex)
			{
		
				logger.LogError(ex.Message);// Development Env
											 // Log Exception in (Database | Files) // Production Env
		
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				httpContext.Response.ContentType = "application/json";
			
				var response = app.Environment.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
				var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
				var json = System.Text.Json.JsonSerializer.Serialize(response, options);
			
				httpContext.Response.WriteAsync(json);
			}
		});

			if (app.Environment.IsDevelopment())
			{
				app.UseSwaggerMiddlewares();
			}
			app.UseStatusCodePagesWithReExecute("/errors/{0}");
			app.UseHttpsRedirection();

			app.UseStaticFiles();


			app.UseRouting();
	
			app.UseAuthorization();
			//app.UseEndpoints(endpoints => { endpoints.MapControllerRoute(name: "defult", pattern: "{controller}/{action}/{id?}");endpoints.MapControllers(); });
			app.UseCors("myPolicy");
			app.MapControllers();
			#endregion

			app.Run();
		}
	}
}

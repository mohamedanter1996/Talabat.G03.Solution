using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;

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

			WebApplicationBuilder.Services.AddControllers();
			// Register Required Web APIs Services to the DI Container
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			WebApplicationBuilder.Services.AddEndpointsApiExplorer();
			WebApplicationBuilder.Services.AddSwaggerGen();

			WebApplicationBuilder.Services.AddDbContext<StoreContext>(options => { options.UseSqlServer(WebApplicationBuilder.Configuration.GetConnectionString("DefaultConnection")); });

			///WebApplicationBuilder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();

			///WebApplicationBuilder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();

			///WebApplicationBuilder.Services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();

			WebApplicationBuilder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			//builder.Services.AddAutoMapper(M => M.AddProfile( new MappingProfiles()));
			WebApplicationBuilder.Services.AddAutoMapper(typeof(MappingProfiles));
			#endregion


			var app = WebApplicationBuilder.Build();

			using var scope = app.Services.CreateScope();

			var services = scope.ServiceProvider;

			var _dbContext = services.GetRequiredService<StoreContext>();


			var loggerFactory =services.GetRequiredService<ILoggerFactory>();

			try
			{
				await _dbContext.Database.MigrateAsync(); //Update-Database

				await StoreContextSeed.SeedAsync(_dbContext); //Data Seeding
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex);

				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex, "an error has been occured during apply the migration");
			}
			#region Configure Kestrel Middlewares
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseRouting();
			//app.UseEndpoints(endpoints => { endpoints.MapControllerRoute(name: "defult", pattern: "{controller}/{action}/{id?}");endpoints.MapControllers(); });

			app.MapControllers();
			#endregion

			app.Run();
		}
	}
}

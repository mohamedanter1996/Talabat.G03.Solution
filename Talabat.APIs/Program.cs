using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Talabat.Repository.Data;

namespace Talabat.APIs
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var WebApplicationBuilder = WebApplication.CreateBuilder(args);

			#region Configure Services
			// Add services to the DI container.

			WebApplicationBuilder.Services.AddControllers();
			// Register Required Web APIs Services to the DI Container
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			WebApplicationBuilder.Services.AddEndpointsApiExplorer();
			WebApplicationBuilder.Services.AddSwaggerGen();

			WebApplicationBuilder.Services.AddDbContext<StoreContext>(options => { options.UseSqlServer(WebApplicationBuilder.Configuration.GetConnectionString("DefaultConnection")); }); ;	
			#endregion


			var app = WebApplicationBuilder.Build();

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

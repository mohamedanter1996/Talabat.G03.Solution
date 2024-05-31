using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Repository;
using Talabat.Service.AuthService;

namespace Talabat.APIs.Extensions
{
	public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			//services.AddScoped(IBasketRepository,BasketRepository);
			services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
			///services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();

			///services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();

			///services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();

			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			//services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));

			services.AddAutoMapper(typeof(MappingProfiles));

			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0).SelectMany(P => P.Value.Errors).Select(P => P.ErrorMessage).ToList();
					var response = new ApivalidationErrorResponse()
					{
						Errors = errors
					};

					return new BadRequestObjectResult(response);
				};


			}

			);

			return services;
		}

		public static IServiceCollection AddAuthServices(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddAuthentication(/*JwtBearerDefaults.AuthenticationScheme*/options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidIssuer = configuration["JWT:ValidIssuer"],
					ValidateAudience = true,
					ValidAudience = configuration["JWT:ValidAudience"],
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AuthKey"] ?? string.Empty)),
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero,
				};
			});

			services.AddScoped(typeof(IAuthService), typeof(AuthService));

			return services;
		}
	}
}

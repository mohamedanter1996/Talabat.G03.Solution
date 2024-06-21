﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Service.Contract;

namespace Talabat.APIs.Helpers
{
	public class CachedAttribute : Attribute, IAsyncActionFilter
	{
		private readonly int _timeToLiveInSeconds;

		public CachedAttribute(int timeToLiveInSeconds)
        {
			_timeToLiveInSeconds = timeToLiveInSeconds;
		}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var responseCacheService= context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
			//Ask CLR for Creating Object From "ResponseCacheService" Explicitly.
			var cacheKey= GenerateCacheKeyFromRequest(context.HttpContext.Request);
			var response = await responseCacheService.GetCachedResponseAsync(cacheKey);

			if (!string.IsNullOrEmpty(response))
			{
				var result = new ContentResult()
				{
					Content = response,
					ContentType = "application/json",
					StatusCode = 200,
				};

				context.Result = result;
				return;
			}

			//Response is Not Cached
			var executedActionContext = await next.Invoke(); // Will Execute The Next Action Filter OR The Action Itself

			if (executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null) 
			{
				await responseCacheService.CachResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
			}
		}

		private string GenerateCacheKeyFromRequest(HttpRequest request)
		{
			// {{url}}/api/products?pageIndex=1&pageSize=5&sort=name
			var keyBuilder = new StringBuilder();

			keyBuilder.Append(request.Path);

			foreach (var (key, value) in request.Query.OrderBy(x=>x.Key))
			{
				keyBuilder.Append($"|{key}-{value}");
			}
			return keyBuilder.ToString();
		}
	}
}

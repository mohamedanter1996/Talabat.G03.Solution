﻿using AutoMapper;
using Microsoft.Extensions.Configuration;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using static System.Net.WebRequestMethods;

namespace Talabat.APIs.Helpers
{
	public class ProductPictureUrlReSolver : IValueResolver<Product, ProductToReturnDto, string>
	{
		private readonly IConfiguration _configuration;
		public ProductPictureUrlReSolver(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
		{
			if (!string.IsNullOrEmpty(source.PictureUrl))
			{
				//	return $"{_configuration["ApiBaseUrl"]}/{source.PictureUrl}";
				return $"{_configuration["ApiBaseUrl"]}/{source.PictureUrl}";
			}
			return string.Empty;
		}

	}
}


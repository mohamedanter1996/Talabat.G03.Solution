using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
	public static class StoreContextSeed
	{
		public static async Task SeedAsync(StoreContext _dbContext)
		{
			if (_dbContext.ProductBrands.Count() == 0)
			{
				var brandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
				var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
				//brands=brands?.Select(b=>new ProductBrand() { Name=b.Name}).ToList();
				if (brands?.Count() > 0)
				{
					foreach (var brand in brands)
					{
						_dbContext.Set<ProductBrand>().Add(brand);
					}
					await _dbContext.SaveChangesAsync();
				}
			}

			if (_dbContext.ProductCategories.Count() == 0)
			{
				var categoriesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");
				var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesData);
				//categories=categories?.Select(b=>new ProductCategory() { Name=b.Name}).ToList();
				if (categories?.Count() > 0)
				{
					foreach (var category in categories)
					{
						_dbContext.Set<ProductCategory>().Add(category);
					}
					await _dbContext.SaveChangesAsync();
				}
			}

			if (_dbContext.Products.Count() == 0)
			{
				var ProductsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
				var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
				//categories=categories?.Select(b=>new ProductCategory() { Name=b.Name}).ToList();
				if (Products?.Count() > 0)
				{
					foreach (var Product in Products)
					{
						_dbContext.Set<Product>().Add(Product);
					}
					await _dbContext.SaveChangesAsync();
				}
			}

			if (_dbContext.DeliveryMethods.Count() == 0)
			{
				var DeliveryMethodsData = File.ReadAllText("../Talabat.Repository/_Data/DataSeed/delivery.json");
				var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
				//categories=categories?.Select(b=>new ProductCategory() { Name=b.Name}).ToList();
				if (DeliveryMethods?.Count() > 0)
				{
					foreach (var DeliveryMethod in DeliveryMethods)
					{
						_dbContext.Set<DeliveryMethod>().Add(DeliveryMethod);
					}
					await _dbContext.SaveChangesAsync();
				}
			}
		}

	}
}

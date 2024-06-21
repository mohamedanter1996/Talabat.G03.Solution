using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.Service.ProductService
{
	public class ProductService : IProductService
	{
		private readonly IUnitOfWork _unitOfWork;

		public ProductService(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}

		public  async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams specParams)
		{
			var Spec = new ProductWithBrandAndCategorySpecifications(specParams);
			var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
			return products;
		}
		public async Task<Product?> GetProductAsync(int productId)
		{
			var Spec = new ProductWithBrandAndCategorySpecifications(productId);
			var product = await _unitOfWork.Repository<Product>().GetWithSpecAsync(Spec);
			return product;
		}
		public async Task<int> GetCountAsync(ProductSpecParams specParams)
		{
			var CountSpec = new ProductsWithFilterationForCountSpecifications(specParams);
			var Count = await _unitOfWork.Repository<Product>().GetCountAsync(CountSpec);
			return Count;
		}
		public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
		{
			var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
			return brands;
		}

		public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
		{
			var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
			return categories;
		}

	}
}

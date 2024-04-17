using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.Product_Specs;
namespace Talabat.APIs.Controllers
{

	public class ProductsController : BaseApiController
	{
		private readonly IGenericRepository<Product> _productRepo;
		private readonly IMapper _mapper;
		public ProductsController(IGenericRepository<Product> ProductRepo, IMapper mapper)
		{
			_productRepo = ProductRepo;
		}

		// api/Products

		[HttpGet]

		public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
		{
			//var products = await _productRepo.GetAllAsync();
			var Spec = new ProductWithBrandAndCategorySpecifications();
			var products = await _productRepo.GetByIdWithSpecAsync(Spec);
			return Ok(products);
		}

		// api/Products/1

		[HttpGet("{id}")]

		public async Task<ActionResult<Product>> GetProduct(int id)
		{
			//	var product= await _productRepo.GetAsync(id);

			var spec = new ProductWithBrandAndCategorySpecifications(id);
			var product = await _productRepo.GetByIdWithSpecAsync(spec);

			if (product is null)
			{
				//return NotFound(new {Message="Not Found",StatusCode=404}); //404
				return NotFound(new ApiResponse(404));
			}

			return Ok(product); //200
		}
	}

}

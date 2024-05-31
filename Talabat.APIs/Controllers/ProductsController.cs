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
			_mapper = mapper;
		}

		// api/Products

		[HttpGet]

		public async Task<ActionResult<IEnumerable<ProductToReturnDto>>> GetProducts()
		{

			//var products = await _productRepo.GetAllAsync();
			var Spec = new ProductWithBrandAndCategorySpecifications();
			var products = await _productRepo.GetAllWithSpecAsync(Spec);
			return Ok(_mapper.Map<IEnumerable<Product>,IEnumerable<ProductToReturnDto>>(products));
		}

		// api/Products/1
		[ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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

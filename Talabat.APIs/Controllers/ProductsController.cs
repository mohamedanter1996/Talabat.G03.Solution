using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.APIs.Controllers
{

	public class ProductsController : BaseApiController
	{
		private readonly IGenericRepository<Product> _productRepo;
		private readonly IGenericRepository<ProductBrand> _brandsRepo;
		private readonly IGenericRepository<ProductCategory> _categoriesRepo;
		private readonly IMapper _mapper;

		public ProductsController(IGenericRepository<Product> ProductRepo, IGenericRepository<ProductBrand> brandsRepo, IGenericRepository<ProductCategory> categoriesRepo, IMapper mapper)
		{
			_productRepo = ProductRepo;
			_brandsRepo = brandsRepo;
			_categoriesRepo = categoriesRepo;
			_mapper = mapper;
		}


		[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet]// api/Products

		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams specParams)
		{
			var Spec = new ProductWithBrandAndCategorySpecifications(specParams);
			var products = await _productRepo.GetAllWithSpecAsync(Spec);
			var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
			var CountSpec = new ProductsWithFilterationForCountSpecifications(specParams);
			var Count =await _productRepo.GetCountAsync(CountSpec);
			return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex,specParams.PageSize,data,Count));
		}

		// api/Products/1
		[ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

		[HttpGet("{id}")]

		public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)

		{
			var Spec = new ProductWithBrandAndCategorySpecifications(id);
			var product = await _productRepo.GetWithSpecAsync(Spec);

			if (product is null)
			{
				return NotFound(new ApiResponse(404)); //404
			}

			return Ok(_mapper.Map<Product, ProductToReturnDto>(product)); //200
		}

		[HttpGet("brands")]    //api/products/brands

		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var brands=await _brandsRepo.GetAllAsync();

			return Ok(brands);
		}

		[HttpGet("categories")] //api/products/categories

		public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
		{
			var categories=await _categoriesRepo.GetAllAsync(); return Ok(categories);
		}
	}

}

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
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.APIs.Controllers
{

	public class ProductsController : BaseApiController
	{
		//private readonly IGenericRepository<Product> _productRepo;
		//private readonly IGenericRepository<ProductBrand> _brandsRepo;
		//private readonly IGenericRepository<ProductCategory> _categoriesRepo;
		private readonly IProductService _productService;
		private readonly IMapper _mapper;

		public ProductsController(IProductService productService/*IGenericRepository<Product> ProductRepo, IGenericRepository<ProductBrand> brandsRepo, IGenericRepository<ProductCategory> categoriesRepo*/, IMapper mapper)
		{
			//_productRepo = ProductRepo;
			//_brandsRepo = brandsRepo;
			//_categoriesRepo = categoriesRepo;
			_productService = productService;
			_mapper = mapper;
		}


		[CachedAttribute(600)]
		[HttpGet]// api/Products

		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams specParams)
		{
			
			var products = await _productService.GetProductsAsync(specParams);
			var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
		
			var Count =await _productService.GetCountAsync(specParams);
			return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex,specParams.PageSize,data,Count));
		}

		// api/Products/1
		[ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

		[CachedAttribute(600)]
		[HttpGet("{id}")]

		public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
		{
			
			var product = await _productService.GetProductAsync(id);

			if (product is null)
			{
				return NotFound(new ApiResponse(404)); //404
			}

			return Ok(_mapper.Map<Product, ProductToReturnDto>(product)); //200
		}

		[CachedAttribute(600)]
		[HttpGet("brands")]    //api/products/brands

		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var brands=await _productService.GetBrandsAsync();

			return Ok(brands);
		}

		[CachedAttribute(600)]
		[HttpGet("categories")] //api/products/categories

		public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
		{
			var categories=await _productService.GetCategoriesAsync(); return Ok(categories);
		}
	}

}

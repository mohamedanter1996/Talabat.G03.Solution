using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.APIs.Controllers
{
	public class BasketController : BaseApiController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;

		public BasketController(IBasketRepository basketRepository,IMapper mapper)
		{
			_basketRepository = basketRepository;
			_mapper = mapper;
		}

		[HttpGet] // api/Basket?id=

		public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
		{
			var basket = await _basketRepository.GetBasketAsync(id);
			return Ok(basket ?? new CustomerBasket(id));
		}

		[HttpPost] //api/Basket

		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
		{
			var mappedBasket=_mapper.Map<CustomerBasketDto,CustomerBasket>(basket);
			var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);

			if (CreatedOrUpdatedBasket is null) { return BadRequest(new ApiResponse(400)); }
			return Ok(CreatedOrUpdatedBasket);
		}

		[HttpDelete] //api/Basket?id=

		public async Task DeleteBasket(string id)
		{
			await _basketRepository.DeleteBasketAsync(id);
		}

	}
}

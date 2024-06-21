using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Service.Contract;

namespace Talabat.APIs.Controllers
{

	public class OrdersController : BaseApiController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;

		public OrdersController(IOrderService orderService, IMapper mapper)
		{
			_orderService = orderService;
			_mapper = mapper;
		}
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[ProducesResponseType(typeof(OrderToReturndDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost] //POST :api/Orders
		public async Task<ActionResult<OrderToReturndDto>> CreateOrder(OrderDto orderDto)
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			var address = _mapper.Map<AddressDto, Talabat.Core.Entities.Order_Aggregate.OrderAddress>(orderDto.ShippingAddress);
			var order = await _orderService.CreateOrderAsync(email, orderDto.BasketId, orderDto.DeliveryMethodId, address);
			if (order is null) { return BadRequest(new ApiResponse(400)); }

			return Ok(_mapper.Map<OrderToReturndDto>(order));
		}

		[HttpGet] //GET: api/Orders?email=ahmed.nasr@linkdev.com
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<ActionResult<IReadOnlyList<OrderToReturndDto>>> GetOrdersForUser()
		{
			var email= User.FindFirstValue(ClaimTypes.Email);
			var orders = await _orderService.GetOrdersForUserAsync(email);

			return Ok(_mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturndDto>>(orders));
		}
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[ProducesResponseType(typeof(OrderToReturndDto),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
		[HttpGet("{id}")] //GET:api/Orders/1?email=ahmed.nasr@linkdev.com
		public async Task<ActionResult<OrderToReturndDto>> GetOrderForUser(int id)
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			var order = await _orderService.GetOrderByIdForUserAsync(email, id);

			if (order is null) { return NotFound(new ApiResponse(404)); }
			return Ok(_mapper.Map<OrderToReturndDto>(order));
		}

		[HttpGet("deliveryMethods")] //GET : /api/Orders/deliveryMethods
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
		{
			var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();
			return Ok(deliveryMethods);
		}
	}

}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Stripe;
using System.Diagnostics;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Service.Contract;

namespace Talabat.APIs.Controllers
{

	public class PaymentController : BaseApiController
	{
		private readonly IPaymentService _paymentService;
		private readonly ILogger<PaymentController> _logger;

		// This is your Stripe CLI webhook secret for testing your endpoint locally.
		private const string whSecret = "whsec_edbd15a83e655bec9578e92c3e49f1127372d445eb74a2fa12cc57651d4b2a5f";
		public PaymentController(IPaymentService paymentService,ILogger<PaymentController> logger)
		{
			_paymentService = paymentService;
			_logger = logger;
		}
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpGet("{basketId}")] // GET: /api/Payment/{basketId}
		public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			if (basket is null) { return BadRequest(new ApiResponse(400, "An Error with your Basket")); }
			return Ok(basket);
		}


		[HttpPost("webhook")]
		public async Task<IActionResult> WebHook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], whSecret);

			var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

			Order? order;
			// Handle the event
			switch (stripeEvent.Type)
				{
					case Events.PaymentIntentSucceeded:
					order=	await _paymentService.UpdateOrderStatus(paymentIntent.Id, true);
					_logger.LogInformation("Order is Succeeded ya Hamada {0}", order?.PaymentIntentId);
					_logger.LogInformation("Unhandled event type: {0}", stripeEvent.Type);
					break;

					case Events.PaymentIntentPaymentFailed:
					order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, false);
					_logger.LogInformation("Order is Failed ya Hamada {0}", order?.PaymentIntentId);
					_logger.LogInformation("Unhandled event type: {0}", stripeEvent.Type);
					break;
				}
				///if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
				///{
				///}
				///else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
				///{
				///}
				/// ... handle other event types
				///else
				///{
				///	Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
				///}

				return Ok();
			
			
		}
	}
}

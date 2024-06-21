using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentService _paymentService;

		//private readonly IGenericRepository<Product> _productRepo;
		//private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
		//private readonly IGenericRepository<Order> _orderRepo;

		public OrderService(IBasketRepository basketRepo,/*IGenericRepository<Product> productRepo,IGenericRepository<DeliveryMethod> deliveryMethodRepo,IGenericRepository<Order> orderRepo*/IUnitOfWork unitOfWork,IPaymentService paymentService)
        {
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
			_paymentService = paymentService;
			//_productRepo = productRepo;
			//_deliveryMethodRepo = deliveryMethodRepo;
			//_orderRepo = orderRepo;
		}
		public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int? deliveryMethodId, Talabat.Core.Entities.Order_Aggregate.OrderAddress shippingAddress)
		{
			// 1.Get Basket From Baskets Repo 

			var basket = await _basketRepo.GetBasketAsync(basketId);

			// 2. Get Selected Items at Basket From Products Repo 
			var orderItems = new List<OrderItem>();

			if (basket?.Items.Count() > 0)
			{
				var productRepository = _unitOfWork.Repository<Product>();
				foreach (var item in basket.Items)
				{

					var product = await productRepository.GetByIdAsync(item.Id);
					var productItemOrder = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
					var orderItem = new OrderItem(productItemOrder, product.Price, item.Quantity);
					orderItems.Add(orderItem);

				}
			}
			// 3. Calculate SubTotal 

			var subtotal = orderItems.Sum(item => item.Price * item.Quantity) ?? default;
			// 4. Get Delivery Method From DeliveryMethods Repo 
				var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId??default);

			var orderRepo = _unitOfWork.Repository<Order>();

			var spec = new orderSpecifications(basket?.PaymentIntentId);

			var existingOrder = await orderRepo.GetWithSpecAsync(spec);

			if (existingOrder is not null)
			{
				orderRepo.Delete(existingOrder);
				await _paymentService.CreateOrUpdatePaymentIntent(basketId);

			}
			// 5. Create Order 
			var order = new Order(
				buyerEmail: buyerEmail,
				//deliveryMethodId: deliveryMethodId,
				deliveryMethod: deliveryMethod,
				shippingAddress: shippingAddress,
				items: orderItems,
				subtotal: subtotal,
				paymentIntentId: basket?.PaymentIntentId??""
				);

			await _unitOfWork.Repository<Order>().AddAsync(order);
			// 6. Save To Database [TODO]
			var result=await _unitOfWork.CompleteAsync();

			if (result <= 0) { return null; }
			return order;
		}



		public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
		=>await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

		public async Task<Order?> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
		{
			var orderRepo= _unitOfWork.Repository<Order>();
			var orderSpec = new orderSpecifications(orderId, buyerEmail);
			var order = await orderRepo.GetWithSpecAsync(orderSpec);
			return order;

		}

		public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
		{
			var ordersRepo = _unitOfWork.Repository<Order>();
			var spec = new orderSpecifications(buyerEmail);

			var orders = await ordersRepo.GetAllWithSpecAsync(spec);
			return orders;
		}
	}
}

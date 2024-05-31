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

namespace Talabat.Service.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;
		//private readonly IGenericRepository<Product> _productRepo;
		//private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
		//private readonly IGenericRepository<Order> _orderRepo;

		public OrderService(IBasketRepository basketRepo,/*IGenericRepository<Product> productRepo,IGenericRepository<DeliveryMethod> deliveryMethodRepo,IGenericRepository<Order> orderRepo*/IUnitOfWork unitOfWork)
        {
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
			//_productRepo = productRepo;
			//_deliveryMethodRepo = deliveryMethodRepo;
			//_orderRepo = orderRepo;
		}
		public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int? deliveryMethodId, Talabat.Core.Entities.Order_Aggregate.OrderAddress shappingAddress)
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

					var product = await productRepository.GetAsync(item.Id);
					var productItemOrder = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
					var orderItem = new OrderItem(productItemOrder, product.Price, item.Quantity);
					orderItems.Add(orderItem);

				}
			}
			// 3. Calculate SubTotal 

			var subtotal = orderItems.Sum(item => item.Price * item.Quantity) ?? default;
			// 4. Get Delivery Method From DeliveryMethods Repo 
				var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId??default);
			// 5. Create Order 
			var order = new Order(
				buyerEmail: buyerEmail,
				//deliveryMethodId: deliveryMethodId,
				deliveryMethod: deliveryMethod,
				shippingAddress: shappingAddress,
				items: orderItems,
				subtotal: subtotal
				);

			await _unitOfWork.Repository<Order>().AddAsync(order);
			// 6. Save To Database [TODO]
			var result=await _unitOfWork.CompleteAsync();

			if (result <= 0) { return null; }
			return order;
		}



		public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Order> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Service.Contract
{
	public interface IOrderService
	{
		public Task<Order?> CreateOrderAsync(string buyerEmail,string basketId,int? deliveryMethodId,Talabat.Core.Entities.Order_Aggregate.OrderAddress shippingAddress);

		public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);

		public Task<Order?> GetOrderByIdForUserAsync(string buyerEmail,int orderId);

		public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
	}
}

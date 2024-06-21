using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIs.Dtos
{
	public class OrderToReturndDto
	{
		public int Id { get; set; }	
		public string BuyerEmail { get; set; } = null!;

		public DateTimeOffset OrderDate { get; set; } 

		public string Status { get; set; } 

		public Talabat.Core.Entities.Order_Aggregate.OrderAddress ShippingAddress { get; set; } = null!;

		
		public string DeliveryMethod { get; set; }

		public decimal DeliveryMethodCost { get; set; }
		public ICollection<OrderItemDto> Items { get; set; }  //Navigational Property [Many]

		public decimal Subtotal { get; set; }

		

		public decimal Total { get; set; }	

		public string PaymentIntentId { get; set; } = string.Empty;
	}
}

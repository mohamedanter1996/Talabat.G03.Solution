using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
	public class Order : BaseEntity
	{
		// There is must be Parameterless Constructor Exist for EF Core
		private Order()
        {
            
        }
        public Order(string buyerEmail, Talabat.Core.Entities.Order_Aggregate.OrderAddress shippingAddress, /*int? deliveryMethodId*/DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subtotal)
		{
			BuyerEmail = buyerEmail;
			ShippingAddress = shippingAddress;
			//DeliveryMethodId = deliveryMethodId;
			DeliveryMethod = deliveryMethod;
			Items = items;
			Subtotal = subtotal;
		}

		public string BuyerEmail { get; set; } = null!;

		public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

		public OrderStatus Status { get; set; } = OrderStatus.Pending;

		public Talabat.Core.Entities.Order_Aggregate.OrderAddress ShippingAddress { get; set; } = null!;

	//	public int? DeliveryMethodId { get; set; } //Foreign Key
		public DeliveryMethod? DeliveryMethod { get; set; } = null!; //Navigational Property [ONE]

		public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>(); //Navigational Property [Many]

		public decimal Subtotal { get; set; }

		//[NotMapped] //عشان ده derived attribute فمش هعمله mapping في ال database
		//public decimal Total => Subtotal + DeliveryMethod.Cost;

		public decimal GetTotal() => Subtotal + DeliveryMethod.Cost;// لازم ال method دي تبداء ب get

		public string PaymentIntentId { get; set; }	= string.Empty;
	}
}

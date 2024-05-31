using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
	public class OrderItem:BaseEntity
	{
        private OrderItem()  //الproject ده عملته عشان ال entityframework core بتحتاج لما تيجي تعمل migration للclassies الي هتتحول ل domains او class مستخدم ك type جوه domain لازم يكون الclass ده فيه empty parametterless constractor
        {
            
        }
        public OrderItem(ProductItemOrdered product, decimal price, int? quantity)
		{
			Product = product;
			Price = price;
			Quantity = quantity;
		}

		public ProductItemOrdered Product { get; set; } = null!;

		public decimal Price { get; set; }

		public int? Quantity { get; set;}
	}
}

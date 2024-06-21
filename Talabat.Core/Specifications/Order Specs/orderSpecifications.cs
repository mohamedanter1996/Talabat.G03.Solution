using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Specs
{
	public class orderSpecifications:BaseSpecifications<Order>
    {
        public orderSpecifications(string buyerEmail) : base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);

            AddOrderByDesc(O => O.OrderDate);



        }

        public orderSpecifications(int orderId, string buyerEmail):base(O=>O.Id==orderId&&O.BuyerEmail==buyerEmail) 
        {
			Includes.Add(O => O.DeliveryMethod);
			Includes.Add(O => O.Items);
		}

       
    }
}

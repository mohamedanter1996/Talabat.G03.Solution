using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles:Profile
	{
		

		public MappingProfiles()
        {

			CreateMap<Product, ProductToReturnDto>().ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand.Name)).ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name)).ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>());

			CreateMap<CustomerBasketDto, CustomerBasket>();

			CreateMap<BasketItemDto, BasketItem>();

			CreateMap<Talabat.Core.Entities.Address, AddressDto>().ReverseMap();
			CreateMap<Talabat.Core.Entities.Order_Aggregate.OrderAddress, AddressDto>().ReverseMap();
			CreateMap<Order, OrderToReturndDto>().ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName)).ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));
			CreateMap<OrderItem, OrderItemDto>().ForMember(d=>d.ProductName,O=>O.MapFrom(s=>s.Product.ProductName)).ForMember(d=>d.ProductId,o=>o.MapFrom(s=>s.Product.ProductId)).ForMember(d=>d.ProductUrl,o=>o.MapFrom(s=>s.Product.ProductUrl)).ForMember(d=>d.ProductUrl,o=>o.MapFrom<OrderItemPictureUrlResolver>());
		}
	}
}

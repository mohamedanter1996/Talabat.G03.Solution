﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;

namespace Talabat.Core
{
	public interface IUnitOfWork : IAsyncDisposable 
	{
		//public IGenericRepository<Product> ProductsRepo { get; set; }

		//public IGenericRepository<ProductBrand> ProductBrandRepo { get; set; }

		//public IGenericRepository<ProductCategory> ProductCategoryRepo { get; set; }

		//public IGenericRepository<DeliveryMethod> DeliveryMethodRepo { get; set; }

		//public IGenericRepository<OrderItem> OrderItemRepo { get; set; }

		//public IGenericRepository<Order> OrdersRepo {  get; set; }

		IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

		Task<int> CompleteAsync();

	}
}

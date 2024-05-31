using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private Hashtable _repositories;
		private readonly StoreContext _dbContext;

		//public IGenericRepository<Product> ProductsRepo {get;set;}
		//public IGenericRepository<ProductBrand> ProductBrandRepo {get;set;}
		//public IGenericRepository<ProductCategory> ProductCategoryRepo {get;set;}
		//public IGenericRepository<DeliveryMethod> DeliveryMethodRepo {get;set;}
		//public IGenericRepository<OrderItem> OrderItemRepo {get;set;}
		//public IGenericRepository<Order> OrdersRepo { get;set;}


		public UnitOfWork(StoreContext dbContext) // Ask CLR for Creating Object from DbContext Implicitly
		{
			_dbContext = dbContext;
			_repositories = new Hashtable();

			//ProductsRepo = new GenericRepository<Product>(_dbContext);
			//ProductBrandRepo= new GenericRepository<ProductBrand>(_dbContext);
			//ProductCategoryRepo=new GenericRepository<ProductCategory>(_dbContext);
			//DeliveryMethodRepo= new GenericRepository<DeliveryMethod>(_dbContext);
			//OrderItemRepo=new GenericRepository<OrderItem>(_dbContext);
			//OrdersRepo=new GenericRepository<Order>(_dbContext);                               //كده اي حد هيكريت object من الunit of work هي initialize كل ال repos دي
		}
		public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
		{
			var key = typeof(TEntity).Name;
			if (!_repositories.ContainsKey(key))
			{
				var repository = new GenericRepository<TEntity>(_dbContext);
				_repositories.Add(key, repository);
			}

			return _repositories[key] as IGenericRepository<TEntity>;       //خدبالك انا عملت الmethod دي عشان ن create ال repository per request

		}
		public async Task<int> CompleteAsync()
		=> await _dbContext.SaveChangesAsync();
		public async ValueTask DisposeAsync()
		=>await _dbContext.DisposeAsync();
	}
}

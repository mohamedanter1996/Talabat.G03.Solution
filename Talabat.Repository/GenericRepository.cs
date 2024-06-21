using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.infrastructure;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly StoreContext _dbContext;

		public GenericRepository(StoreContext dbContext)
        {
			_dbContext = dbContext;
		}
        public async Task<IReadOnlyList<T>> GetAllAsync()
		{
			//if (typeof(T) == typeof(Product))
			//{
			//	return (IEnumerable<T>) await _dbContext.Set<Product>().Include(P => P.Brand).Include(P => P.Category).ToListAsync();
			//}
			return await _dbContext.Set<T>().ToListAsync();
		}

		public async Task<T?> GetByIdAsync(int id)
		{

			//if (typeof(T) == typeof(Product))
			//{
			//	return await _dbContext.Set<Product>().Where(P=>P.Id==id).Include(P => P.Brand).Include(P => P.Category).FirstOrDefaultAsync() as T;
			//}
			return await _dbContext.Set<T>().FindAsync(id);
		}

	

		async Task<IReadOnlyList<T>> IGenericRepository<T>.GetAllWithSpecAsync(Ispecifications<T> spec)
		{
			return await ApplySpecifications(spec).ToListAsync();
		}



		async Task<T?> IGenericRepository<T>.GetWithSpecAsync(Ispecifications<T> spec)
		{
			return await ApplySpecifications(spec).FirstOrDefaultAsync();
		}
		public async Task<int> GetCountAsync(Ispecifications<T> spec)
		{
			return await ApplySpecifications(spec).CountAsync();
		}

		private IQueryable<T> ApplySpecifications(Ispecifications<T> spec)
		{
			return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
		}

		public async Task AddAsync(T entity)=>await _dbContext.Set<T>().AddAsync(entity);
		public void Add(T entity)=> _dbContext.Set<T>().Add(entity);	

		public void Update(T entity)=>_dbContext.Set<T>().Update(entity);


		public void Delete(T entity)=>_dbContext.Set<T>().Remove(entity);	
	}
}

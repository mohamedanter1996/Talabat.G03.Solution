﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories.Contract
{
	public interface IGenericRepository<T> where T : BaseEntity
	{
		Task<T?> GetByIdAsync(int id);

		Task<IReadOnlyList<T>> GetAllAsync();

		Task<IReadOnlyList<T>> GetAllWithSpecAsync(Ispecifications<T> spec);

		Task<T?> GetWithSpecAsync(Ispecifications<T> spec);

		Task<int> GetCountAsync(Ispecifications<T> spec);

		Task AddAsync(T entity);
		void Add(T entity);

		void Update(T entity);	

		void Delete(T entity);



	}
}

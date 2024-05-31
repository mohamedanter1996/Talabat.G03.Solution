using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.infrastructure
{
	internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
	{
		public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,Ispecifications<TEntity> spec)
		{
			var query = inputQuery; //_dbContext.set<Producy>()

			if (spec.Criteria is not null)
			{
				query = query.Where(spec.Criteria);
				// query=_dbContext.set<Producy>().Where(P=>P.Id==1)
				// Includes
				//1. P=>P.Brand
				//2  P=>P.Category


			}

			if (spec.OrderBy is not null) //P=>P.Name
			{
				query = query.OrderBy(spec.OrderBy);
			}

			else if (spec.OrderByDesc is not null) //P=>P.Price
			{
				query = query.OrderByDescending(spec.OrderByDesc);
			}

			if(spec.IsPaginationEnabled==true)
			{
				query=query.Skip(spec.Skip).Take(spec.Take);
			}
			query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

			return query;
		}
	}
}

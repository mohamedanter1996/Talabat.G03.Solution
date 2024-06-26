﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public interface Ispecifications<T> where T : BaseEntity
	{
		public Expression<Func<T, bool>> Criteria { get; set; }/// P=>P.Id==1
		
		public List<Expression<Func<T,Object>>> Includes { get; set; }
	}
}

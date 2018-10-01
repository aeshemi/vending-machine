using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VendingMachine.Repositories
{
	public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, new()
	{
		protected RepositoryDbContext DbContext { get; set; }

		public IEnumerable<T> List()
		{
			return DbContext.Set<T>().AsNoTracking().ToList();
		}

		public T Get(Enum key)
		{
			return DbContext.Find<T>(key);
		}

		void IGenericRepository<T>.Update(T entity)
		{
			DbContext.Entry(entity).State = EntityState.Modified;
			DbContext.SaveChanges();
		}
	}
}

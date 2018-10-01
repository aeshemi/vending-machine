using System;
using System.Collections.Generic;

namespace VendingMachine.Repositories
{
	public interface IGenericRepository<T>
	{
		IEnumerable<T> List();

		T Get(Enum key);

		void Update(T entity);
	}
}

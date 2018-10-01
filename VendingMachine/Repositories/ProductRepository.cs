using VendingMachine.Models;

namespace VendingMachine.Repositories
{
	public class ProductRepository : GenericRepository<Product>, IProductRepository
	{
		public ProductRepository(RepositoryDbContext dbContext)
		{
			DbContext = dbContext;
		}
	}
}

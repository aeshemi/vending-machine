using VendingMachine.Models;

namespace VendingMachine.Repositories
{
	public class CoinRepository : GenericRepository<CoinDetail>, ICoinRepository
	{
		public CoinRepository(RepositoryDbContext dbContext)
		{
			DbContext = dbContext;
		}
	}
}

using Microsoft.EntityFrameworkCore;
using VendingMachine.Models;

namespace VendingMachine.Repositories
{
	public class RepositoryDbContext : DbContext
	{
		public RepositoryDbContext(DbContextOptions<RepositoryDbContext> options) : base(options)
		{
		}

		public DbSet<CoinDetail> Coins { get; set; }

		public DbSet<Product> Products { get; set; }
	}
}

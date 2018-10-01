using System;
using Microsoft.EntityFrameworkCore;
using VendingMachine.Repositories;
using Xunit;

namespace VendingMachine.Tests
{
	public class DbContextFixture : IDisposable
	{
		public RepositoryDbContext Context { get; }

		public DbContextFixture()
		{
			var options = new DbContextOptionsBuilder<RepositoryDbContext>()
				.UseInMemoryDatabase("VendingMachine")
				.Options;

			Context = new RepositoryDbContext(options);

			Context.Coins.AddRange(TestData.Coins);
			Context.Products.AddRange(TestData.Products);

			Context.SaveChanges();
		}

		public void Dispose()
		{
			Context?.Dispose();
		}
	}

	[CollectionDefinition("DbContextCollection")]
	public class DbContextCollection : ICollectionFixture<DbContextFixture>
	{
	}
}

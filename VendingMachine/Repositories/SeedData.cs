using VendingMachine.Models;
using VendingMachine.Models.Enums;

namespace VendingMachine.Repositories
{
	public static class SeedData
	{
		public static void Initialize(RepositoryDbContext context)
		{
			context.Coins.AddRange(
				new CoinDetail
				{
					CoinType = CoinType.TenCent,
					Description = "10 cent",
					Value = 0.10M,
					Quantity = 100
				},
				new CoinDetail
				{
					CoinType = CoinType.TwentyCent,
					Description = "20 cent",
					Value = 0.20M,
					Quantity = 100
				},
				new CoinDetail
				{
					CoinType = CoinType.FiftyCent,
					Description = "50 cent",
					Value = 0.50M,
					Quantity = 100
				},
				new CoinDetail
				{
					CoinType = CoinType.OneEuro,
					Description = "1 euro",
					Value = 1,
					Quantity = 100
				}
			);

			context.Products.AddRange(
				new Product
				{
					ProductType = ProductType.Tea,
					Description = "Tea",
					Price = 1.30M,
					Quantity = 10
				},
				new Product
				{
					ProductType = ProductType.Espresso,
					Description = "Espresso",
					Price = 1.80M,
					Quantity = 20
				},
				new Product
				{
					ProductType = ProductType.Juice,
					Description = "Juice",
					Price = 1.80M,
					Quantity = 20
				},
				new Product
				{
					ProductType = ProductType.ChickenSoup,
					Description = "Chicken Soup",
					Price = 1.80M,
					Quantity = 15
				}
			);

			context.SaveChanges();
		}
	}
}

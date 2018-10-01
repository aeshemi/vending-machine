using System.Collections.Generic;
using VendingMachine.Models;
using VendingMachine.Models.Enums;

namespace VendingMachine.Tests
{
	internal static class TestData
	{
		public static List<CoinDetail> Coins = new List<CoinDetail>
		{
			new CoinDetail
			{
				CoinType = CoinType.TenCent,
				Description = "10 cent",
				Value = 0.10M,
				Quantity = 10
			},
			new CoinDetail
			{
				CoinType = CoinType.TwentyCent,
				Description = "20 cent",
				Value = 0.20M,
				Quantity = 10
			},
			new CoinDetail
			{
				CoinType = CoinType.FiftyCent,
				Description = "50 cent",
				Value = 0.50M,
				Quantity = 10
			},
			new CoinDetail
			{
				CoinType = CoinType.OneEuro,
				Description = "1 euro",
				Value = 1,
				Quantity = 10
			}
		};

		public static List<Product> Products = new List<Product>
		{
			new Product
			{
				ProductType = ProductType.Tea,
				Description = "Tea",
				Price = 1.30M,
				Quantity = 5
			},
			new Product
			{
				ProductType = ProductType.Espresso,
				Description = "Espresso",
				Price = 1.80M,
				Quantity = 5
			},
			new Product
			{
				ProductType = ProductType.Juice,
				Description = "Juice",
				Price = 1.80M,
				Quantity = 5
			},
			new Product
			{
				ProductType = ProductType.ChickenSoup,
				Description = "Chicken Soup",
				Price = 1.80M,
				Quantity = 5
			}
		};
	}
}

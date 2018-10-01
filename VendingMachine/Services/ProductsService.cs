using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Models;
using VendingMachine.Models.Enums;
using VendingMachine.Repositories;

namespace VendingMachine.Services
{
	public class ProductsService : IProductsService
	{
		private readonly IProductRepository productRepository;
		private readonly ICoinsService coinsService;

		public ProductsService(IProductRepository productRepository, ICoinsService coinsService)
		{
			this.productRepository = productRepository;
			this.coinsService = coinsService;
		}

		public IEnumerable<Product> GetProducts()
		{
			return productRepository.List();
		}

		public IEnumerable<Coin> Purchase(ProductType productType, List<Coin> coins)
		{
			var product = productRepository.Get(productType);

			if (product.Quantity < 1)
				throw new ArgumentException("Product is out of stock");

			var amountInserted = CalculateTotal(coins);

			if (product.Price > amountInserted)
				throw new ArgumentException("Insufficient amount");

			var change = amountInserted - product.Price;

			product.Quantity--;
			productRepository.Update(product);

			coinsService.UpdateCoinQuantities(coins);

			return change > 0 ? coinsService.DistributeChange(change) : new List<Coin>();
		}

		private decimal CalculateTotal(IEnumerable<Coin> coins)
		{
			return coins.Aggregate(0M, (current, coin) => current + coinsService.GetValue(coin.CoinType) * coin.Quantity);
		}
	}
}

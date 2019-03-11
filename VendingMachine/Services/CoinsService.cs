using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Models;
using VendingMachine.Models.Enums;
using VendingMachine.Repositories;

namespace VendingMachine.Services
{
	public class CoinsService : ICoinsService
	{
		private readonly ICoinRepository coinRepository;
		private readonly IOrderedEnumerable<CoinDetail> coinDenominations;
		private readonly Dictionary<CoinType, decimal> coinTypeValues;

		public CoinsService(ICoinRepository coinRepository)
		{
			this.coinRepository = coinRepository;

			var coinDetails = coinRepository.List().ToList();

			coinDenominations = coinDetails.OrderByDescending(x => x.Value);
			coinTypeValues = coinDetails.ToDictionary(x => x.CoinType, x => x.Value);
		}

		public IEnumerable<CoinDetail> GetCoins()
		{
			return coinRepository.List();
		}

		public decimal GetValue(CoinType coinType)
		{
			return coinTypeValues.ContainsKey(coinType) ? coinTypeValues[coinType] : 0;
		}

		public void UpdateCoinQuantities(IEnumerable<Coin> coins)
		{
			foreach (var coin in coins)
			{
				var coinDetail = coinRepository.Get(coin.CoinType);
				coinDetail.Quantity += coin.Quantity;
				coinRepository.Update(coinDetail);
			}
		}

		public IEnumerable<Coin> DistributeChange(decimal amount)
		{
			var change = new List<Coin>();

			foreach (var coin in coinDenominations)
			{
				var quantity = 0;
				var coinDetail = coinRepository.Get(coin.CoinType);

				while (coinDetail.Quantity > 0 && amount >= coin.Value)
				{
					quantity++;
					coinDetail.Quantity--;
					amount -= coin.Value;
				}

				if (quantity <= 0) continue;

				coinRepository.Update(coinDetail);
				change.Add(new Coin(coin.CoinType, quantity, coin.Description));
			}

			if (amount > 0) throw new ArgumentException("Insufficient change");

			return change;
		}
	}
}

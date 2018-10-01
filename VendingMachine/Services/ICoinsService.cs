using System.Collections.Generic;
using VendingMachine.Models;
using VendingMachine.Models.Enums;

namespace VendingMachine.Services
{
	public interface ICoinsService
	{
		IEnumerable<CoinDetail> GetCoins();

		decimal GetValue(CoinType coinType);

		void UpdateCoinQuantities(IEnumerable<Coin> coins);

		IEnumerable<Coin> DistributeChange(decimal amount);
	}
}

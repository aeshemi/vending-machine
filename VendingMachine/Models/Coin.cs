using VendingMachine.Models.Enums;

namespace VendingMachine.Models
{
	public class Coin
	{
		public Coin(CoinType coinType, int quantity, string description = null)
		{
			CoinType = coinType;
			Quantity = quantity;
			Description = description;
		}

		public CoinType CoinType { get; set; }

		public int Quantity { get; set; }

		public string Description { get; set; }
	}
}

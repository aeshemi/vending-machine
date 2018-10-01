using System.Collections.Generic;
using VendingMachine.Models.Enums;

namespace VendingMachine.Models
{
	public class PurchaseRequest
	{
		public ProductType ProductType { get; set; }

		public IEnumerable<Coin> Coins { get; set; }
	}
}

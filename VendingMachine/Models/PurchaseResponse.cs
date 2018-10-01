using System.Collections.Generic;

namespace VendingMachine.Models
{
	public class PurchaseResponse
	{
		public PurchaseResponse(IEnumerable<Coin> change, IEnumerable<Product> products)
		{
			Message = "Thank you";
			Change = change;
			Products = products;
		}

		public string Message { get; set; }

		public IEnumerable<Coin> Change { get; set; }

		public IEnumerable<Product> Products { get; set; }
	}
}

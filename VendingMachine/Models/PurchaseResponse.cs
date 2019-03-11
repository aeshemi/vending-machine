using System.Collections.Generic;

namespace VendingMachine.Models
{
	public class PurchaseResponse
	{
		public PurchaseResponse(IEnumerable<Coin> change, IEnumerable<Product> products)
		{
			Success = change != null;
			Message = Success ? "Thank you" : "Insufficient change";
			Change = change;
			Products = products;
		}

		public bool Success { get; set; }

		public string Message { get; set; }

		public IEnumerable<Coin> Change { get; set; }

		public IEnumerable<Product> Products { get; set; }
	}
}

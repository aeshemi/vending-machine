using System.Collections.Generic;
using VendingMachine.Models;
using VendingMachine.Models.Enums;

namespace VendingMachine.Services
{
	public interface IProductsService
	{
		IEnumerable<Product> GetProducts();

		IEnumerable<Coin> Purchase(ProductType productType, List<Coin> coins);
	}
}

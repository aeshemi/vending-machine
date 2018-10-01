using System.ComponentModel.DataAnnotations;
using VendingMachine.Models.Enums;

namespace VendingMachine.Models
{
	public class Product
	{
		[Key]
		public ProductType ProductType { get; set; }

		public string Description { get; set; }

		public decimal Price { get; set; }

		public int Quantity { get; set; }
	}
}

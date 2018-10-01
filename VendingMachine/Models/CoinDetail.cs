using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using VendingMachine.Models.Enums;

namespace VendingMachine.Models
{
	public class CoinDetail
	{
		[Key]
		public CoinType CoinType { get; set; }

		public string Description { get; set; }

		public decimal Value { get; set; }

		[JsonIgnore]
		public int Quantity { get; set; }
	}
}

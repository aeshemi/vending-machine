using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using VendingMachine.Controllers;
using VendingMachine.Models;
using VendingMachine.Services;
using Xunit;

namespace VendingMachine.Tests.Controllers
{
	public class CoinsControllerTests
	{
		private readonly CoinsController coinsController;

		public CoinsControllerTests()
		{
			var mockCoinsService = new Mock<ICoinsService>();
			mockCoinsService.Setup(x => x.GetCoins()).Returns(TestData.Coins.Take(2));

			coinsController = new CoinsController(mockCoinsService.Object);
		}

		[Fact]
		public void List_ShouldReturnCoinDenominations()
		{
			var result = coinsController.List().AssertOkObjectResult().Model<IEnumerable<CoinDetail>>();
			var expected = TestData.Coins.Take(2);

			result.Should().NotBeEmpty()
				.And.HaveCount(2)
				.And.OnlyHaveUniqueItems(x => x.CoinType)
				.And.Equal(expected, (x, y) => 
					x.CoinType == y.CoinType && x.Description == y.Description && x.Value == y.Value && x.Quantity == y.Quantity);
		}
	}
}

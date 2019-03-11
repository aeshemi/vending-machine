using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using VendingMachine.Models;
using VendingMachine.Models.Enums;
using VendingMachine.Repositories;
using VendingMachine.Services;
using Xunit;

namespace VendingMachine.Tests.Services
{
	[Collection("DbContextCollection")]
	public class CoinsServiceTests
	{
		private readonly CoinRepository coinRepository;
		private readonly CoinsService coinsService;

		public CoinsServiceTests(DbContextFixture fixture)
		{
			coinRepository = new CoinRepository(fixture.Context);
			coinsService = new CoinsService(coinRepository);
		}

		[Fact]
		public void GetCoins_ReturnsListofCoinDenominations()
		{
			var result = coinsService.GetCoins();

			result.Should().NotBeEmpty()
				.And.HaveCount(4)
				.And.OnlyHaveUniqueItems(x => x.CoinType);
		}

		public static readonly object[][] CoinTypeValueData =
		{
			new object[] { CoinType.TenCent, 0.10M },
			new object[] { CoinType.TwentyCent, 0.20M },
			new object[] { CoinType.FiftyCent, 0.50M },
			new object[] { CoinType.OneEuro, 1 }
		};

		[Theory, MemberData(nameof(CoinTypeValueData))]
		public void GetValue_ReturnsCoinTypeValue(CoinType coinType, decimal expected)
		{
			coinsService.GetValue(coinType).Should().Be(expected);
		}

		public static readonly object[][] CoinQuantitiesData =
		{
			new object[] { new List<Coin> { new Coin(CoinType.TwentyCent, 2), new Coin(CoinType.TenCent, 9) } },
			new object[] { new List<Coin> { new Coin(CoinType.FiftyCent, 2) } },
			new object[] { new List<Coin> { new Coin(CoinType.OneEuro, 3), new Coin(CoinType.TwentyCent, 5) } }
		};

		[Theory, MemberData(nameof(CoinQuantitiesData))]
		public void UpdateCoinQuantities_ShouldUpdateValues(List<Coin> coins)
		{
			var initialCoins = coinsService.GetCoins().ToList();

			coinsService.UpdateCoinQuantities(coins);

			foreach (var coin in coins)
			{
				var expected = initialCoins.First(x => x.CoinType == coin.CoinType).Quantity + coin.Quantity;
				coinRepository.Get(coin.CoinType).Quantity.Should().Be(expected);
			}
		}

		[Theory]
		[InlineData(.20)]
		[InlineData(1.50)]
		[InlineData(2.70)]
		public void DistributeChange_ReturnsInsufficientChange_IfChangeIsNotEnough(decimal amount)
		{
			var mockCoinsService = SetupMockCoinsService(true);

			mockCoinsService.Invoking(x => x.DistributeChange(amount))
				.Should().Throw<ArgumentException>()
				.WithMessage("Insufficient change");
		}

		public static readonly object[][] DistributeChangeData =
		{
			new object[] { .70M, new List<Coin> { new Coin(CoinType.FiftyCent, 1, "50 cent"), new Coin(CoinType.TwentyCent, 1, "20 cent") } },
			new object[] { .20M, new List<Coin> { new Coin(CoinType.TwentyCent, 1, "20 cent") } },
			new object[] { 0, new List<Coin>() },
			new object[] { 2.20M,  new List<Coin> { new Coin(CoinType.OneEuro, 2, "1 euro"), new Coin(CoinType.TwentyCent, 1, "20 cent") } }
		};

		[Theory, MemberData(nameof(DistributeChangeData))]
		public void DistributeChange_ReturnsCorrectChange(decimal amount, List<Coin> expected)
		{
			coinsService.DistributeChange(amount)
				.Should().NotBeNull()
				.And.HaveCount(expected.Count)
				.And.OnlyHaveUniqueItems(x => x.CoinType)
				.And.Equal(expected, (x, y) => x.CoinType == y.CoinType && x.Description == y.Description && x.Quantity == y.Quantity);
		}

		public static readonly object[][] DistributeChangeEmptyCoinsData =
		{
			new object[] { .70M, new List<Coin> { new Coin(CoinType.TenCent, 7, "10 cent") } },
			new object[] { .20M, new List<Coin> { new Coin(CoinType.TenCent, 2, "10 cent") } },
			new object[] { 2.20M,  new List<Coin> { new Coin(CoinType.OneEuro, 2, "1 euro"), new Coin(CoinType.TenCent, 2, "10 cent") } }
		};

		[Theory, MemberData(nameof(DistributeChangeEmptyCoinsData))]
		public void DistributeChange_ShouldSkipEmptyCoins(decimal amount, List<Coin> expected)
		{
			var mockCoinsService = SetupMockCoinsService();

			mockCoinsService.DistributeChange(amount)
				.Should().NotBeNull()
				.And.HaveCount(expected.Count)
				.And.OnlyHaveUniqueItems(x => x.CoinType)
				.And.Equal(expected, (x, y) => x.CoinType == y.CoinType && x.Description == y.Description && x.Quantity == y.Quantity);
		}

		private CoinsService SetupMockCoinsService(bool singleTenCent = false)
		{
			var emptyCoins = new Dictionary<CoinType, CoinDetail>
			{
				{ CoinType.TenCent, new CoinDetail { CoinType = CoinType.TenCent, Description = "10 cent", Value = 0.10M, Quantity = singleTenCent ? 1 : 10 } },
				{ CoinType.TwentyCent, new CoinDetail { CoinType = CoinType.TwentyCent, Value = 0.20M } },
				{ CoinType.FiftyCent, new CoinDetail { CoinType = CoinType.FiftyCent, Value = 0.50M } },
				{ CoinType.OneEuro, new CoinDetail { CoinType = CoinType.OneEuro, Description = "1 euro", Value = 1, Quantity = 10 } }
			};

			var mockCoinRepository = new Mock<ICoinRepository>();

			mockCoinRepository.Setup(x => x.List()).Returns(emptyCoins.Values);
			mockCoinRepository.Setup(x => x.Get(It.IsAny<CoinType>())).Returns((CoinType x) => emptyCoins[x]);

			return new CoinsService(mockCoinRepository.Object);
		}
	}
}

using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using VendingMachine.Models;
using VendingMachine.Models.Enums;
using VendingMachine.Repositories;
using VendingMachine.Services;
using Xunit;

namespace VendingMachine.Tests.Services
{
	public class ProductsServiceTestsFixture
	{
		internal readonly Mock<ICoinsService> MockCoinsService;
		internal readonly ProductRepository ProductRepository;
		internal readonly ProductsService ProductsService;

		private readonly Dictionary<CoinType, decimal> coinTypeValues = new Dictionary<CoinType, decimal>
		{
			{ CoinType.TenCent, 0.10M },
			{ CoinType.TwentyCent, 0.20M },
			{ CoinType.FiftyCent, 0.50M },
			{ CoinType.OneEuro, 1 }
		};

		public ProductsServiceTestsFixture(DbContextFixture fixture)
		{
			MockCoinsService = new Mock<ICoinsService>();
			MockCoinsService.Setup(x => x.GetValue(It.IsAny<CoinType>())).Returns((CoinType x) => coinTypeValues[x]);

			ProductRepository = new ProductRepository(fixture.Context);
			ProductsService = new ProductsService(ProductRepository, MockCoinsService.Object);
		}
	}

	[Collection("DbContextCollection")]
	public class ProductsServiceTests : IClassFixture<ProductsServiceTestsFixture>, IDisposable
	{
		private readonly Mock<ICoinsService> mockCoinsService;
		private readonly ProductRepository productRepository;
		private readonly ProductsService productsService;

		public ProductsServiceTests(ProductsServiceTestsFixture fixture)
		{
			mockCoinsService = fixture.MockCoinsService;
			productRepository = fixture.ProductRepository;
			productsService = fixture.ProductsService;
		}

		public void Dispose()
		{
			mockCoinsService.Invocations.Clear();
		}

		[Fact]
		public void GetProducts_ReturnsListofProducts()
		{
			var result = productsService.GetProducts();

			result.Should().NotBeEmpty()
				.And.HaveCount(4)
				.And.OnlyHaveUniqueItems(x => x.ProductType);
		}

		public static readonly object[][] InvalidPurchaseData =
		{
			new object[]{ ProductType.Tea, new List<Coin> { new Coin(CoinType.FiftyCent, 2) } },
			new object[]{ ProductType.Espresso, new List<Coin> { new Coin(CoinType.OneEuro, 1), new Coin(CoinType.TenCent, 2) } },
			new object[]{ ProductType.Juice, new List<Coin>() },
			new object[]{ ProductType.ChickenSoup, new List<Coin> { new Coin(CoinType.OneEuro, 1), new Coin(CoinType.TwentyCent, 1) } }
		};

		[Theory, MemberData(nameof(InvalidPurchaseData))]
		public void Purchase_ReturnsInsufficientAmount_IfCoinsIsNotEnough(ProductType productType, List<Coin> coins)
		{
			var productQuantity = productRepository.Get(productType).Quantity;

			productsService.Invoking(x => x.Purchase(productType, coins))
				.Should().Throw<ArgumentException>()
				.WithMessage("Insufficient amount");

			mockCoinsService.Verify(x => x.UpdateCoinQuantities(It.IsAny<IEnumerable<Coin>>()), Times.Never);
			mockCoinsService.Verify(x => x.DistributeChange(It.IsAny<decimal>()), Times.Never);

			productRepository.Get(productType).Quantity.Should().Be(productQuantity);
		}

		[Theory, MemberData(nameof(InvalidPurchaseData))]
		public void Purchase_ReturnsOutOfStock_IfProductIsNotAvailable(ProductType productType, List<Coin> coins)
		{
			var emptyProducts = new Dictionary<ProductType, Product>
			{
				{ ProductType.Tea, new Product() },
				{ ProductType.Espresso, new Product() },
				{ ProductType.Juice, new Product() },
				{ ProductType.ChickenSoup, new Product() }
			};

			var mockProductRepository = new Mock<IProductRepository>();

			mockProductRepository.Setup(x => x.Get(It.IsAny<ProductType>())).Returns((ProductType x) => emptyProducts[x]);

			var mockProductService = new ProductsService(mockProductRepository.Object, mockCoinsService.Object);

			mockProductService.Invoking(x => x.Purchase(productType, coins))
				.Should().Throw<ArgumentException>()
				.WithMessage("Product is out of stock");

			mockCoinsService.Verify(x => x.UpdateCoinQuantities(It.IsAny<IEnumerable<Coin>>()), Times.Never);
			mockCoinsService.Verify(x => x.DistributeChange(It.IsAny<decimal>()), Times.Never);
		}

		public static readonly object[][] ValidPurchaseData =
		{
			new object[] { ProductType.Tea, new List<Coin> { new Coin(CoinType.OneEuro, 2) }, 0.70M },
			new object[] { ProductType.Espresso, new List<Coin> { new Coin(CoinType.OneEuro, 1), new Coin(CoinType.FiftyCent, 2) }, 0.20M },
			new object[] { ProductType.Juice, new List<Coin> { new Coin(CoinType.TwentyCent, 9) }, 0M },
			new object[] { ProductType.ChickenSoup, new List<Coin> { new Coin(CoinType.OneEuro, 2), new Coin(CoinType.FiftyCent, 4) }, 2.20M }
		};

		[Theory, MemberData(nameof(ValidPurchaseData))]
		public void Purchase_ReturnsCorrectChange_IfValidPurchase(ProductType productType, List<Coin> coins, decimal expected)
		{
			var productQuantity = productRepository.Get(productType).Quantity;

			productsService.Purchase(productType, coins)
				.Should().NotBeNull();

			mockCoinsService.Verify(x => x.UpdateCoinQuantities(coins), Times.Once);

			if (expected > 0)
				mockCoinsService.Verify(x => x.DistributeChange(expected), Times.Once);
			else
				mockCoinsService.Verify(x => x.DistributeChange(It.IsAny<decimal>()), Times.Never);

			productRepository.Get(productType).Quantity.Should().Be(--productQuantity);
		}
	}
}

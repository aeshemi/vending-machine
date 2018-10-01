using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using VendingMachine.Controllers;
using VendingMachine.Models;
using VendingMachine.Models.Enums;
using VendingMachine.Services;
using Xunit;

namespace VendingMachine.Tests.Controllers
{
	public class ProductsControllerTests
	{
		private readonly Mock<IProductsService> mockProductService;
		private readonly ProductsController productsController;
		private readonly PurchaseRequest request;

		public ProductsControllerTests()
		{
			mockProductService = new Mock<IProductsService>();
			mockProductService.Setup(x => x.GetProducts()).Returns(TestData.Products);

			productsController = new ProductsController(mockProductService.Object);

			request = new PurchaseRequest
			{
				ProductType = ProductType.Espresso,
				Coins = new List<Coin>
				{
					new Coin(CoinType.OneEuro, 1),
					new Coin(CoinType.FiftyCent, 2)
				}
			};
		}

		[Fact]
		public void List_ShouldReturnProductInventory()
		{
			var result = productsController.List().AssertOkObjectResult().Model<IEnumerable<Product>>();

			result.Should().NotBeEmpty()
				.And.HaveCount(4)
				.And.OnlyHaveUniqueItems(x => x.ProductType)
				.And.Equal(TestData.Products, (x, y) => 
					x.ProductType == y.ProductType && x.Description == y.Description && x.Price == y.Price && x.Quantity == y.Quantity);
		}

		[Fact]
		public void ValidPurchase_ReturnsMessageChangeAndUpdatedInventory()
		{
			var expectedChange = new List<Coin>
			{
				new Coin(CoinType.FiftyCent, 1, "50 cent"),
				new Coin(CoinType.TwentyCent, 1, "20 cent")
			};

			mockProductService.Setup(x => x.Purchase(It.IsAny<ProductType>(), It.IsAny<List<Coin>>())).Returns(expectedChange);

			var result = productsController.Purchase(request).AssertOkObjectResult().Model<PurchaseResponse>();

			result.Should().NotBeNull();
			result.Message.Should().Be("Thank you");

			result.Change.Should().NotBeEmpty()
				.And.HaveCount(2)
				.And.OnlyHaveUniqueItems(x => x.CoinType)
				.And.Equal(expectedChange, (x, y) => x.CoinType == y.CoinType && x.Description == y.Description && x.Quantity == y.Quantity);

			result.Products.Should().NotBeEmpty()
				.And.HaveCount(4)
				.And.OnlyHaveUniqueItems(x => x.ProductType)
				.And.Equal(TestData.Products, (x, y) =>
					x.ProductType == y.ProductType && x.Description == y.Description && x.Price == y.Price && x.Quantity == y.Quantity);
		}

		[Fact]
		public void InvalidPurchase_ProductOutOfStock_ReturnsBadRequest()
		{
			const string expected = "Product is out of stock";

			mockProductService.Setup(x => x.Purchase(It.IsAny<ProductType>(), It.IsAny<List<Coin>>()))
				.Throws(new ArgumentException(expected));

			var result = productsController.Purchase(request).AssertBadRequestObjectResult();

			result.Message.Should().Be(expected);
			result.Key.Should().BeNull();
		}

		[Fact]
		public void InvalidPurchase_InsufficientCoins_ReturnsBadRequest()
		{
			const string expected = "Insufficient amount";

			mockProductService.Setup(x => x.Purchase(It.IsAny<ProductType>(), It.IsAny<List<Coin>>()))
				.Throws(new ArgumentException(expected));

			var result = productsController.Purchase(request).AssertBadRequestObjectResult();

			result.Message.Should().Be(expected);
			result.Key.Should().BeNull();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VendingMachine.Models;
using VendingMachine.Services;

namespace VendingMachine.Controllers
{
	[Produces("application/json")]
	[Route("api/products")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductsService productsService;

		public ProductsController(IProductsService productsService)
		{
			this.productsService = productsService;
		}

		/// <summary>
		/// Retrieves product information for the vending machine's inventory
		/// </summary>
		/// <response code="200">Returns the product inventory for the vending machine</response>
		// GET api/products
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<Product>), 200)]
		public IActionResult List()
		{
			return new OkObjectResult(productsService.GetProducts());
		}

		/// <summary>
		/// Endpoint to purchase a product
		/// </summary>
		/// <param name="request">PurchaseRequest object containing the product type and coins inserted</param>
		/// <response code="200">Returns PurchaseResponse containing a message, change if any and the updated product inventory for the vending machine</response>
		/// <response code="400">If product to be purchased is already out of stock</response>
		/// <response code="400">If amount inserted is insufficient to purchase the product</response>
		// POST api/products/purchase
		[ProducesResponseType(typeof(PurchaseResponse), 200)]
		[ProducesResponseType(400)]
		[HttpPost("purchase")]
		public IActionResult Purchase([FromBody] PurchaseRequest request)
		{
			try
			{
				var result = productsService.Purchase(request.ProductType, request.Coins.ToList());
				return new OkObjectResult(new PurchaseResponse(result, productsService.GetProducts()));
			}
			catch (ArgumentException ex)
			{
				return new BadRequestObjectResult(new ValidationError(ex.Message));
			}
		}
	}
}

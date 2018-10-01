using Microsoft.AspNetCore.Mvc;
using VendingMachine.Services;

namespace VendingMachine.Controllers
{
	[Produces("application/json")]
	[Route("api/coins")]
	[ApiController]
	public class CoinsController : ControllerBase
	{
		private readonly ICoinsService coinsService;

		public CoinsController(ICoinsService coinsService)
		{
			this.coinsService = coinsService;
		}

		// GET api/coins
		[HttpGet]
		public IActionResult List()
		{
			return new OkObjectResult(coinsService.GetCoins());
		}
	}
}

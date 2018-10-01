using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using VendingMachine.Models;

namespace VendingMachine.Tests
{
	public static class ActionResultExtensions
	{
		public static ObjectResult AssertStatus(this ObjectResult result, HttpStatusCode code)
		{
			result.StatusCode.Should().Be((int) code);
			return result;
		}

		public static ObjectResult AssertOkObjectResult(this IActionResult result)
		{
			result.Should().NotBeNull();

			var okObjectResult = result as OkObjectResult;

			okObjectResult.Should().NotBeNull();
			okObjectResult.AssertStatus(HttpStatusCode.OK);

			return okObjectResult;
		}

		public static ValidationError AssertBadRequestObjectResult(this IActionResult result)
		{
			result.Should().NotBeNull();

			var okObjectResult = result as BadRequestObjectResult;

			okObjectResult.Should().NotBeNull();
			okObjectResult.AssertStatus(HttpStatusCode.BadRequest);

			return okObjectResult.Model<ValidationError>();
		}

		public static T Model<T>(this ObjectResult result)
		{
			return (T)result.Value;
		}
	}
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TRMApi.Library.DataAccess;

using TRMCommon.Library.Models;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Cashier")]
	public class ProductController(ILogger<ProductController> logger, IProductData productData) : ControllerBase
	{
		private readonly ILogger<ProductController> _logger = logger;
		private readonly IProductData _productData = productData;

		[HttpGet]
		public List<ProductModel> Get()
		{
			_logger.LogInformation("GET Product API Controller");
			List<ProductModel> products = _productData.GetProducts();
			_logger.LogDebug("GetProducts Returned {ProductCount} Products", products.Count);
			return products;
		}
	}
}
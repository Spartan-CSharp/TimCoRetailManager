using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TRMApi.Library.DataAccess;
using TRMApi.Library.Models;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SaleController(ILogger<SaleController> logger, ISaleData saleData) : ControllerBase
	{
		private readonly ILogger<SaleController> _logger = logger;
		private readonly ISaleData _saleData = saleData;

		[Authorize(Roles = "Cashier")]
		[HttpPost]
		public void Post(SaleModel sale)
		{
			_logger.LogInformation("POST Sale API Controller, Sale Model with {SaleDetailCount} Sale Detail Lines.", sale.SaleDetails.Count);
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			_logger.LogDebug("Cashier UserId is {UserId}", userId);
			_saleData.SaveSale(sale, userId);
		}

		[Authorize(Roles = "Admin,Manager")]
		[Route("GetSalesReport")]
		[HttpGet]
		public List<SaleReportModel> GetSalesReport()
		{
			_logger.LogInformation("GET Sale API Controller, GetSalesReport Route");
			List<SaleReportModel> output = _saleData.GetSaleReport();
			_logger.LogDebug("GetSalesReport Returned {NumberOfReports} Sale Report Models", output.Count);
			return output;
		}

		[AllowAnonymous]
		[Route("GetTaxRate")]
		[HttpGet]
		public decimal GetTaxRate()
		{
			_logger.LogInformation("GET Sale API Controller, GetTaxRate Route");
			decimal output = _saleData.GetTaxRate();
			_logger.LogDebug("GetTaxRate Returned Tax Rate of {TaxRate}", output);
			return output;
		}
	}
}
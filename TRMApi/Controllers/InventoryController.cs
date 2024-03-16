using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TRMApi.Library.DataAccess;

using TRMCommon.Library.Models;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class InventoryController(ILogger<InventoryController> logger, IInventoryData inventoryData) : ControllerBase
	{
		private readonly ILogger<InventoryController> _logger = logger;
		private readonly IInventoryData _inventoryData = inventoryData;

		[Authorize(Roles = "Manager,Admin")]
		[HttpGet]
		public List<InventoryModel> Get()
		{
			_logger.LogInformation("GET Inventory API Controller");
			List<InventoryModel> output = _inventoryData.GetInventory();
			_logger.LogDebug("GetInventory Returned {InventoryCount} Inventory Transactions", output.Count);
			return output;
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public void Post(InventoryModel item)
		{
			_logger.LogInformation("POST Inventory API Controller, Inventory Model for ProductId = {ProductId}, Purchased {Quantity} units at {UnitPrice} on {PurchaseDate}.", item.ProductId, item.Quantity, item.PurchasePrice, item.PurchaseDate);
			_inventoryData.SaveInventoryRecord(item);
		}
	}
}
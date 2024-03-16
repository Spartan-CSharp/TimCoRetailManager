using TRMApi.Library.Models;

namespace TRMApi.Library.DataAccess
{
	public interface IInventoryData
	{
		List<InventoryModel> GetInventory();
		void SaveInventoryRecord(InventoryModel item);
	}
}
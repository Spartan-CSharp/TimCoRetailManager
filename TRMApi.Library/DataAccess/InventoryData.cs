using TRMApi.Library.Models;

namespace TRMApi.Library.DataAccess
{
	public class InventoryData(ISqlDataAccess sql) : IInventoryData
	{
		private readonly ISqlDataAccess _sql = sql;

		public List<InventoryModel> GetInventory()
		{
			List<InventoryModel> output = _sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");

			return output;
		}

		public void SaveInventoryRecord(InventoryModel item)
		{
			_sql.SaveData("dbo.spInventory_Insert", item, "TRMData");
		}
	}
}

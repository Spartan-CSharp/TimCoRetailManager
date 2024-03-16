using TRMApi.Library.Models;

using TRMCommon.Library.Models;

namespace TRMApi.Library.DataAccess
{
	public interface ISaleData
	{
		List<SaleReportModel> GetSaleReport();
		decimal GetTaxRate();
		void SaveSale(SaleModel saleInfo, string cashierId);
	}
}
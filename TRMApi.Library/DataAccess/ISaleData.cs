using TRMApi.Library.Models;

namespace TRMApi.Library.DataAccess
{
	public interface ISaleData
	{
		List<SaleReportModel> GetSaleReport();
		decimal GetTaxRate();
		void SaveSale(SaleModel saleInfo, string cashierId);
	}
}
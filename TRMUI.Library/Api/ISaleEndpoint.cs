using TRMCommon.Library.Models;

namespace TRMUI.Library.Api
{
	public interface ISaleEndpoint
	{
		Task PostSale(SaleModel sale);
	}
}
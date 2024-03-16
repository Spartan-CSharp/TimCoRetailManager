using TRMCommon.Library.Models;

namespace TRMUI.Library.Api
{
	public interface IProductEndpoint
	{
		Task<List<ProductModel>> GetAll();
	}
}
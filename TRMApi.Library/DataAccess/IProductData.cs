using TRMApi.Library.Models;

namespace TRMApi.Library.DataAccess
{
	public interface IProductData
	{
		ProductModel GetProductById(int productId);
		List<ProductModel> GetProducts();
	}
}
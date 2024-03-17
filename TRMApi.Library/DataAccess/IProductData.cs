using TRMCommon.Library.Models;

namespace TRMApi.Library.DataAccess
{
	public interface IProductData
	{
		void CreateProduct(ProductModel product);
		ProductModel GetProductById(int productId);
		List<ProductModel> GetProducts();
	}
}
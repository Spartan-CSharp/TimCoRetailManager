using TRMApi.Library.Models;

namespace TRMApi.Library.DataAccess
{
	public class ProductData(ISqlDataAccess sql) : IProductData
	{
		private readonly ISqlDataAccess _sql = sql;

		public List<ProductModel> GetProducts()
		{
			List<ProductModel> output = _sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "TRMData");

			return output;
		}

		public ProductModel GetProductById(int productId)
		{
			ProductModel? output = _sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

			return output is null ? throw new ApplicationException($"Product with ID {productId} Not Found") : output;
		}
	}
}

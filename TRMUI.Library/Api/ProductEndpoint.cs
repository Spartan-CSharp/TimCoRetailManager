using TRMCommon.Library.Models;

namespace TRMUI.Library.Api
{
	public class ProductEndpoint(IAPIHelper apiHelper) : IProductEndpoint
	{
		private readonly IAPIHelper _apiHelper = apiHelper;

		public async Task<List<ProductModel>> GetAll()
		{
			using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/Product");
			if ( response.IsSuccessStatusCode )
			{
				List<ProductModel> result = await response.Content.ReadAsAsync<List<ProductModel>>();
				return result;
			}
			else
			{
				throw new Exception(response.ReasonPhrase);
			}
		}
	}
}

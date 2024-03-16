using TRMCommon.Library.Models;

namespace TRMUI.Library.Api
{
	public class SaleEndpoint(IAPIHelper apiHelper) : ISaleEndpoint
	{
		private readonly IAPIHelper _apiHelper = apiHelper;

		public async Task PostSale(SaleModel sale)
		{
			using ( HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Sale", sale) )
			{
				if ( response.IsSuccessStatusCode )
				{
					// Log successful call?
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}
	}
}

using System.Net.Http.Headers;

using Microsoft.Extensions.Configuration;

using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
	public class APIHelper : IAPIHelper
	{
		private readonly ILoggedInUserModel _loggedInUser;
		private readonly IConfiguration _config;

		public APIHelper(ILoggedInUserModel loggedInUser, IConfiguration config)
		{
			_loggedInUser = loggedInUser;
			_config = config;
			InitializeClient();
		}

		public HttpClient ApiClient { get; private set; } = new HttpClient();

		private void InitializeClient()
		{
			string api = _config.GetValue<string>("api") ?? throw new InvalidOperationException("AppSetting 'api' not found.");

			ApiClient = new()
			{
				BaseAddress = new Uri(api)
			};
			ApiClient.DefaultRequestHeaders.Accept.Clear();
			ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<AuthenticatedUser> Authenticate(string username, string password)
		{
			FormUrlEncodedContent data = new FormUrlEncodedContent(
			[
				new KeyValuePair<string, string>("grant_type", "password"),
				new KeyValuePair<string, string>("username", username),
				new KeyValuePair<string, string>("password", password)
			]);

			using HttpResponseMessage response = await ApiClient.PostAsync("/Token", data);
			if ( response.IsSuccessStatusCode )
			{
				AuthenticatedUser result = await response.Content.ReadAsAsync<AuthenticatedUser>();
				return result;
			}
			else
			{
				throw new Exception(response.ReasonPhrase);
			}
		}

		public void LogOffUser()
		{
			ApiClient.DefaultRequestHeaders.Clear();
		}

		public async Task GetLoggedInUserInfo(string token)
		{
			ApiClient.DefaultRequestHeaders.Clear();
			ApiClient.DefaultRequestHeaders.Accept.Clear();
			ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			ApiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

			using HttpResponseMessage response = await ApiClient.GetAsync("/api/User");
			if ( response.IsSuccessStatusCode )
			{
				LoggedInUserModel result = await response.Content.ReadAsAsync<LoggedInUserModel>();
				_loggedInUser.CreatedDate = result.CreatedDate;
				_loggedInUser.EmailAddress = result.EmailAddress;
				_loggedInUser.FirstName = result.FirstName;
				_loggedInUser.Id = result.Id;
				_loggedInUser.LastName = result.LastName;
				_loggedInUser.Token = token;
			}
			else
			{
				throw new Exception(response.ReasonPhrase);
			}
		}
	}
}

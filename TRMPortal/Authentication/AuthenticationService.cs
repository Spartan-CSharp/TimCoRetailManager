using System.Net.Http.Headers;
using System.Text.Json;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using TRMCommon.Library.Authentication;

using TRMPortal.Models;

namespace TRMPortal.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly HttpClient _client;
		private readonly AuthenticationStateProvider _authStateProvider;
		private readonly ILocalStorageService _localStorage;
		private readonly IConfiguration _config;
		private readonly string _authTokenStorageKey;
		private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

		public AuthenticationService(HttpClient client,
									 AuthenticationStateProvider authStateProvider,
									 ILocalStorageService localStorage,
									 IConfiguration config)
		{
			_client = client;
			_authStateProvider = authStateProvider;
			_localStorage = localStorage;
			_config = config;
			_authTokenStorageKey = _config["authTokenStorageKey"] ?? throw new InvalidOperationException("AppSetting 'authTokenStorageKey' not found.");
		}

		public async Task<AuthenticatedUser> Login(AuthenticationUserModel userForAuthentication)
		{
			FormUrlEncodedContent data = new FormUrlEncodedContent(
			[
				new KeyValuePair<string, string>("grant_type", "password"),
				new KeyValuePair<string, string>("username", userForAuthentication.Email),
				new KeyValuePair<string, string>("password", userForAuthentication.Password)
			]);

			string api = _config["api"] + _config["tokenEndpoint"];
			HttpResponseMessage authResult = await _client.PostAsync(api, data);

			if ( authResult.IsSuccessStatusCode == false )
			{
				throw new Exception(authResult.ReasonPhrase);
			}

			string authContent = await authResult.Content.ReadAsStringAsync();
			AuthenticatedUser result = JsonSerializer.Deserialize<AuthenticatedUser>(
				authContent,
				_options) ?? throw new Exception("Failed to authenticate user.");

			await _localStorage.SetItemAsync(_authTokenStorageKey, result.Access_Token);

			_ = await ((AuthStateProvider)_authStateProvider).NotifyUserAuthenticationAsync(result.Access_Token);

			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Access_Token);

			return result;
		}

		public async Task Logout()
		{
			await ((AuthStateProvider)_authStateProvider).NotifyUserLogoutAsync();
		}
	}
}

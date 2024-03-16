using System.Net.Http.Headers;
using System.Security.Claims;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using TRMDesktopUI.Library.Api;

namespace TRMPortal.Authentication
{
	public class AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration config, IAPIHelper apiHelper) : AuthenticationStateProvider
	{
		private readonly HttpClient _httpClient = httpClient;
		private readonly ILocalStorageService _localStorage = localStorage;
		private readonly IConfiguration _config = config;
		private readonly IAPIHelper _apiHelper = apiHelper;
		private readonly AuthenticationState _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			string authTokenStorageKey = _config["authTokenStorageKey"] ?? throw new InvalidOperationException("AppSetting 'authTokenStorageKey' not found.");
			string? token = await _localStorage.GetItemAsync<string>(authTokenStorageKey);

			if ( string.IsNullOrWhiteSpace(token) )
			{
				return _anonymous;
			}

			bool isAuthetcated = await NotifyUserAuthenticationAsync(token);

			if ( isAuthetcated == false )
			{
				return _anonymous;
			}

			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

			return new AuthenticationState(
				new ClaimsPrincipal(
					new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token),
					"jwtAuthType")));
		}

		public async Task<bool> NotifyUserAuthenticationAsync(string token)
		{
			bool isAuthenticatedOutput;
			Task<AuthenticationState> authState;

			try
			{
				await _apiHelper.GetLoggedInUserInfo(token);
			}
			catch ( Exception ex )
			{
				Console.WriteLine(ex.Message);
			}

			try
			{
				ClaimsPrincipal authenticatedUser = new ClaimsPrincipal(
					new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token),
					"jwtAuthType"));
				authState = Task.FromResult(new AuthenticationState(authenticatedUser));
				NotifyAuthenticationStateChanged(authState);
				isAuthenticatedOutput = true;
			}
			catch ( Exception ex )
			{
				Console.WriteLine(ex);
				await NotifyUserLogoutAsync();
				isAuthenticatedOutput = false;
			}

			return isAuthenticatedOutput;
		}

		public async Task NotifyUserLogoutAsync()
		{
			string authTokenStorageKey = _config["authTokenStorageKey"] ?? throw new InvalidOperationException("AppSetting 'authTokenStorageKey' not found.");
			await _localStorage.RemoveItemAsync(authTokenStorageKey);
			Task<AuthenticationState> authState = Task.FromResult(_anonymous);
			_apiHelper.LogOffUser();
			_httpClient.DefaultRequestHeaders.Authorization = null;
			NotifyAuthenticationStateChanged(authState);
		}
	}
}

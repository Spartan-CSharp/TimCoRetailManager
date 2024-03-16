using System.Security.Claims;
using System.Text.Json;

namespace Portal.Authentication
{
	public static class JwtParser
	{
		public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			List<Claim> claims = [];
			string payload = jwt.Split('.')[1];

			byte[] jsonBytes = ParseBase64WithoutPadding(payload);

			Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? [];

			ExtractRolesFromJWT(claims, keyValuePairs);

			claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));

			return claims;
		}

		private static void ExtractRolesFromJWT(List<Claim> claims, Dictionary<string, object> keyValuePairs)
		{
			_ = keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);

			if ( roles is not null )
			{
				string rolesString = roles.ToString() ?? string.Empty;
				string[] parsedRoles = rolesString.Trim().TrimStart('[').TrimEnd(']').Split(',');

				if ( parsedRoles.Length > 1 )
				{
					foreach ( string parsedRole in parsedRoles )
					{
						claims.Add(new Claim(ClaimTypes.Role, parsedRole.Trim('"')));
					}
				}
				else
				{
					claims.Add(new Claim(ClaimTypes.Role, parsedRoles[0]));
				}

				_ = keyValuePairs.Remove(ClaimTypes.Role);
			}
		}

		private static byte[] ParseBase64WithoutPadding(string base64)
		{
			switch ( base64.Length % 4 )
			{
				case 2:
					base64 += "==";
					break;
				case 3:
					base64 += "=";
					break;
			}

			return Convert.FromBase64String(base64);
		}
	}
}

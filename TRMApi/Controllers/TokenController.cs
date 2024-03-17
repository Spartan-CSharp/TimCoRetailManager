using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using TRMApi.Data;

using TRMCommon.Library.Authentication;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TokenController(ILogger<TokenController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration config) : ControllerBase
	{
		private readonly ILogger<TokenController> _logger = logger;
		private readonly ApplicationDbContext _context = context;
		private readonly UserManager<IdentityUser> _userManager = userManager;
		private readonly IConfiguration _config = config;

		// POST api/Token
		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> CreateAsync([FromForm] string username, [FromForm] string password, [FromForm] string grant_type)
		{
			_logger.LogInformation("POST Token API Controller with Grant Type = {GrantType}, User Name = {UserName}, and Password Is Null or Whitespace = {PasswordEmpty}", grant_type, username, string.IsNullOrWhiteSpace(password));
			bool isValidLogin = await IsValidUsernameAndPasswordAsync(username, password);
			_logger.LogDebug("IsValidUserameAndPasswordAsync Returned {IsValidLogin}", isValidLogin);

			if ( isValidLogin )
			{
				AuthenticatedUser authUser = await GenerateTokenAsync(username);
				_logger.LogDebug("GenerateTokenAsync Returned the following authUser for User {UserName}: {AccessToken}", authUser.UserName, authUser.Access_Token);
				ObjectResult output = new ObjectResult(authUser);
				_logger.LogDebug("POST Token API Controller Returning ObjectResult");
				return output;
			}
			else
			{
				var loginError = new
				{
					Message = "Invalid Login Attempt"
				};
				UnauthorizedObjectResult output = Unauthorized(loginError);
				_logger.LogDebug("POST Token API Controller Returning Unauthorized with Message = {Message}", loginError.Message);
				return output;
			}
		}

		private async Task<bool> IsValidUsernameAndPasswordAsync(string username, string password)
		{
			_logger.LogTrace("IsValidUsernameAndPasswordAsync called with User Name = {UserName}, and Password Is Null or Whitespace = {PasswordEmpty}", username, string.IsNullOrWhiteSpace(password));
			IdentityUser? user = await _userManager.FindByEmailAsync(username);

			if ( user is null )
			{
				_logger.LogTrace("User Name {UserName} Not Found: Returning false", username);
				return false;
			}

			bool isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
			_logger.LogTrace("Password for User Name {UserName} Is Valid: {PasswordValid}", username, isPasswordValid);

			return isPasswordValid;
		}

		private async Task<AuthenticatedUser> GenerateTokenAsync(string username)
		{
			_logger.LogTrace("GenerateTokenAsync called with User Name = {UserName}", username);

			AuthenticatedUser output = new AuthenticatedUser()
			{
				UserName = username
			};

			IdentityUser? user = await _userManager.FindByEmailAsync(username);

			if ( user is not null )
			{
				var roles = from ur in _context.UserRoles
							join r in _context.Roles on ur.RoleId equals r.Id
							where ur.UserId == user.Id
							select new
							{
								ur.UserId,
								ur.RoleId,
								r.Name
							};

				output.Issued = DateTime.Now;
				output.Expires = DateTime.Now.AddDays(1);
				output.ExpiresIn = (int)output.Expires.Subtract(output.Issued).TotalSeconds;

				List<Claim> claims =
					[
						new Claim(ClaimTypes.Name, username),
						new Claim(ClaimTypes.NameIdentifier, user.Id),
						new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(output.Issued).ToUnixTimeSeconds().ToString()),
						new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(output.Expires).ToUnixTimeSeconds().ToString())
					];

				foreach ( var role in roles )
				{
					claims.Add(new Claim(ClaimTypes.Role, role.Name));
				}

				string key = _config.GetValue<string>("Secrets:SecurityKey") ?? throw new InvalidOperationException("AppSetting 'Secrets:SecurityKey' not found.");

				JwtSecurityToken token = new JwtSecurityToken(
					new JwtHeader(
						new SigningCredentials(
							new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
							SecurityAlgorithms.HmacSha256)),
					new JwtPayload(claims));

				output.Access_Token = new JwtSecurityTokenHandler().WriteToken(token);
				_logger.LogTrace("Token Generated for User {UserName} valid from {Issued} to {Expires}", username, output.Issued, output.Expires);
			}
			else
			{
				_logger.LogTrace("User Name {UserName} Not Found: Returning empty access token", username);
			}

			return output;
		}
	}
}
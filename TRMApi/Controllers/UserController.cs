using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using TRMApi.Data;
using TRMApi.Models;

using TRMApi.Library.DataAccess;
using TRMApi.Library.Models;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IUserData userData, ILogger<UserController> logger) : ControllerBase
	{
		private readonly ApplicationDbContext _context = context;
		private readonly UserManager<IdentityUser> _userManager = userManager;
		private readonly IUserData _userData = userData;
		private readonly ILogger<UserController> _logger = logger;

		public record UserRegistrationModel(
			string FirstName,
			string LastName,
			string EmailAddress,
			string Password);

		[HttpGet]
		public UserModel GetById()
		{
			_logger.LogInformation("GET User API Controller");
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			_logger.LogDebug("User's UserId is {UserId}", userId);
			UserModel output = _userData.GetUserById(userId).First();
			_logger.LogDebug("User {UserId}, {FirstName} {LastName}, has Email {EmailAddress} and was created {CeatedDate}", output.Id, output.FirstName, output.LastName, output.EmailAddress, output.CreatedDate);
			return output;
		}

		[HttpPost]
		[Route("Register")]
		[AllowAnonymous]
		public async Task<IActionResult> RegisterAsync(UserRegistrationModel user)
		{
			BadRequestObjectResult output;
			_logger.LogInformation("POST User API Controller, Register Route, UserRegistrationModel for {FirstName} {LastName} with Email {EmailAddress}, Model State is Valid: {ModelValid}", user.FirstName, user.LastName, user.EmailAddress, ModelState.IsValid);
			if ( ModelState.IsValid )
			{
				IdentityUser? existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);
				if ( existingUser is null )
				{
					IdentityUser newUser = new()
					{
						Email = user.EmailAddress,
						EmailConfirmed = true,
						UserName = user.EmailAddress
					};

					IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
					_logger.LogDebug("CreateAsync Returned {Succeeded} for User {EmailAddress}", result.Succeeded, user.EmailAddress);

					if ( result.Succeeded )
					{
						existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);

						if ( existingUser is null )
						{
							_logger.LogDebug("User Not Created Correctly");
							var userNotCreated = new
							{
								Message = $"User Not Created Correctly",
							};
							output = BadRequest(userNotCreated);
							_logger.LogDebug("GET Token API Controller Returning Bad Request with Message = {Message}", userNotCreated.Message);
							return output;
						}

						UserModel u = new()
						{
							Id = existingUser.Id,
							FirstName = user.FirstName,
							LastName = user.LastName,
							EmailAddress = user.EmailAddress
						};
						_userData.CreateUser(u);
						_logger.LogDebug("User {EmailAddress} created with Id = {Id}", u.EmailAddress, u.Id);
						return Created();
					}
				}

				_logger.LogDebug("User with Email {EmailAddress} already exists.", user.EmailAddress);
				var userExists = new
				{
					Message = $"User {user.EmailAddress} Already Exists",
				};
				output = BadRequest(userExists);
				_logger.LogDebug("GET Token API Controller Returning Bad Request with Message = {Message}", userExists.Message);
				return output;
			}

			_logger.LogDebug("Model State Not Valid");
			var modelStateNotValid = new
			{
				Message = $"Model State Not Valid",
			};
			output = BadRequest(modelStateNotValid);
			_logger.LogDebug("GET Token API Controller Returning Bad Request with Message = {Message}", modelStateNotValid.Message);
			return output;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		[Route("Admin/GetAllUsers")]
		public List<ApplicationUserModel> GetAllUsers()
		{
			_logger.LogInformation("GET User API Controller, Admin/GetAllUsers Route");

			List<ApplicationUserModel> output = [];

			List<IdentityUser> users = [.. _context.Users];
			var userRoles = from ur in _context.UserRoles
							join r in _context.Roles on ur.RoleId equals r.Id
							select new
							{
								ur.UserId,
								ur.RoleId,
								r.Name
							};

			foreach ( IdentityUser user in users )
			{
				ApplicationUserModel u = new()
				{
					Id = user.Id,
					Email = user.Email ?? user.UserName ?? string.Empty
				};

				u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(key => key.RoleId, val => val.Name);

				output.Add(u);
			}

			return output;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		[Route("Admin/GetAllRoles")]
		public Dictionary<string, string> GetAllRoles()
		{
			_logger.LogInformation("GET User API Controller, Admin/GetAllRolesRoute");
			Dictionary<string, string> roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name ?? string.Empty);
			_logger.LogDebug("Returning Dictionary of {RoleCount} Roles", roles.Count);
			return roles;
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("Admin/AddRole")]
		public async Task AddARoleAsync(UserRolePairModel pairing)
		{
			_logger.LogInformation("GET User API Controller, Admin/AddARole Route with UserRolePaidModel UserId = {User} and RoleName = {Role}", pairing.UserId, pairing.RoleName);
			string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			_logger.LogDebug("Logged In User's UserId is {UserId}", loggedInUserId);

			IdentityUser? user = await _userManager.FindByIdAsync(pairing.UserId);

			if ( user is not null )
			{
				_ = await _userManager.AddToRoleAsync(user, pairing.RoleName);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("Admin/RemoveRole")]
		public async Task RemoveARoleAsync(UserRolePairModel pairing)
		{
			_logger.LogInformation("GET User API Controller, Admin/RemoveRole Route with UserRolePaidModel UserId = {User} and RoleName = {Role}", pairing.UserId, pairing.RoleName);
			string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
			_logger.LogDebug("Logged In User's UserId is {UserId}", loggedInUserId);

			IdentityUser? user = await _userManager.FindByIdAsync(pairing.UserId);

			if ( user is not null )
			{
				_ = await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
			}
		}
	}
}
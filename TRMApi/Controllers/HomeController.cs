using System.Diagnostics;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using TRMApi.Models;

namespace TRMApi.Controllers
{
	public class HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) : Controller
	{
		private readonly ILogger<HomeController> _logger = logger;
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;
		private readonly UserManager<IdentityUser> _userManager = userManager;

		public IActionResult Index()
		{
			_logger.LogInformation("GET Home Controller, Index View, No Model");
			return View();
		}

		public async Task<IActionResult> PrivacyAsync()
		{
			_logger.LogInformation("GET Home Controller, PrivacyAsync View, No Model");
			_logger.LogDebug("Starting Administrator Initialization");
			string[] roles = ["Admin", "Manager", "Cashier"];

			foreach ( string role in roles )
			{
				bool roleExist = await _roleManager.RoleExistsAsync(role);

				if ( roleExist == false )
				{
					_logger.LogTrace("Role {Role} does not already exist, creating", role);
					_ = await _roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			IdentityUser? user = await _userManager.FindByEmailAsync("tim@iamtimcorey.com");

			if ( user != null )
			{
				_logger.LogTrace("User {UserName} found, adding Admin & Cashier Roles", user.UserName);
				_ = await _userManager.AddToRoleAsync(user, "Admin");
				_ = await _userManager.AddToRoleAsync(user, "Cashier");
			}

			_logger.LogDebug("Finished Administrator Initialization");
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			string? requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
			_logger.LogError("GET Home Controller, Error View, Error View Model with RequestId = {RequestId}", requestId);
			return View(new ErrorViewModel { RequestId = requestId });
		}
	}
}

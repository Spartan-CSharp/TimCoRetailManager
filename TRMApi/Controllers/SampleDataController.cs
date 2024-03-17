using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using TRMApi.Library.DataAccess;

using TRMCommon.Library.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SampleDataController(ILogger<SampleDataController> logger, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IUserData userData, IProductData productData, IInventoryData inventoryData) : ControllerBase
	{
		private readonly ILogger<SampleDataController> _logger = logger;
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;
		private readonly UserManager<IdentityUser> _userManager = userManager;
		private readonly IUserData _userData = userData;
		private readonly IProductData _productData = productData;
		private readonly IInventoryData _inventoryData = inventoryData;

		// POST api/SampleData
		[AllowAnonymous]
		[HttpPost]
		public async Task PostAsync()
		{
			_logger.LogInformation("POST Sample Data API Controller");
			await CreateSecurityRolesAsync();
			await CreateSampleUsersAsync();
			CreateSampleProducts();
			CreateSampleInventory();
		}

		private async Task CreateSecurityRolesAsync()
		{
			string[] roles = ["Admin", "Manager", "Cashier"];

			foreach ( string role in roles )
			{
				bool roleExist = await _roleManager.RoleExistsAsync(role);

				if ( roleExist == false )
				{
					_ = await _roleManager.CreateAsync(new IdentityRole(role));
				}
			}
		}

		private async Task CreateSampleUsersAsync()
		{
			List<UserModel> sampleUsers =
			[
				new UserModel
				{
					FirstName = "Tim",
					LastName = "Corey",
					EmailAddress = "tim@iamtimcorey.com",
					CreatedDate = DateTime.Now
				},
				new UserModel
				{
					FirstName = "Pierre",
					LastName = "Plourde",
					EmailAddress = "pierre@spartancsharp.net",
					CreatedDate = DateTime.Now
				},
				new UserModel
				{
					FirstName = "Sue",
					LastName = "Storm",
					EmailAddress = "sue@stormy.com",
					CreatedDate = DateTime.Now
				}
			];

			foreach ( UserModel sampleUser in sampleUsers )
			{
				IdentityUser? existingUser = await _userManager.FindByEmailAsync(sampleUser.EmailAddress);
				if ( existingUser is null )
				{
					IdentityUser newUser = new()
					{
						Email = sampleUser.EmailAddress,
						EmailConfirmed = true,
						UserName = sampleUser.EmailAddress
					};

					IdentityResult result = await _userManager.CreateAsync(newUser, "Pwd12345.");
					if ( result.Succeeded )
					{
						IdentityUser? createdUser = await _userManager.FindByEmailAsync(sampleUser.EmailAddress);
						if ( createdUser is not null )
						{
							await ProcessUserAsync(sampleUser, createdUser);
						}
					}
				}
				else
				{
					await ProcessUserAsync(sampleUser, existingUser);
				}
			}
		}

		private async Task ProcessUserAsync(UserModel userModel, IdentityUser identityUser)
		{
			userModel.Id = identityUser.Id;
			await AddRolesToSampleUserAsync(identityUser);
			if ( _userData.GetUserById(userModel.Id).FirstOrDefault() is null )
			{
				_userData.CreateUser(userModel);
			}
		}

		private async Task AddRolesToSampleUserAsync(IdentityUser user)
		{
			_ = await _userManager.AddToRoleAsync(user, "Admin");
			_ = await _userManager.AddToRoleAsync(user, "Cashier");
		}

		private void CreateSampleProducts()
		{
			List<ProductModel> sampleProducts = new List<ProductModel>()
			{
				new ProductModel
				{
					ProductName = "Fluffy Bath Towels",
					Description = "Large fluffy bath towel set (2 toweels and 2 washcloths)",
					RetailPrice = 29.9500M,
					QuantityInStock = 20,
					IsTaxable = true,
					ProductImage = null,
					CreatedDate = DateTime.UtcNow,
					LastModified = DateTime.UtcNow
				},
				new ProductModel
				{
					ProductName = "10\" Skillet",
					Description = "A non-stick skillet made with stainless steel.",
					RetailPrice = 18.7500M,
					QuantityInStock = 10,
					IsTaxable = true,
					ProductImage = null,
					CreatedDate = DateTime.UtcNow,
					LastModified = DateTime.UtcNow
				},
				new ProductModel
				{
					ProductName = "Large Toaster Oven",
					Description = "A temperature-adjustable toaster oven with dual racks and interior light.",
					RetailPrice = 49.9900M,
					QuantityInStock = 5,
					IsTaxable = false,
					ProductImage = null,
					CreatedDate = DateTime.UtcNow,
					LastModified = DateTime.UtcNow
				},
				new ProductModel
				{
					ProductName = "Home Repair Kit",
					Description = "Features four screwdrivers, a hammer, a tape measure, a pair of pliers, and a level.",
					RetailPrice = 25.0000M,
					QuantityInStock = 50,
					IsTaxable = true,
					ProductImage = null,
					CreatedDate = DateTime.UtcNow,
					LastModified = DateTime.UtcNow
				},
				new ProductModel
				{
					ProductName = "Finish by Jon Acuff",
					Description = "The book to read",
					RetailPrice = 13.6000M,
					QuantityInStock = 10,
					IsTaxable = true,
					ProductImage = "/images/finish.jpg",
					CreatedDate = DateTime.UtcNow,
					LastModified = DateTime.UtcNow
				},
				new ProductModel
				{
					ProductName = "8 AA Eneloop Batteries",
					Description = "The best rechargeable batteries out there",
					RetailPrice = 18.0000M,
					QuantityInStock = 5,
					IsTaxable = true,
					ProductImage = "/images/eneloop.jpg",
					CreatedDate = DateTime.UtcNow,
					LastModified = DateTime.UtcNow
				}
			};

			List<ProductModel> existingProducts = _productData.GetProducts();

			foreach ( ProductModel sampleProduct in sampleProducts )
			{
				if ( existingProducts.Where(x => x.ProductName == sampleProduct.ProductName).FirstOrDefault() is null )
				{
					_productData.CreateProduct(sampleProduct);
				}
			}
		}

		private void CreateSampleInventory()
		{
			List<ProductModel> existingProducts = _productData.GetProducts();

			foreach ( ProductModel product in existingProducts )
			{
				var inventory = new InventoryModel
				{
					ProductId = product.Id,
					PurchaseDate = DateTime.UtcNow
				};

				switch ( product.ProductName )
				{
					case "Fluffy Bath Towels":
						inventory.Quantity = 20;
						inventory.PurchasePrice = 15.0000M;
						break;
					case "10\" Skillet":
						inventory.Quantity = 10;
						inventory.PurchasePrice = 9.5000M;
						break;
					case "Large Toaster Oven":
						inventory.Quantity = 5;
						inventory.PurchasePrice = 25.0000M;
						break;
					case "Home Repair Kit":
						inventory.Quantity = 50;
						inventory.PurchasePrice = 12.5000M;
						break;
					case "Finish by Jon Acuff":
						inventory.Quantity = 10;
						inventory.PurchasePrice = 9.7500M;
						break;
					case "8 AA Eneloop Batteries":
						inventory.Quantity = 5;
						inventory.PurchasePrice = 12.0000M;
						break;
					default:
						inventory.Quantity = 0;
						inventory.PurchasePrice = 0.0000M;
						break;
				}

				_inventoryData.SaveInventoryRecord(inventory);
			}
		}
	}
}

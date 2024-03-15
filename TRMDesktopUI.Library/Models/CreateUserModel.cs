using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TRMDesktopUI.Library.Models
{
	public class CreateUserModel
	{
		[Required]
		[DisplayName("First Name")]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[DisplayName("Last Name")]
		public string LastName { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[DisplayName("Email Address")]
		public string EmailAddress { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;

		[Required]
		[DisplayName("Confirm Password")]
		[Compare(nameof(Password), ErrorMessage = "The passwords do not match")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}

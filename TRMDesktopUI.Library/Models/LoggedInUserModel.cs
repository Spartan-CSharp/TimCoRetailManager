namespace TRMDesktopUI.Library.Models
{
	public class LoggedInUserModel : ILoggedInUserModel
	{
		public string Token { get; set; } = string.Empty;
		public string Id { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string EmailAddress { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }

		public void ResetUserModel()
		{
			Token = "";
			Id = "";
			FirstName = "";
			LastName = "";
			EmailAddress = "";
			CreatedDate = DateTime.MinValue;
		}
	}
}

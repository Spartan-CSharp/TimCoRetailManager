namespace TRMDataManager.Library.Models
{
	public class UserModel
	{
		public string Id { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string EmailAddress { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
	}
}

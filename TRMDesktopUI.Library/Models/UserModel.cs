namespace TRMDesktopUI.Library.Models
{
	public class UserModel
	{
		public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public Dictionary<string, string> Roles { get; set; } = [];

		public string RoleList
		{
			get
			{
				return string.Join(", ", Roles.Select(x => x.Value));
			}
		}
	}
}

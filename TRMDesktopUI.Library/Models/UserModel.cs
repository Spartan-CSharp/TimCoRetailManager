using System.Collections.Generic;
using System.Linq;

namespace TRMDesktopUI.Library.Models
{
	public class UserModel
	{
		public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();

		public string RoleList
		{
			get
			{
				return string.Join(", ", Roles.Select(x => x.Value));
			}
		}

	}
}

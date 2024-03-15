using System.Collections.Generic;

namespace TRMApi.Models
{
	public class ApplicationUserModel
	{
		public string Id { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();
	}
}

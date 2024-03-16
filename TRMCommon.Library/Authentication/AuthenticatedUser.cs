namespace TRMCommon.Library.Authentication
{
	public class AuthenticatedUser
	{
		public string Access_Token { get; set; } = string.Empty;
		public string Token_Type { get; set; } = string.Empty;
		public int ExpiresIn { get; set; }
		public string UserName { get; set; } = string.Empty;
		public DateTime Issued { get; set; }
		public DateTime Expires { get; set; }
	}
}

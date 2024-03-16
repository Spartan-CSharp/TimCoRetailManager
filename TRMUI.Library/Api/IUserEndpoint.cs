using TRMUI.Library.Models;

namespace TRMUI.Library.Api
{
	public interface IUserEndpoint
	{
		Task<List<UserModelWithRoles>> GetAll();
		Task<Dictionary<string, string>> GetAllRoles();
		Task AddUserToRole(string userId, string roleName);
		Task RemoveUserFromRole(string userId, string roleName);
		Task CreateUser(CreateUserModel model);
	}
}
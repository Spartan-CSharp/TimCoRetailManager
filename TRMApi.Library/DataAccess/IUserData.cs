using TRMCommon.Library.Models;

namespace TRMApi.Library.DataAccess
{
	public interface IUserData
	{
		void CreateUser(UserModel user);
		List<UserModel> GetUserById(string id);
	}
}
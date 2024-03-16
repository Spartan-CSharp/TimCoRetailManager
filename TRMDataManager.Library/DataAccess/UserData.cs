using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class UserData(ISqlDataAccess sql) : IUserData
	{
		private readonly ISqlDataAccess _sql = sql;

		public List<UserModel> GetUserById(string id)
		{
			List<UserModel> output = _sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", new { id }, "TRMData");

			return output;
		}

		public void CreateUser(UserModel user)
		{
			_sql.SaveData("dbo.spUser_Insert", new { user.Id, user.FirstName, user.LastName, user.EmailAddress }, "TRMData");
		}
	}
}

using System.Data;
using System.Data.SqlClient;

using Dapper;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TRMDataManager.Library.DataAccess
{
	public class SqlDataAccess(IConfiguration config, ILogger<SqlDataAccess> logger) : IDisposable, ISqlDataAccess
	{
		public string GetConnectionString(string name)
		{
			return _config.GetConnectionString(name) ?? throw new InvalidOperationException($"Connection string '{name}' not found.");
		}

		public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
		{
			string connectionString = GetConnectionString(connectionStringName);

			using ( IDbConnection connection = new SqlConnection(connectionString) )
			{
				List<T> rows = connection.Query<T>(storedProcedure, parameters,
					commandType: CommandType.StoredProcedure).ToList();

				return rows;
			}
		}

		public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
		{
			string connectionString = GetConnectionString(connectionStringName);

			using ( IDbConnection connection = new SqlConnection(connectionString) )
			{
				_ = connection.Execute(storedProcedure, parameters,
					commandType: CommandType.StoredProcedure);
			}
		}

		private IDbConnection? _connection;
		private IDbTransaction? _transaction;

		public void StartTransaction(string connectionStringName)
		{
			string connectionString = GetConnectionString(connectionStringName);

			_connection = new SqlConnection(connectionString);
			_connection.Open();

			_transaction = _connection.BeginTransaction();

			_isClosed = false;
		}

		public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
		{
			List<T> rows = _connection is not null
				? _connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ToList()
				: [];

			return rows;
		}

		public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
		{
			_ = (_connection?.Execute(storedProcedure, parameters,
					commandType: CommandType.StoredProcedure, transaction: _transaction));
		}

		private bool _isClosed = false;
		private readonly IConfiguration _config = config;
		private readonly ILogger<SqlDataAccess> _logger = logger;

		public void CommitTransaction()
		{
			_transaction?.Commit();
			_connection?.Close();

			_isClosed = true;
		}

		public void RollbackTransaction()
		{
			_transaction?.Rollback();
			_connection?.Close();

			_isClosed = true;
		}

		public void Dispose()
		{
			if ( _isClosed == false )
			{
				try
				{
					CommitTransaction();
				}
				catch ( Exception ex )
				{
					_logger.LogError(ex, "Commit transaction failed in the dispose method.");
				}
			}

			GC.SuppressFinalize(this);

			_transaction = null;
			_connection = null;
		}
	}
}

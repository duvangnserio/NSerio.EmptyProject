using NSerio.EmptyProject.Core.Models;
using NSerio.EmptyProject.Core.Resources;
using NSerio.Utils.Relativity;
using Relativity.API;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Repositories.Implementation
{
	internal class TransactionTestRepository : ITransactionTestRepository
	{
		//private readonly IRelativityDBContext _dbContext;

		//public TransactionTestRepository(IRelativityDBContext dbContext)
		//{
		//	_dbContext = dbContext;
		//}

		public async Task<int> CreateJobAsync(IDBContext dbContext, TransactionTests_Job job)
		{
			//Create a job in TransactionTests_Job table
			//Creates an sql to create the table in the database

			string sql = SqlStatement.TransactionTests_CreateJob;

			var parameters = new List<SqlParameter>()
			{
				new SqlParameter
				{
					ParameterName = nameof(TransactionTests_Job.Name),
					Value = job.Name,
					SqlDbType = System.Data.SqlDbType.NVarChar
				},
			};

			return await dbContext.ExecuteSqlStatementAsScalarAsync<int>(sql, parameters);
		}

		public async Task CreateTransactionAsync(IDBContext dbContext, TransactionTests_Transactions transaction)
		{
			//Create a transaction in TransactionTests_Transactions table

			string sql = SqlStatement.TransactionTests_CreateJobTransactions;
			var parameters = new List<SqlParameter>()
				{
				new SqlParameter
				{
					ParameterName = nameof(TransactionTests_Transactions.JobId),
					Value = transaction.JobId,
					SqlDbType = System.Data.SqlDbType.Int
						},
				new SqlParameter
					{
					ParameterName = nameof(TransactionTests_Transactions.Name),
					Value = transaction.Name,
					SqlDbType = System.Data.SqlDbType.NVarChar
						},
				};

			await dbContext.ExecuteNonQuerySQLStatementAsync(sql, parameters);
		}
	}
}
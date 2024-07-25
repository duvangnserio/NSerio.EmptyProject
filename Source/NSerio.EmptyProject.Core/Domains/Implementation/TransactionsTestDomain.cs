using NSerio.EmptyProject.Core.DataProviders;
using NSerio.EmptyProject.Core.Models;
using NSerio.Utils.Relativity;
using Relativity.API;
using System;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains.Implementation
{
	internal class TransactionsTestDomain : ITransactionsTestDomain
	{
		private readonly IRelativityDBContext _dbContext;
		private readonly ITransactionTestDataProvider _dataProvider;

		public TransactionsTestDomain(IRelativityDBContext dbContext, ITransactionTestDataProvider dataProvider)
		{
			_dbContext = dbContext;
			_dataProvider = dataProvider;
		}

		public async Task CreateJobWithTransactionsAsync()
		{
			//Create a job in TransactionTests_Job table and then create a transaction in TransactionTests_Transactions table with the job id from the previous step
			//If the transaction fails, the job should be rolled back

			//Begin a transaction

			IDBContext dbContext = _dbContext.MasterDB();
			dbContext.BeginTransaction();
			try
			{
				//Create a job
				var job = new TransactionTests_Job
				{
					Name = "Job 1"
				};

				int jobId = await _dataProvider.CreateJobAsync(dbContext, job);

				//Create a transaction

				var transaction = new TransactionTests_Transactions
				{
					JobId = jobId,
					Name = "Transaction 1"
				};

				//await _dataProvider.CreateTransactionAsync(dbContext, transaction);

				throw new Exception("Transaction failed");

				//Commit the transaction

				dbContext.CommitTransaction();
			}
			catch (Exception)
			{
				dbContext.RollbackTransaction();
				throw;
			}
		}
	}
}

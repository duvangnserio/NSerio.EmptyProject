using NSerio.EmptyProject.Core.Models;
using NSerio.EmptyProject.Core.Repositories;
using Relativity.API;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.DataProviders.Implementation
{
	internal class TransactionTestDataProvider : ITransactionTestDataProvider
	{
		private readonly ITransactionTestRepository _repository;

		public TransactionTestDataProvider(ITransactionTestRepository repository)
		{
			_repository = repository;
		}

		public Task<int> CreateJobAsync(IDBContext dbContext, TransactionTests_Job job)
		{
			//Create a job in TransactionTests_Job table
			return _repository.CreateJobAsync(dbContext, job);
		}

		public Task CreateTransactionAsync(IDBContext dbContext, TransactionTests_Transactions transaction)
		{
			//Create a transaction in TransactionTests_Transactions table
			return _repository.CreateTransactionAsync(dbContext, transaction);
		}
	}
}
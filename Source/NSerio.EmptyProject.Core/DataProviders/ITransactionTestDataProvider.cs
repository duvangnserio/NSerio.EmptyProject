using NSerio.EmptyProject.Core.Models;
using Relativity.API;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.DataProviders
{
	internal interface ITransactionTestDataProvider : IDataProvider
	{
		Task<int> CreateJobAsync(IDBContext dbContext, TransactionTests_Job job);
		Task CreateTransactionAsync(IDBContext dbContext, TransactionTests_Transactions transaction);
	}
}
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains
{
	public interface ITransactionsTestDomain : IDomain
	{
		Task CreateJobWithTransactionsAsync();
	}
}
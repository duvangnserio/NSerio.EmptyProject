using NSerio.EmptyProject.Core.Domains;
using Relativity.API;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NSerio.EmptyProject.Test.Integration.Domains
{
	public class TransactionTestsDomainTests : IntegrationTestBase<IHelper>
	{
		[Fact]
		public async Task ExecuteAutoLinkerJobAsync_Success_TestAsync()
		{
			await ExecuteDomainServiceAsync<ITransactionsTestDomain>(t => t.CreateJobWithTransactionsAsync());
		}

		public TransactionTestsDomainTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}
	}
}

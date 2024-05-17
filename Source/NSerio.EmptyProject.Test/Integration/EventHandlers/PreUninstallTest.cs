using NSerio.EmptyProject.Core.Domains;
using Relativity.API;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NSerio.EmptyProject.Test.Integration.EventHandlers
{
	public class PreUninstallTest : IntegrationTestBase<IEHHelper>
	{
		#region [Constructor]

		public PreUninstallTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

		#endregion

		#region [Methods]

		[Fact]
		public async Task Execute()
		{
			bool successful = true;

			try
			{
				await ExecuteDomainServiceAsync<IUninstallDomain>(s => s.UninstallApplicationAsync(Helper));
			}
			catch (Exception)
			{
				successful = false;
			}

			Assert.True(successful);
		}

		#endregion
	}
}
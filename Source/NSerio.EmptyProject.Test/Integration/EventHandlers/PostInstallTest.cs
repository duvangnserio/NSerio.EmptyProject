using NSerio.EmptyProject.Core.Domains;
using Relativity.API;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NSerio.EmptyProject.Test.Integration.EventHandlers
{
	public class PostInstallTest : IntegrationTestBase<IEHHelper>
	{
		#region [Constructor]

		public PostInstallTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

		#endregion

		#region [Methods]

		[Fact]
		public async Task ExecutePostInstallInstance()
		{
			bool successful = true;

			try
			{
				await ExecuteDomainServiceAsync<IInstallerDomain>(s => s.InstallApplicationAtInstanceLevelAsync());
			}
			catch (Exception)
			{
				successful = false;
			}

			Assert.True(successful);
		}

		[Fact]
		public async Task ExecutePostInstallWorkspace()
		{
			bool successful = true;

			try
			{
				await ExecuteDomainServiceAsync<IInstallerDomain>(s => s.InstallApplicationAtWorkspaceLevelAsync(Helper));
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
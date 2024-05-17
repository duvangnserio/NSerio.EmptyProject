using NSerio.EmptyProject.Core;
using NSerio.EmptyProject.Core.Extensions;
using NSerio.Utils;
using Relativity.API;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace NSerio.EmptyProject.Test.Integration
{
	public abstract class IntegrationTestBase<THelper>
		where THelper : class, IHelper
	{
		public readonly int WORKSPACE_ID = int.Parse(ConfigurationManager.AppSettings["WorkspaceID"] ?? "-1");

		public THelper Helper => TestHelper.GetHelper<THelper>();

		public ITestOutputHelper TestOutputHelper { get; }

		public IDomainManager DomainManager => Helper.GetDomainManager();

		public IntegrationTestBase(ITestOutputHelper testOutputHelper)
		{
			TestOutputHelper = testOutputHelper;
		}

		protected void TraceInformation(string msg)
		{
			System.Diagnostics.Trace.TraceInformation(msg);
		}

		protected async Task<V> ExecuteDomainServiceAsync<T, V>(Func<T, Task<V>> serviceMethodAsync) where T : class, IDomain
		{
			try
			{
				T domainProxy = DomainManager.CreateProxy<T>();
				return await serviceMethodAsync(domainProxy);
			}
			catch (Exception ex)
			{
				TestOutputHelper.WriteLine(ex.InnerException.ToJSON(true));
				throw;
			}
		}

		protected async Task ExecuteDomainServiceAsync<T>(Func<T, Task> serviceMethodAsync) where T : class, IDomain
		{
			try
			{
				T domainProxy = DomainManager.CreateProxy<T>();
				await serviceMethodAsync(domainProxy);
			}
			catch (Exception ex)
			{
				TestOutputHelper.WriteLine(ex.InnerException.ToJSON(true));
				throw;
			}
		}
	}
}

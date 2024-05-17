using NSerio.EmptyProject.Core;
using NSerio.EmptyProject.Core.Extensions;
using NSerio.EmptyProject.Core.Helpers;
using NSerio.Utils;
using NSerio.Utils.Relativity;
using Relativity.API;
using System;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Kepler.Services
{
	public class KeplerManagerBase : IDisposable
	{
		#region [Properties]

		protected virtual string ErrorSource => ErrorHelper.GetErrorSource("KeplerManager");

		protected internal IHelper Helper { get; }
		protected internal IDomainManager DomainManager { get; }
		protected internal IUserInfo UserInfo { get; }

		#endregion

		#region [Constructor]

		public KeplerManagerBase(IHelper helper)
		{
			var serviceHelper = (IServiceHelper)helper;
			Helper = helper;
			UserInfo = serviceHelper.GetAuthenticationManager().UserInfo;
			DomainManager = Helper.GetDomainManager();
		}

		#endregion

		#region [Methods]

		protected async Task<V> ExecuteDomainServiceAsync<T, V>(int workspaceId, Func<T, Task<V>> serviceMethodAsync) where T : class, IDomain
		{
			try
			{
				T domainProxy = DomainManager.CreateProxy<T>();
				return await serviceMethodAsync(domainProxy);
			}
			catch (Exception ex)
			{
				throw await ErrorHandlerAsync(workspaceId, ex);
			}
		}

		protected async Task ExecuteDomainServiceAsync<T>(int workspaceId, Func<T, Task> serviceMethodAsync) where T : class, IDomain
		{
			try
			{
				T domainProxy = DomainManager.CreateProxy<T>();
				await serviceMethodAsync(domainProxy);
			}
			catch (Exception ex)
			{
				throw await ErrorHandlerAsync(workspaceId, ex);
			}
		}

		protected async Task<ApplicationException> ErrorHandlerAsync(int workspaceId, Exception exception)
		{
			await Helper.AddToRelativityErrorTabAsync(workspaceId, exception, ErrorSource);
			return new ApplicationException($"{Constants.APPLICATION_NAME} | Kepler Error: '{exception.GetException()}'");
		}

		public void Dispose()
		{
			if (Helper != null)
			{
				Helper.Dispose();
			}

			DomainManager?.Dispose();
		}

		#endregion
	}
}

using NSerio.EmptyProject.Core.Infrastructure;
using Relativity.API;
using SimpleInjector;
using System;

namespace NSerio.EmptyProject.Core
{
	internal class DomainManager : IDomainManager
	{
		internal IHelper Helper { get; }
		internal Lazy<Scope> Scope { get; }
		internal Container Container => Scope.Value.Container;

		public DomainManager(IHelper helper)
		{
			Helper = helper ?? throw new ArgumentNullException(nameof(helper));
			Scope = new Lazy<Scope>(BuildScopeBase);
		}

		protected virtual Scope BuildScopeBase()
			=> DependencyContainer.GetAsyncScopedLifestyle(Helper);

		public virtual T CreateProxy<T>() where T : class, IDomain
		{
			return Container.GetInstance<T>();
		}

		#region [ IDisposable ]

		protected bool _disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					Scope?.Value?.Dispose();
				}

				_disposed = true;
			}
		}

		~DomainManager()
		{
			Dispose(false);
		}

		#endregion [ IDisposable ]
	}
}
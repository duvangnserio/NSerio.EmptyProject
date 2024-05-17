using Relativity.API;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace NSerio.EmptyProject.Core.Infrastructure
{
	internal static class DependencyContainer
	{
		public static Container Container { get; }
		private static bool _isReady = false;
		private static object _lock = new object();

		static DependencyContainer()
		{
			Container = new Container();

			// container options
			Container.Options.DefaultLifestyle = Lifestyle.Scoped;
			Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
		}

		public static Scope GetAsyncScopedLifestyle(IHelper helper)
		{
			lock (_lock)
			{
				if (!_isReady)
				{
					// container options
					Container.RegisterCore(helper);

					_isReady = true;

					// verify container
					Container.Verify();
				}
			}

			// scope
			return AsyncScopedLifestyle.BeginScope(Container);
		}
	}
}

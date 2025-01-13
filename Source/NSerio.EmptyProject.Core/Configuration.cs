using NSerio.EmptyProject.Core.RetryPolicies;
using NSerio.Utils.Relativity;
using NSerio.Utils.SimpleInjector;
using Polly.Registry;
using Relativity.API;
using Relativity.Services.Objects;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NSerio.EmptyProject.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace NSerio.EmptyProject.Core
{
	internal static class Configuration
	{
		private static IEnumerable<Assembly> Assemblies { get; } = new Assembly[]
		{
			Assembly.GetExecutingAssembly()
		};

		internal static void RegisterCore(this Container container, IHelper helper)
		{
			container.RegisterCommon();
			container.RegisterPollyRetryPolicies();
			container.RegisterInstallScripts();
			container.RegisterUninstallScripts();
			container.RegisterPreInstallScripts();
			container.RegisterRelativity(helper);

			// register from type (dynamically)
			container.RegisterAllServiceTypeImplementations<IInjectable>(Assemblies);
		}

		private static void RegisterCommon(this Container container)
		{
			container.Register<IRelativityDBContext, RelativityDBContext>();
		}

		private static void RegisterPollyRetryPolicies(this Container container)
		{
			var policyRegistry = new PolicyRegistry();

			policyRegistry.RegisterSqlAsyncRetryPolicy();

			container
				.RegisterInstance<IReadOnlyPolicyRegistry<string>>(policyRegistry);
		}

		private static void RegisterInstallScripts(this Container container)
		{
			// ordered list of installation steps
			var instanceInstallers = new Type[]
			{
				// e.g typeof(ConfigurationInstaller)
			};

			container.Collection.Register<IInstanceInstaller>(instanceInstallers);

			var workspaceInstallers = new Type[]
			{
				// e.g typeof(ConfigurationInstaller)
			};

			container.Collection.Register<IWorkspaceInstaller>(workspaceInstallers);
		}

		private static void RegisterUninstallScripts(this Container container)
		{
			// ordered list of uninstallation steps
			var uninstallItems = new Type[]
			{
				// e.g  typeof(CaseDBUninstaller)
			};

			container.Collection.Register<IUninstaller>(uninstallItems);
		}

		private static void RegisterPreInstallScripts(this Container container)
		{
			// ordered list of pre-install steps
			var workspacePreInstallers = new Type[]
			{
				// e.g  typeof(PreConfigurationInstaller)
			};

			container.Collection.Register<IWorkspacePreInstaller>(workspacePreInstallers);
		}

		private static void RegisterRelativity(this Container container, IHelper helper)
		{
			container.RegisterSingleton(() => helper);
			container.RegisterSingleton(() => helper.GetLoggerFactory().GetLogger());
			container.RegisterSingleton(helper.GetSecretStore);

			container.Register(() => helper.GetServicesManager());
			container.Register(() => helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser));
		}
	}
}
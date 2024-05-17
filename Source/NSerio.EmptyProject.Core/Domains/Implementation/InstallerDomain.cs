using NSerio.Utils;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains.Implementation
{
	internal class InstallerDomain : IInstallerDomain
	{
		private readonly IAPILog _logger;
		private readonly IEnumerable<IInstanceInstaller> _instanceInstallers;
		private readonly IEnumerable<IWorkspaceInstaller> _workspaceInstallers;
		private readonly IEnumerable<IWorkspacePreInstaller> _workspacePreInstallers;

		public InstallerDomain(IAPILog logger, IEnumerable<IInstanceInstaller> instanceInstallers, IEnumerable<IWorkspaceInstaller> workspaceInstallers, IEnumerable<IWorkspacePreInstaller> workspacePreInstallers)
		{
			_logger = logger;
			_instanceInstallers = instanceInstallers;
			_workspaceInstallers = workspaceInstallers;
			_workspacePreInstallers = workspacePreInstallers;
		}

		public async Task InstallApplicationAtInstanceLevelAsync()
		{
			if (_instanceInstallers.HasValues())
			{
				foreach (var installer in _instanceInstallers)
				{
					var installerType = installer.GetType();

					try
					{
						_logger.LogDebug($"Running the installer '{installerType}'");

						await installer.ExecuteAsync().ConfigureAwait(false);
					}
					catch (Exception exception)
					{
						throw new ApplicationException($"There was an error running {installerType}. {exception.ToJSON()}", exception);
					}
				}
			}
		}

		public async Task InstallApplicationAtWorkspaceLevelAsync(IEHHelper helper)
		{
			if (_workspaceInstallers.HasValues())
			{
				foreach (var installer in _workspaceInstallers)
				{
					var installerType = installer.GetType();

					try
					{
						_logger.LogDebug($"Running the installer '{installerType}'");

						await installer.ExecuteAsync(helper).ConfigureAwait(false);
					}
					catch (Exception exception)
					{
						throw new ApplicationException($"There was an error running {installerType}. {exception.ToJSON()}", exception);
					}
				}
			}
		}

		public async Task PreInstallApplicationAtWorkspaceLevelAsync(IEHHelper helper)
		{
			if (_workspaceInstallers.HasValues())
			{
				foreach (var installer in _workspacePreInstallers)
				{
					var installerType = installer.GetType();

					try
					{
						_logger.LogDebug($"Running the pre-installer '{installerType}'");

						await installer.ExecuteAsync(helper).ConfigureAwait(false);
					}
					catch (Exception exception)
					{
						throw new ApplicationException($"There was an error running {installerType}. {exception.ToJSON()}", exception);
					}
				}
			}
		}
	}
}

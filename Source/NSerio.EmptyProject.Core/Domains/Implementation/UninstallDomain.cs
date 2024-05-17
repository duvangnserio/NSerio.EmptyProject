using NSerio.Utils;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains.Implementation
{
	internal class UninstallDomain : IUninstallDomain
	{
		private readonly IAPILog _logger;
		private readonly IEnumerable<IUninstaller> _unInstallers;

		public UninstallDomain(IAPILog logger, IEnumerable<IUninstaller> unInstallers)
		{
			_logger = logger;
			_unInstallers = unInstallers;
		}

		public async Task UninstallApplicationAsync(IEHHelper helper)
		{
			if (_unInstallers.HasValues())
			{
				foreach (var unInstaller in _unInstallers)
				{
					var unInstallerType = unInstaller.GetType();

					try
					{
						_logger.LogDebug($"Running the installer '{unInstallerType}'");

						await unInstaller.ExecuteAsync(helper).ConfigureAwait(false);
					}
					catch (Exception exception)
					{
						throw new ApplicationException($"There was an error running {unInstallerType}. {exception.ToJSON()}", exception);
					}
				}
			}
		}
	}
}

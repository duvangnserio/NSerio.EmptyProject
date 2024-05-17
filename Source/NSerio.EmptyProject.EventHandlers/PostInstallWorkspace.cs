using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using NSerio.EmptyProject.Core.Domains;
using NSerio.EmptyProject.Core.Extensions;
using NSerio.EmptyProject.Core.Helpers;
using NSerio.Utils;
using NSerio.Utils.Relativity;
using System;
using System.Runtime.InteropServices;

namespace NSerio.EmptyProject.EventHandlers
{
	[RunOnce(false)]
	[Guid("4e588936-f85e-496d-8b57-6fb3a2a8ea83")]
	[Description("Creates the underlying tables and objects for the workspace")]
	[RunTarget(kCura.EventHandler.Helper.RunTargets.Workspace)]
	public class PostInstallWorkspace : PostInstallEventHandler
	{
		protected string ErrorSource => ErrorHelper.GetErrorSource(nameof(PostInstallWorkspace));

		public override Response Execute()
		{
			bool success = false;
			string message = string.Empty;

			try
			{
				using (var domainManager = Helper.GetDomainManager())
				{
					domainManager
						.CreateProxy<IInstallerDomain>()
						.InstallApplicationAtWorkspaceLevelAsync(Helper)
						.ToSync();
				}

				success = true;
			}
			catch (Exception exception)
			{
				message = exception.Message;
				int workspaceArtifactId = Helper.GetActiveCaseID();
				Helper.AddToRelativityErrorTab(workspaceArtifactId, exception, ErrorSource);
			}

			return new Response
			{
				Success = success,
				Message = message
			};
		}
	}
}
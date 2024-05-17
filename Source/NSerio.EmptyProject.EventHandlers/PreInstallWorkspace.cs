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
	[Guid("5b9e7aeb-5dce-4e89-bd4b-d8b07ea234e4")]
	[Description("Creates the underlying tables and objects for the workspace")]
	[RunTarget(kCura.EventHandler.Helper.RunTargets.Workspace)]
	public class PreInstallWorkspace : PreInstallEventHandler
	{
		protected string ErrorSource => ErrorHelper.GetErrorSource(nameof(PreInstallWorkspace));

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
						.PreInstallApplicationAtWorkspaceLevelAsync(Helper)
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
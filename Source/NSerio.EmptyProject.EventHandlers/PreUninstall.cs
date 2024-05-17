using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using NSerio.EmptyProject.Core.Domains;
using NSerio.EmptyProject.Core.Extensions;
using NSerio.Utils;
using NSerio.Utils.Relativity;
using System;
using System.Runtime.InteropServices;

namespace NSerio.EmptyProject.EventHandlers
{
	[RunOnce(false)]
	[Guid("0740e88d-f674-4ce4-9141-94f39c866167")]
	[Description("Remove the workspace tables and mark the files to be deleted")]
	public class PreUninstall : PreUninstallEventHandler
	{
		public override Response Execute()
		{
			bool success = false;
			string message = string.Empty;

			try
			{
				using (var domainManager = Helper.GetDomainManager())
				{
					domainManager
							.CreateProxy<IUninstallDomain>()
							.UninstallApplicationAsync(Helper)
							.ToSync();
				}

				success = true;
			}
			catch (Exception exception)
			{
				message = exception.Message;
				Helper.AddToRelativityErrorTab(Core.Constants.APP_ADMIN_CASE_ID, exception, "PreUninstallSetup");
			}

			return new Response
			{
				Success = success,
				Message = message
			};
		}
	}
}
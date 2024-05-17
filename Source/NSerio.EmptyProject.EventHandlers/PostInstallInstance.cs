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
	[Guid("40683ef1-26b0-49d4-9c5d-86135c0da7ef")]
	[Description("Creates the underlying tables and instance settings for the application")]
	[RunTarget(kCura.EventHandler.Helper.RunTargets.Instance)]
	public class PostInstallInstance : PostInstallEventHandler
	{
		protected string ErrorSource => ErrorHelper.GetErrorSource(nameof(PostInstallInstance));

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
						.InstallApplicationAtInstanceLevelAsync()
						.ToSync();
				}

				success = true;
			}
			catch (Exception exception)
			{
				message = exception.Message;
				Helper.AddToRelativityErrorTab(Core.Constants.APP_ADMIN_CASE_ID, exception, ErrorSource);
			}

			return new Response
			{
				Success = success,
				Message = message
			};
		}
	}
}
using Cimplifi.CILicensing.LicenseValidator.Helpers;
using kCura.EventHandler;
using kCura.EventHandler.CustomAttributes;
using NSerio.Utils;
using System;
using System.Runtime.InteropServices;

namespace NSerio.EmptyProject.EventHandlers
{
	[RunOnce(false)]
	[Guid("A4265498-FBA4-4D52-9471-2FEE0CA91022")]
	[Description("Validate business rules after of Save.")]
	public class TodoListPostSave : PostSaveEventHandler
	{
		public override Response Execute()
		{
			Helper.GetLoggerFactory().GetLogger().LogInformation("Executing TodoListPostSave...");

			var licenseValid = Helper.ValidateLicenseAsync().ToSync();

			if (!licenseValid)
			{
				Helper.GetLoggerFactory().GetLogger().LogError("License is invalid");
				return new Response { Success = false, Message = "License is invalid" };
			}

			Helper.GetLoggerFactory().GetLogger().LogInformation("License is valid");

			return new Response { Success = true, Message = "License is valid" };
		}

		public override FieldCollection RequiredFields { get; }
	}
}

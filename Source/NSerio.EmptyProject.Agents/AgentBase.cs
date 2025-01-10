using Cimplifi.CILicensing.LicenseValidator.Helpers;
using NSerio.EmptyProject.Core.Helpers;
using NSerio.Utils;
using NSerio.Utils.Relativity;
using Relativity.API;
using System;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Agents
{
	public abstract class AgentBase : kCura.Agent.AgentBase
	{
		public override string Name { get; }
		protected IAPILog Logger => Helper.GetLoggerFactory().GetLogger();
		private const int DEFAULT_LOGGING_LEVEL = 10;

		public AgentBase()
		{
			var agentNameAttribute = (kCura.Agent.CustomAttributes.Name)Attribute.GetCustomAttribute(this.GetType(), typeof(kCura.Agent.CustomAttributes.Name));
			if (agentNameAttribute == null)
			{
				throw new InvalidOperationException("The attribute 'kCura.Agent.CustomAttributes.Name' not found");
			}

			Name = agentNameAttribute.Name;
		}

		public AgentBase(string agentName)
		{
			if (!agentName.HasValue())
			{
				throw new ArgumentException("Is null or empty", nameof(agentName));
			}

			Name = agentName;
		}

		protected abstract Task ExecuteAsync();

		public override void Execute()
		{
			string startingMessage = $"Starting ({DateTime.UtcNow.Millisecond})...";
			RaiseMessageBase(startingMessage);

			try
			{
				Helper.ValidateLicenseWithinAgentAsync().ToSync();

				ExecuteAsync().ToSync();
			}
			catch (Exception exception)
			{
				Helper.AddToRelativityErrorTab(Core.Constants.APP_ADMIN_CASE_ID, exception, Name.GetErrorSource());
				string errorMessage = $"ERROR: {exception.Message}";
				RaiseError(errorMessage, exception.ToString());
			}
		}

		public void RaiseMessageBase(string message)
		{
			RaiseMessage(message, DEFAULT_LOGGING_LEVEL);
			Logger?.LogInformation("{@Agent} {@Message}", Name, message);
		}

		public override void RaiseError(string message, string detailMessage)
		{
			base.RaiseError(message, detailMessage);
			Logger?.LogError("Something occurred in {@Agent} {@Message}", Name, message);
		}
	}
}
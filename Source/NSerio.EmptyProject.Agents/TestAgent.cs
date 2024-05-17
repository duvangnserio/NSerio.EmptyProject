using kCura.Agent.CustomAttributes;
using NSerio.EmptyProject.Core.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WorkloadDiscovery.CustomAttributes;

namespace NSerio.EmptyProject.Agents
{
	[Name("Test Agent")]
	[Guid("4ba10ea2-f92d-453b-a2f3-e52be2d2ca98")]
	[Path(_WORKLOAD_ENPOINT)]
	public class TestAgent : AgentBase
	{
		private const string _WORKLOAD_ENPOINT = "Relativity.REST/api/NSerioEmptyProjectModule/workload-agent/{guid}/get-workload";


		protected override async Task ExecuteAsync()
		{
			using (var domainManager = Helper.GetDomainManager())
			{
				string progressMessage = "In progress...";
				RaiseMessageBase(progressMessage);
				await Task.Delay(5000);
				string completedMessage = "Completed...";
				RaiseMessageBase(completedMessage);
			}
		}
	}
}
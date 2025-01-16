using kCura.Agent.CustomAttributes;
using NSerio.EmptyProject.Core;
using NSerio.EmptyProject.Core.Domains;
using NSerio.EmptyProject.Core.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WorkloadDiscovery.CustomAttributes;

namespace NSerio.EmptyProject.Agents
{
	[Name("Generate Sas Token Test Agent")]
	[Guid("388AF2DC-D748-4604-9E94-EF9150A344B8")]
	[Path(_WORKLOAD_ENDPOINT)]
	public class GenerateSasTokenAgent : AgentBase
	{
		private const string _WORKLOAD_ENDPOINT = "Relativity.REST/api/NSerioEmptyProject/workload-agent/388AF2DC-D748-4604-9E94-EF9150A344B8/get-workload";
		protected override async Task ExecuteAsync()
		{
			RaiseMessage("Starting Generate Sas Token Agent", 10);

			int workspaceId = 10585512;
			using IDomainManager domainManager = Helper.GetDomainManager();
			var token = await domainManager.CreateProxy<ITransferDomain>().GetReadOnlyTokenAsync(workspaceId);

			RaiseMessage($"Token: {token}", 10);

			RaiseMessage("Generate Sas Token Agent completed", 10);
		}
	}
}

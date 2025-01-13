using kCura.Agent.CustomAttributes;
using NSerio.EmptyProject.Core.Helpers;
using NSerio.EmptyProject.Core.Repositories.Implementation;
using NSerio.Utils;
using NSerio.Utils.Relativity;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WorkloadDiscovery.CustomAttributes;

namespace NSerio.EmptyProject.Agents
{
	[Name("Api Call Render Test Agent")]
	[Guid("54387B4A-0508-48CB-957E-9EC1B9FCB88F")]
	[Path(_WORKLOAD_ENDPOINT)]
	public class ApiCallRenderTestAgent : AgentBase
	{
		private const string _WORKLOAD_ENDPOINT = "Relativity.REST/api/NSerioEmptyProject/workload-agent/54387B4A-0508-48CB-957E-9EC1B9FCB88F/get-workload";

		protected override async Task ExecuteAsync()
		{
			RaiseMessageBase("License is valid");
			var httpClientRepository = new HttpClientRepository();
			httpClientRepository.SetBaseUrl("https://naturemap-webapi.onrender.com/api/");

			var birds = await httpClientRepository.GetAsync<dynamic>("birds");
			var birdsJson = birds.ToJSON();
			var exceptionToLogBirds = new Exception("Birds from render API: " + birdsJson);
			await Helper.AddToRelativityErrorTabAsync(-1, exceptionToLogBirds, nameof(ApiCallRenderTestAgent).GetErrorSource());

			var createBirdResponse = await httpClientRepository.PostAsync<int>("birds", new { CommonName = "Pigeon", ScientificName = "Columba livia" });

			var exceptionToLogCreateBird = new Exception("Create bird response: " + createBirdResponse.ToJSON());
			await Helper.AddToRelativityErrorTabAsync(-1, exceptionToLogCreateBird, nameof(ApiCallRenderTestAgent).GetErrorSource());
		}
	}
}

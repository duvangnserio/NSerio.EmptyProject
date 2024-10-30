using kCura.Agent.CustomAttributes;
using NSerio.Utils.Relativity;
using Relativity.API;
using Relativity.Processing.V1.Services;
using Relativity.Processing.V1.Services.Interfaces.DTOs;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WorkloadDiscovery.CustomAttributes;

namespace NSerio.EmptyProject.Agents
{
	[Name("Test Agent CI Reporting Pivoting")]
	[Guid("4ba10ea2-f92d-453b-a2f3-e52be2d2ca98")]
	[Path(_WORKLOAD_ENPOINT)]
	public class TestAgent : AgentBase
	{
		private const string _WORKLOAD_ENPOINT = "Relativity.REST/api/NSerioEmptyProject/workload-agent/4ba10ea2-f92d-453b-a2f3-e52be2d2ca98/get-workload";


		protected override async Task ExecuteAsync()
		{
			int workspaceId = 3663333;
			GetDiscoveredDocumentsRequest request = new GetDiscoveredDocumentsRequest()
			{
				//Expression =
				//"{\"Type\":\"ConditionalExpression\",\"Property\":\"FileExtension\",\"Constraint\":\"15\",\"Value\":\"European\"}",
				StartingPointOfResult = 0,
			};
			using var proxy = Helper.GetServicesManager().CreateProxy<IProcessingFilterManager>(ExecutionIdentity.CurrentUser);
			ProcessingFilterData response = await proxy.GetDiscoveredDocumentsAsync(workspaceId, request);

			string fieldName = nameof(ProcessingFilterResult.FileExtension);

			var results = response.Results;

			// Pivot data taking into account the field name using reflection

			var pivotData = results
				.GroupBy(x => x.GetType().GetProperty(fieldName).GetValue(x))
				.Select(x => new
				{
					x.Key,
					Count = x.Count()
				})
				.ToList();

			StringBuilder messages = new StringBuilder();
			foreach (var x1 in pivotData)
			{
				messages.AppendLine($"Key: {x1.Key}, Count: {x1.Count}");
				RaiseMessageBase(messages.ToString());
			}

			var exception = new Exception(messages.ToString());

			await Helper.AddToRelativityErrorTabAsync(workspaceId, exception, nameof(TestAgent));
		}
	}
}
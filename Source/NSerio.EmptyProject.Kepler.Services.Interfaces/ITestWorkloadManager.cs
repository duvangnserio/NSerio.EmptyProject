using Relativity.Kepler.Services;
using System;
using System.Threading.Tasks;
using WorkloadDiscovery;

namespace NSerio.EmptyProject.Kepler.Services.Interfaces
{
	[WebService("Workload Service")]
	[ServiceAudience(Audience.Private)]
	[RoutePrefix("workload-agent")]
	public interface ITestWorkloadManager
	{
		/// <summary>
		/// Get the workload size for an agent type
		/// </summary>
		/// <param name="guid">The agent Guid identifier</param>
		/// <returns>A Workload object <seealso cref="Workload"/></returns>
		/// /// Example REST request:
		/// [GET] /Relativity.REST/api/NSerioEmptyProjectModule/workload-agent/{guid}/get-workload 
		[HttpGet]
		[Route("{guid}/get-workload")]
		Task<Workload> GetWorkloadAsync(Guid guid);
	}
}

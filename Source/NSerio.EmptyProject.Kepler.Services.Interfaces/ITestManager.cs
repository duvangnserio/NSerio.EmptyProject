using Relativity.Kepler.Services;
using System;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Kepler.Services.Interfaces
{
	[WebService("TestManager")]
	[ServiceAudience(Audience.Public)]
	public interface ITestManager : IDisposable
	{
		[HttpGet]
		[Route("Get")]
		Task<string> GetAsync();
	}
}

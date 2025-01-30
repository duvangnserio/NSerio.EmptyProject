using NSerio.EmptyProject.Core.Domains.Implementation;
using Relativity.Kepler.Services;
using System;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Kepler.Services.Interfaces
{
	[WebService("TestManager")]
	[ServiceAudience(Audience.Public)]
	[RoutePrefix("test-manager")]
	public interface ITestManager : IDisposable
	{
		[HttpGet]
		[Route("Get")]
		Task<string> GetAsync();


		[HttpGet]
		[Route("GenerateLicense/{instanceIdentifier}")]
		Task<string> GenerateLicenseAsync(string instanceIdentifier);

		[HttpPost]
		[Route("ValidateLicense")]
		Task<bool> ValidateLicenseAsync(ValidateLicenseRequestModel request);
	}
}

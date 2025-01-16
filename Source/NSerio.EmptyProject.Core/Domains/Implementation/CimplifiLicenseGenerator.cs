using Newtonsoft.Json;
using NSerio.EmptyProject.Core.Repositories.Implementation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains.Implementation
{
	public interface ICimplifiLicenseGenerator : IDomain
	{
		Task<string> GenerateLicenseAsync(string instanceIdentifier);
	}

	internal class CimplifiLicenseGenerator : ICimplifiLicenseGenerator
	{
		public async Task<string> GenerateLicenseAsync(string instanceIdentifier)
		{
			var httpClientRepository = new HttpClientRepository();
			httpClientRepository.SetBaseUrl("https://devhooks.cimplifi.com/");
			var headers = new Dictionary<string, string>
			{
				{ "X-Api-Key", "957574AE-D714-4F27-9415-BA1D972F9452" }
			};

			var license = await httpClientRepository
				.GetAsync<GenerateLicenseResponseModel>($"api/license/{instanceIdentifier}", headers);

			return license.Data.Data.Token;
		}
	}

	internal class GenerateLicenseResponseModel
	{
		[JsonProperty("data")]
		public LicenseContentModel Data { get; set; }
	}

	internal class LicenseContentModel
	{
		[JsonProperty("token")]
		public string Token { get; set; }

		[JsonProperty("publicKey")]
		public string PublicKey { get; set; }
	}
}

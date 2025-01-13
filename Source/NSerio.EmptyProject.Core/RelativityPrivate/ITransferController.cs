using NSerio.EmptyProject.Core.RelativityPrivate.DataContracts.GetReadonlyToken;
using Relativity.Kepler.Services;
using System;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.RelativityPrivate
{
	[RoutePrefix("Transfer")]
	[ServiceAudience(Audience.Private)]
	[WebService("Transfer Service")]
	public interface ITransferController : IDisposable
	{
		[HttpPost]
		[Route("GetReadOnlyToken")]
		Task<GetTokenResult> GetReadOnlyTokenAsync(GetReadOnlyTokenRequest request);
	}
}

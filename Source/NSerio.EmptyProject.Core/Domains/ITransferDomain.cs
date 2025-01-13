using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains
{
	public interface ITransferDomain : IDomain
	{
		Task<string> GetReadOnlyTokenAsync(int workspaceId);
	}
}

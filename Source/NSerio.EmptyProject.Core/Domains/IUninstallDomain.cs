using Relativity.API;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains
{
	public interface IUninstallDomain : IDomain
	{
		Task UninstallApplicationAsync(IEHHelper helper);
	}
}

using Relativity.API;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains
{
	public interface IInstallerDomain : IDomain
	{
		Task InstallApplicationAtInstanceLevelAsync();
		Task InstallApplicationAtWorkspaceLevelAsync(IEHHelper helper);
		Task PreInstallApplicationAtWorkspaceLevelAsync(IEHHelper helper);
	}
}

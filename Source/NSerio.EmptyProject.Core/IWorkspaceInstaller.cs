using Relativity.API;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core
{
	internal interface IWorkspaceInstaller
	{
		Task ExecuteAsync(IEHHelper helper);
	}
}
using Relativity.API;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core
{
	internal interface IWorkspacePreInstaller
	{
		Task ExecuteAsync(IEHHelper helper);
	}
}
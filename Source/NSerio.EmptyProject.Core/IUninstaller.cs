using Relativity.API;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core
{
	internal interface IUninstaller
	{
		Task ExecuteAsync(IEHHelper helper);
	}
}
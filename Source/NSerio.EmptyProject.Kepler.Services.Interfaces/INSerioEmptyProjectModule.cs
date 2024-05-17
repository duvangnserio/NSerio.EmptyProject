using NSerio.EmptyProject.Core;
using Relativity.Kepler.Services;

namespace NSerio.EmptyProject.Kepler.Services.Interfaces
{
	[ServiceModule("NSerioEmptyProjectModule")]
	[ServiceAudience(Audience.Private)]
	[RoutePrefix(Constants.APP_SERVICE_MODULE, VersioningStrategy.None)]
	public interface INSerioEmptyProjectModule { }
}

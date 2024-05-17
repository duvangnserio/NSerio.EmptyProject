using Relativity.API;

namespace NSerio.EmptyProject.Core.Extensions
{
	public static class RelativityHelperExtensions
	{
		public static IDomainManager GetDomainManager(this IHelper helper)
		{
			return new DomainManager(helper);
		}
	}
}
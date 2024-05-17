using System;

namespace NSerio.EmptyProject.Core
{
	public interface IDomainManager : IDisposable
	{
		T CreateProxy<T>() where T : class, IDomain;
	}
}
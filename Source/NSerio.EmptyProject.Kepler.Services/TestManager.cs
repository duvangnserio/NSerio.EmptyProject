using NSerio.EmptyProject.Core.Helpers;
using NSerio.EmptyProject.Kepler.Services.Interfaces;
using Relativity.API;
using System;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Kepler.Services
{
	public class TestManager : KeplerManagerBase, ITestManager
	{
		protected override string ErrorSource => ErrorHelper.GetErrorSource(nameof(TestManager));

		public TestManager(IHelper helper) : base(helper) { }

		public async Task<string> GetAsync()
		{
			await Task.Yield();
			var utcNowAsString = DateTime.UtcNow.ToString("yyyy/MM/dd hh:mm:ss.fff tt");
			return $"Utc Now: '{utcNowAsString}'";
		}
	}
}

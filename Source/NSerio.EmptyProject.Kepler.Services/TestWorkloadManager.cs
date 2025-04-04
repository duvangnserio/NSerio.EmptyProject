﻿using NSerio.EmptyProject.Core.Helpers;
using NSerio.EmptyProject.Kepler.Services.Interfaces;
using Relativity.API;
using System;
using System.Threading.Tasks;
using WorkloadDiscovery;

namespace NSerio.EmptyProject.Kepler.Services
{
	public class TestWorkloadManager : KeplerManagerBase, ITestWorkloadManager
	{
		protected override string ErrorSource => ErrorHelper.GetErrorSource(nameof(TestWorkloadManager));

		public TestWorkloadManager(IHelper helper) : base(helper) { }

		public Task<Workload> GetWorkloadAsync(Guid guid)
		{
			Workload workload = new Workload { Size = WorkloadSize.One };
			return Task.FromResult(workload);
		}
	}
}

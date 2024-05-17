using Moq;
using NSerio.EmptyProject.Test.Fake.Mock.Relativity;
using Relativity.API;
using System;
using System.Configuration;

namespace NSerio.EmptyProject.Test
{
	public static class TestHelper
	{
		//
		// From config file
		//
		private static bool RunSet => bool.Parse(ConfigurationManager.AppSettings["RunSet"]);
		private static int WorkspaceID => int.Parse(ConfigurationManager.AppSettings["WorkspaceID"]);
		private static int AdminUserArtifactId => int.Parse(ConfigurationManager.AppSettings["AdminUserArtifactId"]);

		//
		// Get Helper from <T>
		//
		public static T GetHelper<T>()
			where T : class, IHelper
		{
			dynamic helper;
			var helperName = typeof(T).Name;

			switch (helperName)
			{
				case "IHelper":
					helper = Helper;
					break;

				case "IAgentHelper":
					helper = AgentHelper;
					break;

				case "IEHHelper":
					helper = EventHandlerHelper;
					break;

				case "IServiceHelper":
					helper = ServiceHelper;
					break;


				default:
					throw new NotImplementedException(typeof(T).Name);
			}

			return (T)helper;
		}


		//
		// Helpers
		//
		public static IHelper Helper => _lazyHelper.Value;
		public static IAgentHelper AgentHelper => _lazyAgentHelper.Value;
		public static IEHHelper EventHandlerHelper => _lazyEventHandlerHelper.Value;
		public static IServiceHelper ServiceHelper => _lazyServiceHelper.Value;

		//
		// Lazy Helpers
		//
		private static readonly Lazy<IHelper> _lazyHelper = new Lazy<IHelper>(GetIHelper);
		private static readonly Lazy<IAgentHelper> _lazyAgentHelper = new Lazy<IAgentHelper>(GetIAgentHelper);
		private static readonly Lazy<IEHHelper> _lazyEventHandlerHelper = new Lazy<IEHHelper>(GetIEHHelper);
		private static readonly Lazy<IServiceHelper> _lazyServiceHelper = new Lazy<IServiceHelper>(GetIServiceHelper);

		//
		// Get specific 'Helper'
		//
		private static IHelper GetIHelper()
		{
			// get basic mock
			var mock = BuildMockOfRelativityHelper<IHelper>();

			// return 'IHelper'
			return mock.Object;
		}

		private static IAgentHelper GetIAgentHelper()
		{
			// get basic mock
			var mock = BuildMockOfRelativityHelper<IAgentHelper>();

			// setup
			var authenticationManager = CreateAuthenticationMgrMock();
			mock.Setup(p => p.GetAuthenticationManager()).Returns(authenticationManager);

			// return 'IAgentHelper'
			return mock.Object;
		}

		private static IEHHelper GetIEHHelper()
		{
			// get basic mock
			var mock = BuildMockOfRelativityHelper<IEHHelper>();

			// setup
			var authenticationManager = CreateAuthenticationMgrMock();
			mock.Setup(p => p.GetAuthenticationManager()).Returns(authenticationManager);
			mock.Setup(p => p.GetActiveCaseID()).Returns(WorkspaceID);

			// return 'IEHHelper'
			return mock.Object;
		}

		private static IServiceHelper GetIServiceHelper()
		{
			// get basic mock
			var mock = BuildMockOfRelativityHelper<IServiceHelper>();

			// setup
			var authenticationManager = CreateAuthenticationMgrMock();
			mock.Setup(p => p.GetAuthenticationManager()).Returns(authenticationManager);

			// return 'IServiceHelper'
			return mock.Object;
		}

		private static IAuthenticationMgr CreateAuthenticationMgrMock()
		{
			var userInfo = new Mock<IUserInfo>();
			userInfo.Setup(x => x.ArtifactID).Returns(AdminUserArtifactId);

			var authenticationManager = new Mock<IAuthenticationMgr>();
			authenticationManager.Setup(x => x.UserInfo).Returns(userInfo.Object);

			return authenticationManager.Object;
		}

		//
		// Basic builder for Helpers
		//

		private static Mock<T> BuildMockOfRelativityHelper<T>()
					where T : class, IHelper
		{
			var mockHelper = new Mock<T>();
			var relativityTestHelper = Relativity.Test.Helpers.TestHelper.System();

			if (RunSet)
			{
				mockHelper.Setup(p => p.GetUrlHelper()).Returns(relativityTestHelper.GetUrlHelper);
				mockHelper.Setup(p => p.GetServicesManager()).Returns(relativityTestHelper.GetServicesManager);
				mockHelper.Setup(p => p.GetDBContext(It.IsAny<int>())).Returns<int>(relativityTestHelper.GetDBContext);
			}
			else
			{
				mockHelper.Setup(p => p.GetUrlHelper()).Returns(Mock.Of<IUrlHelper>());
				mockHelper.Setup(p => p.GetServicesManager()).Returns(Mock.Of<IServicesMgr>());
				mockHelper.Setup(p => p.GetDBContext(It.IsAny<int>())).Returns(Mock.Of<IDBContext>());
			}

			mockHelper.Setup(p => p.GetLoggerFactory()).Returns(LogFactoryMock.Build());
			mockHelper.Setup(p => p.GetInstanceSettingBundle()).Returns(InstanceSettingsBundleMock.Build());
			mockHelper.Setup(p => p.GetSecretStore()).Returns(SecretStoreMock.Build());

			return mockHelper;
		}
	}
}

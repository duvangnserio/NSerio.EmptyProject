using Moq;
using NSerio.Utils;
using Relativity.API;
using System.Configuration;

namespace NSerio.EmptyProject.Test.Fake.Mock.Relativity
{
	internal class InstanceSettingsBundleMock : MockBase<InstanceSettingsBundleMock, IInstanceSettingsBundle>
	{
		public override Mock<IInstanceSettingsBundle> BuildMock()
		{
			var mock = CreateMock();

			mock.Setup(s => s.GetStringAsync(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync((string section, string name) =>
				{
					return GetInstanceSettingsValue(section, name);
				});

			mock.Setup(s => s.GetUIntAsync(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync((string section, string name) =>
				{
					var value = GetInstanceSettingsValue(section, name);
					return value.ToUInt();
				});

			return mock;
		}

		private static string GetInstanceSettingsValue(string section, string name)
			=> ConfigurationManager.AppSettings[$"InstanceSettings:{section}:{name}"];
	}
}

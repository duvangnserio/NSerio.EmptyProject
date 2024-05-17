using Moq;
using Relativity.API;

namespace NSerio.EmptyProject.Test.Fake.Mock.Relativity
{
	internal class LogFactoryMock : MockBase<LogFactoryMock, ILogFactory>
	{
		public override Mock<ILogFactory> BuildMock()
		{
			var mock = CreateMock();

			mock.Setup(p => p.GetLogger()).Returns(new DebugOuputLog());

			return mock;
		}
	}
}

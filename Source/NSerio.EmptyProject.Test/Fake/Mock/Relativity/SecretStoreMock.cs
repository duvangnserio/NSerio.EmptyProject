using Moq;
using Relativity.API;
using System.Collections.Generic;

namespace NSerio.EmptyProject.Test.Fake.Mock.Relativity
{
	internal class SecretStoreMock : MockBase<SecretStoreMock, ISecretStore>
	{
		public override Mock<ISecretStore> BuildMock()
		{
			var mock = CreateMock();

			mock.Setup(s => s.GetAsync(It.IsAny<string>()))
				 .ReturnsAsync((string path) => BuildSecretStore(path));

			mock.Setup(s => s.DeleteAsync(It.IsAny<string>()));

			return mock;
		}

		private Secret BuildSecretStore(string path)
		{
			var secret = new Secret { Data = new Dictionary<string, string>() };

			// Add secret to dictionary Data
			// e.g secret.Data.Add("MySecret", "I enjoy Chayanne's songs.");

			return secret;
		}
	}
}

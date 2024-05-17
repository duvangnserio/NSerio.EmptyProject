using System;

namespace NSerio.EmptyProject.Test.Fake
{
	internal abstract class MockBase<T, V> where T : MockBase<T, V>, new() where V : class
	{
		#region [ Attributes ]

		private static readonly Lazy<T> lazy = new Lazy<T>(() => new T());

		#endregion

		#region [ Properties ]

		public static T Instance => lazy.Value;

		#endregion

		#region [ Methods ]

		protected static Moq.Mock<V> CreateMock() => new Moq.Mock<V>();

		public abstract Moq.Mock<V> BuildMock();

		public static V Build() => Instance.BuildMock().Object;

		#endregion
	}
}

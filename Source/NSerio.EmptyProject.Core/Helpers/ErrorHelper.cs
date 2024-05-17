
namespace NSerio.EmptyProject.Core.Helpers
{
	public static class ErrorHelper
	{
		public static string GetErrorSource(this string source)
		{
			return $"{Constants.APPLICATION_NAME} - {source}";
		}
	}
}
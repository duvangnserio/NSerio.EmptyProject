using NSerio.Utils;
using Relativity.API;
using System;

namespace NSerio.EmptyProject.Test
{
	public class DebugOuputLog : IAPILog
	{
		private static readonly object _lock = new object();

		public IAPILog ForContext<T>()
		{
			return new DebugOuputLog();
		}

		public IAPILog ForContext(Type source)
		{
			return this;
		}

		public IAPILog ForContext(string propertyName, object value, bool destructureObjects)
		{
			return this;
		}

		public IDisposable LogContextPushProperty(string propertyName, object obj)
		{
			throw new NotImplementedException();
		}

		public void LogDebug(string messageTemplate, params object[] propertyValues)
		{
			Write("DEBUG", messageTemplate, propertyValues);
		}

		public void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			Write("DEBUG", messageTemplate, propertyValues, exception);
		}

		public void LogError(string messageTemplate, params object[] propertyValues)
		{
			Write("ERROR", messageTemplate, propertyValues);
		}

		public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			Write("ERROR", messageTemplate, propertyValues, exception);
		}

		public void LogFatal(string messageTemplate, params object[] propertyValues)
		{
			Write("FATAL", messageTemplate, propertyValues);
		}

		public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			Write("FATAL", messageTemplate, propertyValues, exception);
		}

		public void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			Write("INFO", messageTemplate, propertyValues);
		}

		public void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			Write("INFO", messageTemplate, propertyValues, exception);
		}

		public void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
			Write("VERBOSE", messageTemplate, propertyValues);
		}

		public void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			Write("VERBOSE", messageTemplate, propertyValues, exception);
		}

		public void LogWarning(string messageTemplate, params object[] propertyValues)
		{
			Write("WARNING", messageTemplate, propertyValues);
		}

		public void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			Write("WARNING", messageTemplate, propertyValues, exception);
		}

		protected virtual void Write(string prefix, string messageTemplate, object[] propertyValues, Exception exception = null)
		{
			lock (_lock)
			{
				try
				{
					prefix = prefix.HasValue() ? $"[{prefix}] " : string.Empty;

					var now = DateTime.Now;

					string
						fileName = $"{now.ToString("yyyyMMdd")}.log",
						propertyValuesText = propertyValues?.Length > 0 ? $"{Environment.NewLine}\tProperty Values: [{string.Join("; ", propertyValues).TrimEnd()}]{Environment.NewLine}" : string.Empty,
						exceptionMessage = exception != null ? $"{(!propertyValuesText.HasValue() ? Environment.NewLine : string.Empty)}\tException Message: {exception}{Environment.NewLine}" : string.Empty,
						content = $"{messageTemplate}{propertyValuesText}{exceptionMessage}",
						text = content.HasValue() ? $"{now.ToString("HH:mm:ss.fff")} {prefix}{content}" : string.Empty;

					System.Diagnostics.Debug.WriteLine(text);
				}
				catch { }
			}
		}
	}
}
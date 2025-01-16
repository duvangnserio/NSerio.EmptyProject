using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Repositories
{
	public class HttpClientRepository
	{
		private Uri _baseUrl;
		private static readonly int[] _STATUS_CODE_RESPONSE_FAILED = { 500, 503, 408, 504, 401, 403, 407 };

		public Task<HttpResponseModel<T>> GetAsync<T>(string resourcePath, Dictionary<string, string>? customHeaders = null)
		{
			return QueryAsync<T>(ApiMethodEnum.Get, resourcePath, customHeaders: customHeaders);
		}

		public Task<HttpResponseModel<T>> PostAsync<T>(string resourcePath, object? bodyContent = null, Dictionary<string, string>? customHeaders = null)
		{
			return QueryAsync<T>(ApiMethodEnum.Post, resourcePath, bodyContent, customHeaders);
		}

		public Task<HttpResponseModel<T>> PutAsync<T>(string resourcePath, object? bodyContent = null, Dictionary<string, string>? customHeaders = null)
		{
			return QueryAsync<T>(ApiMethodEnum.Put, resourcePath, bodyContent, customHeaders);
		}

		public Task<HttpResponseModel<T>> PatchAsync<T>(string resourcePath, object? bodyContent = null, Dictionary<string, string>? customHeaders = null)
		{
			return QueryAsync<T>(ApiMethodEnum.Patch, resourcePath, bodyContent, customHeaders);
		}

		private async Task<HttpResponseModel<T>> QueryAsync<T>(ApiMethodEnum serviceMethod,
			string resourcePath,
			object? bodyContent = null,
			IDictionary<string, string>? customHeaders = null)
		{
			using HttpClient client = CreateHttpClient(customHeaders);
			using HttpResponseMessage response = await DoPetition(client, serviceMethod, resourcePath, bodyContent);
			var result = await GetResponseAsync<T>(response);

			return result;
		}

		private HttpClient CreateHttpClient(IDictionary<string, string>? customHeaders)
		{
			var client = new HttpClient
			{
				Timeout = new TimeSpan(TimeSpan.TicksPerSecond * Constants.Http.TIMEOUT),
				MaxResponseContentBufferSize = Constants.Http.MAX_REQUEST_LENGTH,
				BaseAddress = _baseUrl
			};

			customHeaders
				?.ToList()
				.ForEach(x => client.DefaultRequestHeaders.Add(x.Key, x.Value));
			return client;
		}

		private Task<HttpResponseMessage> DoPetition(HttpClient client, ApiMethodEnum serviceMethod, string resourcePath, object? bodyContent = null)
		{
			var task = serviceMethod switch
			{
				ApiMethodEnum.Get => client.GetAsync(resourcePath),
				ApiMethodEnum.Post => client.PostAsJsonAsync(resourcePath, bodyContent),
				ApiMethodEnum.Put => client.PutAsJsonAsync(resourcePath, bodyContent),
				ApiMethodEnum.Delete => client.DeleteAsync(resourcePath),
				_ => throw new NotImplementedException()
			};

			return task;
		}

		private static async Task<HttpResponseModel<T>> GetResponseAsync<T>(HttpResponseMessage response)
		{
			var result = new HttpResponseModel<T>
			{
				StatusCode = response.StatusCode,
				StatusMessage = response.ReasonPhrase,
				Headers = response.Headers
			};

			string stringContent = await response.Content.ReadAsStringAsync();

			if (_STATUS_CODE_RESPONSE_FAILED.Contains((int)response.StatusCode))
			{
				throw new HttpRequestException(stringContent);
			}

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(stringContent);
			}

			if (typeof(T) == typeof(string))
			{
				result.Data = (T)(object)stringContent;
			}
			else
			{
				result.Data = JsonConvert.DeserializeObject<T>(stringContent);
			}

			return result;
		}

		public void SetBaseUrl(string baseUrl)
		{
			_baseUrl = new Uri(baseUrl);
		}
	}
	public class HttpResponseModel<T>
	{
		public bool IsSuccessStatusCode => Constants.Http.VALID_HTTP_STATUS_CODES.Contains(StatusCode);
		public HttpStatusCode StatusCode { get; set; }
		public string StatusMessage { get; set; }
		public T Data { get; set; }
		public HttpResponseHeaders Headers { get; set; }
	}
	public partial class Constants
	{
		public struct Http
		{
			public static readonly HttpStatusCode[] VALID_HTTP_STATUS_CODES = {
				HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted, HttpStatusCode.NoContent
			};
			public const long MAX_REQUEST_LENGTH = 2147483647;
			public const int TIMEOUT = 600;
		}
	}

	public enum ApiMethodEnum
	{
		Get,
		Post,
		Put,
		Patch,
		Delete
	}
}

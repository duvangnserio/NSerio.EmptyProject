using Newtonsoft.Json;
using Relativity.OAuth2Client.IdentityModel.Client;
using Relativity.OAuth2Client.IdentityModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Oauth2Client.Test
{
	internal class Program
	{
		private static readonly string _OAUTH2_CLIENT_ID = "9140d76d8ad043b5b2bcc74e40f6d9e0";
		private static readonly string _OAUTH2_CLIENT_SECRET = "724db8b57bd7259351dd4a8284e10682513f5051";
		private static readonly string _RELATIVITY_INSTANCE = "https://nserio-us-development.relativity.one/";

		static void Main(string[] args)
		{
			string oAuth2AccessToken = GetOAuth2AccessToken();
			string defaultFileRepository = @"\\files.t013.esus025064.relativity.one\T013\Files\EDDS3663333";
			GetTokenResult tokenResult = GetReadonlyTokenOauthAsync(defaultFileRepository, oAuth2AccessToken)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			Console.WriteLine(tokenResult.SasUrl);
		}

		private static async Task<GetTokenResult> GetReadonlyTokenOauthAsync(string defaultFileRepository, string accessToken)
		{
			string url = "Relativity.Rest/API/Transfer.Service/v1/Transfer/GetReadOnlyToken";

			var request = new GetReadOnlyTokenRequest
			{
				Path = defaultFileRepository
			};

			var payload = new
			{
				request
			};

			var headers = new Dictionary<string, string>()
			{
				{
					"Authorization", "Bearer " + accessToken
				},
				{
					"X-CSRF-Header", "-"
				}
			};

			var httpClientRepository = new HttpClientRepository();
			httpClientRepository.SetBaseUrl(_RELATIVITY_INSTANCE);
			var tokenResult = await httpClientRepository.PostAsync<GetTokenResult>(url, payload, headers);

			return tokenResult.Data;
		}

		private static string GetOAuth2AccessToken()
		{
			using (var clientToken = new TokenClient($"{_RELATIVITY_INSTANCE}/Relativity/Identity/connect/token", _OAUTH2_CLIENT_ID, _OAUTH2_CLIENT_SECRET))
			{
				ITokenResponse token = clientToken.RequestClientCredentialsAsync("UserInfoAccess")
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult();

				if (token.Exception != null)
				{
					throw token.Exception;
				}

				if (token.IsError)
				{
					throw new AuthenticationException($"{token.Error}: {token.ErrorDescription}");
				}

				string accessToken = token.AccessToken;
				return accessToken;
			}
		}
	}

	public class GetReadOnlyTokenRequest : BaseRequest
	{
		public string Path
		{
			get;
			set;
		}

		public GetReadOnlyTokenRequest()
		{
		}
	}

	public class BaseRequest
	{
		public Guid CorrelationID
		{
			get;
			set;
		}

		public BaseRequest()
		{
		}
	}

	public class GetTokenResult
	{
		public Uri SasUrl
		{
			get;
			set;
		}

		public GetTokenResult()
		{
		}
	}

	public class HttpClientRepository
	{
		private Uri _baseUrl;
		private static readonly int[] _STATUS_CODE_RESPONSE_FAILED = { 500, 503, 408, 504, 401, 403, 407 };

		public Task<HttpResponseModel<T>> GetAsync<T>(string resourcePath, Dictionary<string, string> customHeaders = null)
		{
			return QueryAsync<T>(ApiMethodEnum.Get, resourcePath, customHeaders: customHeaders);
		}

		public Task<HttpResponseModel<T>> PostAsync<T>(string resourcePath, object bodyContent = null, Dictionary<string, string> customHeaders = null)
		{
			return QueryAsync<T>(ApiMethodEnum.Post, resourcePath, bodyContent, customHeaders);
		}

		public Task<HttpResponseModel<T>> PutAsync<T>(string resourcePath, object bodyContent = null, Dictionary<string, string> customHeaders = null)
		{
			return QueryAsync<T>(ApiMethodEnum.Put, resourcePath, bodyContent, customHeaders);
		}

		public Task<HttpResponseModel<T>> PatchAsync<T>(string resourcePath, object bodyContent = null, Dictionary<string, string> customHeaders = null)
		{
			return QueryAsync<T>(ApiMethodEnum.Patch, resourcePath, bodyContent, customHeaders);
		}

		private async Task<HttpResponseModel<T>> QueryAsync<T>(ApiMethodEnum serviceMethod,
			string resourcePath,
			object bodyContent = null,
			IDictionary<string, string> customHeaders = null)
		{
			using (HttpClient client = CreateHttpClient(customHeaders))
			{
				using (HttpResponseMessage response = await DoPetition(client, serviceMethod, resourcePath, bodyContent))
				{
					var result = await GetResponseAsync<T>(response);

					return result;
				}
			}
		}

		private HttpClient CreateHttpClient(IDictionary<string, string> customHeaders)
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

		private Task<HttpResponseMessage> DoPetition(HttpClient client, ApiMethodEnum serviceMethod, string resourcePath, object bodyContent = null)
		{
			Task<HttpResponseMessage> task;
			switch (serviceMethod)
			{
				case ApiMethodEnum.Get:
					task = client.GetAsync(resourcePath);
					break;
				case ApiMethodEnum.Post:
					task = client.PostAsJsonAsync(resourcePath, bodyContent);
					break;
				case ApiMethodEnum.Put:
					task = client.PutAsJsonAsync(resourcePath, bodyContent);
					break;
				case ApiMethodEnum.Delete:
					task = client.DeleteAsync(resourcePath);
					break;
				default:
					throw new NotImplementedException();
			}

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

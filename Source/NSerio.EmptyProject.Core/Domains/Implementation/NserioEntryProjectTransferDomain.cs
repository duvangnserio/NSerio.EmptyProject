using NSerio.EmptyProject.Core.RelativityPrivate;
using NSerio.EmptyProject.Core.RelativityPrivate.DataContracts.GetReadonlyToken;
using NSerio.EmptyProject.Core.Repositories.Implementation;
using NSerio.Utils;
using NSerio.Utils.Relativity;
using Relativity.API;
using Relativity.OAuth2Client.IdentityModel.Client;
using Relativity.OAuth2Client.IdentityModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains.Implementation
{
	internal class NserioEntryProjectTransferDomain : ITransferDomain
	{
		private readonly IHelper _helper;
		private readonly ISecretStore _secretStore;
		private readonly IRelativityDBContext _dbContext;


		private static readonly string _OAUTH2_CLIENT_ID = "ec10ef166e69458aa1318294d9923cc4";
		private static readonly string _OAUTH2_CLIENT_SECRET = "97859a3cd6f471d90f5a37078dd5038ac886b16a";
		private static readonly string _RELATIVITY_INSTANCE = "https://nserio.relativity.one/";


		public NserioEntryProjectTransferDomain(IHelper helper, ISecretStore secretStore, IRelativityDBContext dbContext)
		{
			_helper = helper;
			_secretStore = secretStore;
			_dbContext = dbContext;
		}

		public async Task<string> GetReadOnlyTokenAsync(int workspaceId)
		{
			string readOnlyTokenAsyncName = nameof(NserioEntryProjectTransferDomain) + "." + nameof(GetReadOnlyTokenAsync);

			var secretKey = $"Nserio.EmptyProject/tokens/{workspaceId}";
			Secret secret = await GetCredentialsAsync(secretKey);
			string token;

			if (true || secret is null || !secret.Data.HasValues())
			{
				await _helper.AddToRelativityErrorTabAsync(-1, new Exception($"No token found for workspace {workspaceId}"), readOnlyTokenAsyncName);
				string oAuth2AccessToken = GetOAuth2AccessToken();
				string defaultFileRepository = await GetDefaultFileRepository(workspaceId);
				await _helper.AddToRelativityErrorTabAsync(-1, new Exception($"Default file repository for workspace {workspaceId}: {defaultFileRepository}"), readOnlyTokenAsyncName);
				GetTokenResult tokenResult = await GetReadonlyTokenOauthAsync(defaultFileRepository, oAuth2AccessToken);
				token = tokenResult.SasUrl;
				await SetCredentialsAsync(secretKey, workspaceId, token);
			}
			else
			{
				await _helper.AddToRelativityErrorTabAsync(-1, new Exception($"Token found for workspace {workspaceId}"), readOnlyTokenAsyncName);

				var tokenKeyValuePair = secret.Data.FirstOrDefault();
				token = tokenKeyValuePair.Value;
			}

			await _helper.AddToRelativityErrorTabAsync(-1, new Exception($"Token: {token}"), readOnlyTokenAsyncName);
			return token;
		}

		private Task<string> GetDefaultFileRepository(int workspaceId)
		{
			string sql = $@"
							SELECT
								CONCAT([DefaultFileLocationName], 'EDDS', {workspaceId})
							FROM
								[eddsdbo].[ExtendedCase] WITH(NOLOCK)
							WHERE
								[ArtifactID] = {workspaceId}";
			return _dbContext.MasterDB()
				.ExecuteSqlStatementAsScalarAsync<string>(sql);
		}

		private async Task<GetTokenResult> GetReadonlyTokenAsync(string defaultFileRepository)
		{
			GetReadOnlyTokenRequest request = new GetReadOnlyTokenRequest() { Path = defaultFileRepository };
			using var privateMassOperationManager
				= _helper.GetServicesManager().CreateProxy<ITransferController>(ExecutionIdentity.CurrentUser);

			GetTokenResult tokenResult = await privateMassOperationManager.GetReadOnlyTokenAsync(request);

			return tokenResult;
		}

		private async Task<GetTokenResult> GetReadonlyTokenOauthAsync(string defaultFileRepository, string accessToken)
		{
			string url = "Relativity.Rest/API/Transfer.Service/v1/Transfer/GetReadOnlyToken";

			var request = new GetReadOnlyTokenRequest
			{
				Path = defaultFileRepository
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
			var tokenResult = await httpClientRepository.PostAsync<GetTokenResult>(url, request, headers);

			return tokenResult.Data;
		}

		private static HttpClient GetHttpClient(string accessToken)
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(_RELATIVITY_INSTANCE);

			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			client.DefaultRequestHeaders.Add("X-CSRF-Header", "-");
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
			return client;
		}

		private static HttpResponseMessage SendPostRequest(HttpClient client, string url, object request)
		{
			HttpResponseMessage response = client.PostAsJsonAsync(url, request).ToSync();
			return response;
		}

		private static string GetOAuth2AccessToken()
		{
			using var clientToken = new TokenClient($"{_RELATIVITY_INSTANCE}/Relativity/Identity/connect/token", _OAUTH2_CLIENT_ID, _OAUTH2_CLIENT_SECRET);
			ITokenResponse token = clientToken.RequestClientCredentialsAsync("UserInfoAccess").ToSync();
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

		public Task SetCredentialsAsync(string key, int workspaceId, string token)
		{
			var secret = new Secret();
			secret.Add(workspaceId.ToString(), token);
			return _secretStore.SetAsync(key, secret);
		}

		public Task<Secret> GetCredentialsAsync(string path)
		{
			return _secretStore.GetAsync(path);
		}
	}
}
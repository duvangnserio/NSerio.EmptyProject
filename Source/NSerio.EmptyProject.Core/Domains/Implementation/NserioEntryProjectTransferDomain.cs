using NSerio.EmptyProject.Core.RelativityPrivate;
using NSerio.EmptyProject.Core.RelativityPrivate.DataContracts.GetReadonlyToken;
using NSerio.Utils;
using NSerio.Utils.Relativity;
using Relativity.API;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NSerio.EmptyProject.Core.Domains.Implementation
{
	internal class NserioEntryProjectTransferDomain : ITransferDomain
	{
		private readonly IHelper _helper;
		private readonly ISecretStore _secretStore;
		private readonly IRelativityDBContext _dbContext;

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

			if (secret is null || !secret.Data.HasValues())
			{
				await _helper.AddToRelativityErrorTabAsync(-1, new Exception($"No token found for workspace {workspaceId}"), readOnlyTokenAsyncName);
				GetTokenResult tokenResult = await GetReadonlyTokenAsync(workspaceId);
				token = tokenResult.SasUrl.ToString();
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

		private async Task<GetTokenResult> GetReadonlyTokenAsync(int workspaceId)
		{
			string defaultFileRepository = await GetDefaultFileRepository(workspaceId);
			GetReadOnlyTokenRequest request = new GetReadOnlyTokenRequest() { Path = defaultFileRepository };
			using var privateMassOperationManager
				= _helper.GetServicesManager().CreateProxy<ITransferController>(ExecutionIdentity.CurrentUser);

			var tokenResult = await privateMassOperationManager.GetReadOnlyTokenAsync(request);

			return tokenResult;
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
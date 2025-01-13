namespace NSerio.EmptyProject.Core.RelativityPrivate.DataContracts.GetReadonlyToken
{
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
}
using System;

namespace NSerio.EmptyProject.Core.RelativityPrivate.DataContracts.GetReadonlyToken
{
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
}
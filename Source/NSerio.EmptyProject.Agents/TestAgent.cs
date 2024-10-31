using kCura.Agent.CustomAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSerio.Utils;
using NSerio.Utils.Relativity;
using Relativity.API;
using Relativity.Processing.V1.Services;
using Relativity.Processing.V1.Services.Interfaces.DTOs;
using Relativity.Services.Search;
using Relativity.Services.View;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WorkloadDiscovery.CustomAttributes;

namespace NSerio.EmptyProject.Agents
{
	[Name("Test Agent CI Reporting Pivoting")]
	[Guid("4ba10ea2-f92d-453b-a2f3-e52be2d2ca98")]
	[Path(_WORKLOAD_ENPOINT)]
	public class TestAgent : AgentBase
	{
		private const string _WORKLOAD_ENPOINT = "Relativity.REST/api/NSerioEmptyProject/workload-agent/4ba10ea2-f92d-453b-a2f3-e52be2d2ca98/get-workload";


		protected override async Task ExecuteAsync()
		{
			int workspaceId = 3663333;
			GetDiscoveredDocumentsRequest request = new GetDiscoveredDocumentsRequest()
			{
				//Expression =
				//"{\"Type\":\"ConditionalExpression\",\"Property\":\"FileExtension\",\"Constraint\":\"15\",\"Value\":\"European\"}",
				StartingPointOfResult = 0,
			};
			using var proxy = Helper.GetServicesManager().CreateProxy<IProcessingFilterManager>(ExecutionIdentity.CurrentUser);
			//ProcessingFilterData response = await proxy.GetDiscoveredDocumentsAsync(workspaceId, request);

			//string fieldName = nameof(ProcessingFilterResult.FileExtension);

			//var results = response.Results;

			//// Pivot data taking into account the field name using reflection

			//var pivotData = results
			//	.GroupBy(x => x.GetType().GetProperty(fieldName).GetValue(x))
			//	.Select(x => new
			//	{
			//		x.Key,
			//		Count = x.Count()
			//	})
			//	.ToList();

			//StringBuilder messages = new StringBuilder();
			//foreach (var x1 in pivotData)
			//{
			//	messages.AppendLine($"Key: {x1.Key}, Count: {x1.Count}");
			//	RaiseMessageBase(messages.ToString());
			//}

			//var exception = new Exception(messages.ToString());

			//await Helper.AddToRelativityErrorTabAsync(workspaceId, exception, nameof(TestAgent));

			await PivotDiscoveredDocumentsAsync(proxy, workspaceId);
		}

		private async Task PivotDiscoveredDocumentsAsync(IProcessingFilterManager proxy, int workspaceId)
		{
			int viewId = 1050017;
			using var viewManager = Helper.GetServicesManager().CreateProxy<IViewManager>(ExecutionIdentity.CurrentUser);
			var view = await viewManager.ReadSingleAsync(workspaceId, viewId);

			if (view != null)
			{
				IProcessingFilterExpressionModel expression = ConvertCriteriaToProcessingFilter(view.SearchCriteria);

				string expressionJson = JsonConvert.SerializeObject(expression, new StringEnumConverter());
				var exception = new Exception(expressionJson);
				await Helper.AddToRelativityErrorTabAsync(workspaceId, exception, nameof(PivotDiscoveredDocumentsAsync));

				var request = new GetDiscoveredDocumentsWithPivotOnRequest()
				{
					Expression = expressionJson,
					PivotOnOption = new PivotOnOption()
					{
						GroupByProperty = Property.FileExtension,
					}
				};
				var result = await proxy.PivotOnDiscoveredDocumentsAsync(workspaceId, request);

				var resultAsJson = result.ToJSON();

				exception = new Exception(resultAsJson);
				await Helper.AddToRelativityErrorTabAsync(workspaceId, exception, nameof(PivotDiscoveredDocumentsAsync));
				RaiseMessageBase(resultAsJson);
			}

		}

		private IProcessingFilterExpressionModel? MapCriteriaToFilter(Criteria criteria)
		{
			var property = GetProperty(criteria.Condition.FieldIdentifier.Name);
			ProcessingFilterConditionalExpression filterExpression = null;

			if (property != null)
			{
				switch (criteria.Condition)
				{
					case CriteriaCondition criteriaCondition:
						ProcessingFilterConstraint filterConstraint = GetConstraint(criteriaCondition.Operator);
						filterExpression = new ProcessingFilterConditionalExpression
						{
							Property = property.Value,
							Constraint = criteriaCondition.NotOperator ? GetNegateConstraint(filterConstraint) : filterConstraint,
							Value = criteriaCondition.Value?.ToJSON()
						};
						break;

					case CriteriaDateCondition criteriaDateCondition:
						ProcessingFilterConstraint processingFilterConstraint = GetDateConstraint(criteriaDateCondition.Operator);
						filterExpression = new ProcessingFilterConditionalExpression
						{
							Property = property.Value,
							Constraint = criteriaDateCondition.NotOperator ? GetNegateConstraint(processingFilterConstraint) : processingFilterConstraint,
							Value = GetDateValue(criteriaDateCondition)
						};
						break;

					default:
						throw new ArgumentException("Unknown criteria type", nameof(criteria));
				}
			}

			return filterExpression;
		}

		private string GetDateValue(CriteriaDateCondition criteriaDateCondition)
		{
			string value = string.Empty;
			if (criteriaDateCondition.Month != Month.NotSet)
			{
				value = new[] { criteriaDateCondition.Month.ToString() }.ToJSON(indented: false, useCamelCase: false);
			}
			else
			{
				if (criteriaDateCondition.Value.ToString() == "ThisMonth")
				{
					//An array of two dates where the first datetime is the start in hours and minutes zero of the month and the second datetime is the end of the month
					DateTime startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
					DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

					value = new[] { startOfMonth.ToString("yyyy-MM-ddTHH:mm:ss"), endOfMonth.ToString("yyyy-MM-ddTHH:mm:ss") }.ToJSON(indented: false, useCamelCase: false);

				}
				else if (criteriaDateCondition.Value.ToString() == "ThisWeek")
				{
					// An array of two dates where the first date is the start of the week with time zero and the second date is the end of the week
					DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)DateTime.Today.DayOfWeek);
					DateTime endOfWeek = startOfWeek.AddDays(6);
					value = new[] { startOfWeek.ToString("yyyy-MM-ddTHH:mm:ss"), endOfWeek.ToString("yyyy-MM-ddTHH:mm:ss") }.ToJSON(indented: false, useCamelCase: false);
				}
				else if (criteriaDateCondition.Value.ToString() == "LastWeek")
				{
					// An array of two dates where the first date is the start of the week and the second date is the end of the week
					DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)DateTime.Today.DayOfWeek - 7);
					DateTime endOfWeek = startOfWeek.AddDays(6);

					value = new[] { startOfWeek.ToString("yyyy-MM-ddTHH:mm:ss"), endOfWeek.ToString("yyyy-MM-ddTHH:mm:ss") }.ToJSON(indented: false, useCamelCase: false);
				}
				else if (criteriaDateCondition.Value.ToString() == "NextWeek")
				{
					// An array of two dates where the first date is the start of the week and the second date is the end of the week
					DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)DateTime.Today.DayOfWeek + 7);
					DateTime endOfWeek = startOfWeek.AddDays(6);

					value = new[] { startOfWeek.ToString("yyyy-MM-ddTHH:mm:ss"), endOfWeek.ToString("yyyy-MM-ddTHH:mm:ss") }.ToJSON(indented: false, useCamelCase: false);
				}
				else if (criteriaDateCondition.Value.ToString() == "Last7Days")
				{
					// An array of two dates where the first date is the start of the week and the second date is the end of the week with time
					DateTime startOfWeek = DateTime.Today.AddDays(-7);
					DateTime endOfWeek = DateTime.Today;

					value = new[] { startOfWeek.ToString("yyyy-MM-ddTHH:mm:ss"), endOfWeek.ToString("yyyy-MM-ddTHH:mm:ss") }.ToJSON(indented: false, useCamelCase: false);
				}
				else if (criteriaDateCondition.Value.ToString() == "Last30Days")
				{
					// An array of two dates where the first date is the start of the week and the second date is the end of the week
					DateTime startOfWeek = DateTime.Today.AddDays(-30);
					DateTime endOfWeek = DateTime.Today;

					value = new[] { startOfWeek.ToString("yyyy-MM-ddTHH:mm:ss"), endOfWeek.ToString("yyyy-MM-ddTHH:mm:ss") }.ToJSON(indented: false, useCamelCase: false);
				}
				else
				{
					value = new[] { criteriaDateCondition.Value.ToString() }.ToJSON(indented: false, useCamelCase: false);
				}
			}

			return value;
		}

		private ProcessingFilterConstraint GetNegateConstraint(ProcessingFilterConstraint constraint)
		{
			return constraint switch
			{
				ProcessingFilterConstraint.Is => ProcessingFilterConstraint.IsNot,
				ProcessingFilterConstraint.IsIn => ProcessingFilterConstraint.IsNotIn,
				ProcessingFilterConstraint.IsSet => ProcessingFilterConstraint.IsNotSet,
				ProcessingFilterConstraint.IsLike => ProcessingFilterConstraint.IsNotLike,
				ProcessingFilterConstraint.BeginsWith => ProcessingFilterConstraint.DoesNotBeginWith,
				ProcessingFilterConstraint.EndsWith => ProcessingFilterConstraint.DoesNotEndWith,
				ProcessingFilterConstraint.Between => ProcessingFilterConstraint.NotBetween,
				_ => throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null)
			};
		}

		// Helper method to map CriteriaDateConditionEnum to ProcessingFilterConstraintEnum or any relevant constraint enum
		private ProcessingFilterConstraint GetDateConstraint(CriteriaDateConditionEnum dateOperator)
		{
			return dateOperator switch
			{
				CriteriaDateConditionEnum.Between => ProcessingFilterConstraint.Between,
				CriteriaDateConditionEnum.In => ProcessingFilterConstraint.Between,
				CriteriaDateConditionEnum.Is => ProcessingFilterConstraint.Is,
				CriteriaDateConditionEnum.IsAfter => ProcessingFilterConstraint.IsAfter,
				CriteriaDateConditionEnum.IsAfterOrOn => ProcessingFilterConstraint.IsAfterOrOn,
				CriteriaDateConditionEnum.IsBefore => ProcessingFilterConstraint.IsBefore,
				CriteriaDateConditionEnum.IsBeforeOrOn => ProcessingFilterConstraint.IsBeforeOrOn,
				CriteriaDateConditionEnum.IsSet => ProcessingFilterConstraint.IsSet,
				_ => throw new ArgumentOutOfRangeException(nameof(dateOperator), $"Unknown operator: {dateOperator}")
			};
		}

		private ProcessingFilterCompositeExpression MapCriteriaCollectionToComposite(CriteriaCollection criteriaCollection)
		{
			var compositeExpression = new ProcessingFilterCompositeExpression
			{
				Operator = criteriaCollection.BooleanOperator == BooleanOperatorEnum.And || criteriaCollection.BooleanOperator == BooleanOperatorEnum.None
					? ProcessingFilterOperator.And
					: ProcessingFilterOperator.Or,
				Expressions = criteriaCollection.Conditions
					.Select(c => c is Criteria criteria
						? MapCriteriaToFilter(criteria)
						: MapCriteriaCollectionToComposite((CriteriaCollection)c)) // Recursive for nested collections
					.Where(t => t != null)
					.ToArray()
			};

			return compositeExpression;
		}

		public IProcessingFilterExpressionModel ConvertCriteriaToProcessingFilter(CriteriaBase criteriaBase)
		{
			if (criteriaBase is CriteriaCollection criteriaCollection)
			{
				return MapCriteriaCollectionToComposite(criteriaCollection);
			}
			else if (criteriaBase is Criteria criteria)
			{
				return MapCriteriaToFilter(criteria);
			}

			throw new ArgumentException("Unknown criteria type", nameof(criteriaBase));
		}


		public Property? GetProperty(string name)
		{
			return name switch
			{
				"Container ID" => Property.ContainerID,
				"Container Name" => Property.ContainerName,
				"Custodian" => Property.CustodianArtifactId,
				"File Extension - Text" => Property.FileExtension,
				"Container Extension" => Property.ContainerExtension,
				"Data Source" => Property.DataSourceArtifactId,
				"Dedupe Status" => Property.DedupeStatus,
				"Discovery Group ID" => Property.DiscoverGroupId,
				"Exception Category" => Property.ErrorCategory,
				"Exception Message" => Property.ErrorMessage,
				"Exception Phase" => Property.ErrorPhase,
				"Exception Status" => Property.ErrorStatus,
				"Extracted Text Location" => Property.ExtractedTextLocation,
				"File ID" => Property.ProcessingFileId,
				"File Name" => Property.FileName,
				"File Size (KB)" => Property.FileSize,
				"File Type" => Property.FileType,
				"Folder Path" => Property.FolderPath,
				"Import Source" => Property.ImportSource,
				"Is Container?" => Property.IsContainer,
				"Is Embedded?" => Property.IsEmbedded,
				"Is Published?" => Property.IsPublished,
				"Logical Path" => Property.LogicalPath,
				"MD5 Hash" => Property.MD5Hash,
				"Original Path" => Property.OriginalPath,
				"Parent File ID" => Property.ParentDocumentId,
				"Processing Deletion?" => Property.IsDeleted,
				"Sender Domain - Text" => Property.SenderDomain,
				"SHA-256 Hash" => Property.SHA256Hash,
				"SHA1 Hash" => Property.SHA1Hash,
				"Sort Date" => Property.SortDate,
				"Storage ID" => Property.StorageId,
				"Text Extraction Method" => Property.TextExtractionMethod,
				"Unprocessable" => Property.Unprocessable,
				"Virtual Path - Text" => Property.VirtualPath,
				_ => null
			};
		}


		private ProcessingFilterConstraint GetConstraint(CriteriaConditionEnum criteriaConditionOperator)
		{
			return criteriaConditionOperator switch
			{
				//case CriteriaConditionEnum.AllOfThese Not Mapped since is not available in condition builder of view In Relativity
				CriteriaConditionEnum.AnyOfThese => ProcessingFilterConstraint.IsIn,
				CriteriaConditionEnum.IsLike => ProcessingFilterConstraint.IsLike,
				//case CriteriaConditionEnum.Contains: Does not appear in Condition builder
				CriteriaConditionEnum.EndsWith => ProcessingFilterConstraint.EndsWith,
				CriteriaConditionEnum.GreaterThan => ProcessingFilterConstraint.IsGreaterThan,
				CriteriaConditionEnum.GreaterThanOrEqualTo => ProcessingFilterConstraint.IsGreaterThanOrEqualTo,
				CriteriaConditionEnum.In =>
					ProcessingFilterConstraint
						.Between // Not sure about this mapping, the view returns for example ThisMonth, but the processing filter is with between with an array
				,
				CriteriaConditionEnum.Is => ProcessingFilterConstraint.Is,
				CriteriaConditionEnum.IsSet => ProcessingFilterConstraint.IsSet,
				CriteriaConditionEnum.LessThan => ProcessingFilterConstraint.IsLessThan,
				CriteriaConditionEnum.LessThanOrEqualTo => ProcessingFilterConstraint.IsLessThanOrEqualTo,
				CriteriaConditionEnum.StartsWith => ProcessingFilterConstraint.BeginsWith,
				//CriteriaConditionEnum.Unknown => expr,
				//CriteriaConditionEnum.AllOfThese => expr,
				//CriteriaConditionEnum.Contains => expr,
				//CriteriaConditionEnum.IsLoggedInUser => expr,
				//CriteriaConditionEnum.LuceneSearch => expr,
				_ => throw new ArgumentOutOfRangeException(nameof(criteriaConditionOperator), criteriaConditionOperator, null)
			};
		}
	}
}
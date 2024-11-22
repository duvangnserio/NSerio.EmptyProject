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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WorkloadDiscovery.CustomAttributes;

namespace NSerio.EmptyProject.Agents
{
	// Core interfaces for better abstraction and dependency injection
	public interface IDateRangeProvider
	{
		(DateTime start, DateTime end) GetDateRange();
	}

	public interface IProcessingFilterMapper
	{
		IProcessingFilterExpressionModel MapToFilter(CriteriaBase criteria);
	}

	// Date range providers implementing the strategy pattern
	public abstract class BaseDateRangeProvider : IDateRangeProvider
	{
		protected const int LastMomentHour = 23;
		protected const int LastMomentMinute = 59;
		protected const int LastMomentSecond = 59;
		protected const int LastMomentMillisecond = 999;

		protected static DateTime EndOfDay(DateTime date) =>
			date.Date.AddHours(LastMomentHour)
					 .AddMinutes(LastMomentMinute)
					 .AddSeconds(LastMomentSecond)
					 .AddMilliseconds(LastMomentMillisecond);

		public abstract (DateTime start, DateTime end) GetDateRange();
	}

	public class ThisMonthRangeProvider : BaseDateRangeProvider
	{
		public override (DateTime start, DateTime end) GetDateRange()
		{
			DateTime today = DateTime.Today;
			DateTime start = new DateTime(today.Year, today.Month, 1);
			DateTime end = EndOfDay(start.AddMonths(1).AddDays(-1));
			return (start, end);
		}
	}

	// Additional date range providers
	public class Last7DaysRangeProvider : BaseDateRangeProvider
	{
		private const int DaysInWeek = 7;

		public override (DateTime start, DateTime end) GetDateRange()
		{
			DateTime today = DateTime.Today;
			return (today.AddDays(-DaysInWeek), EndOfDay(today));
		}
	}

	public class Last30DaysRangeProvider : BaseDateRangeProvider
	{
		private const int DaysInMonth = 30;

		public override (DateTime start, DateTime end) GetDateRange()
		{
			DateTime today = DateTime.Today;
			return (today.AddDays(-DaysInMonth), EndOfDay(today));
		}
	}

	public abstract class WeekRangeProvider : BaseDateRangeProvider
	{
		protected const int DaysInAWeek = 7;

		protected static DateTime StartOfCurrentWeek()
		{
			DateTime today = DateTime.Today;
			int daysSinceMonday = today.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)today.DayOfWeek - 1;
			return today.AddDays(-daysSinceMonday).Date;
		}

		public abstract override (DateTime start, DateTime end) GetDateRange();
	}

	public class NextWeekRangeProvider : WeekRangeProvider
	{
		public override (DateTime start, DateTime end) GetDateRange()
		{
			DateTime startOfWeek = StartOfCurrentWeek();
			DateTime start = startOfWeek.AddDays(DaysInAWeek);
			DateTime end = EndOfDay(start.AddDays(DaysInAWeek - 1));
			return (start, end);
		}
	}

	public class CurrentWeekRangeProvider : WeekRangeProvider
	{
		public override (DateTime start, DateTime end) GetDateRange()
		{
			DateTime startOfWeek = StartOfCurrentWeek();
			DateTime endOfWeek = EndOfDay(startOfWeek.AddDays(6));
			return (startOfWeek, endOfWeek);
		}
	}

	public class LastWeekRangeProvider : WeekRangeProvider
	{
		public override (DateTime start, DateTime end) GetDateRange()
		{
			DateTime startOfCurrentWeek = StartOfCurrentWeek();
			DateTime start = startOfCurrentWeek.AddDays(-DaysInAWeek);
			DateTime end = EndOfDay(start.AddDays(DaysInAWeek - 1));
			return (start, end);
		}
	}

	public interface IPropertyMapper
	{
		Property? GetProperty(string name);
	}

	public interface IConstraintMapper
	{
		ProcessingFilterConstraint GetConstraint(CriteriaConditionEnum criteriaConditionOperator);
		ProcessingFilterConstraint GetDateConstraint(CriteriaDateConditionEnum dateOperator);
		ProcessingFilterConstraint GetNegateConstraint(ProcessingFilterConstraint constraint);
	}

	public interface IDateRangeFactory
	{
		IDateRangeProvider CreateProvider(string dateRangeType);
	}

	public interface IServiceManagerFactory
	{
		T CreateProxy<T>(ExecutionIdentity identity) where T : class;
	}

	// Implementation of PropertyMapper
	public class PropertyMapper : IPropertyMapper
	{
		private static readonly Dictionary<string, Property> PropertyMap = new Dictionary<string, Property>
		{
			["Container ID"] = Property.ContainerID,
			["Container Name"] = Property.ContainerName,
			["Custodian"] = Property.CustodianArtifactId,
			["File Extension - Text"] = Property.FileExtension,
			["Container Extension"] = Property.ContainerExtension,
			["Data Source"] = Property.DataSourceArtifactId,
			["Dedupe Status"] = Property.DedupeStatus,
			["Discovery Group ID"] = Property.DiscoverGroupId,
			["Exception Category"] = Property.ErrorCategory,
			["Exception Message"] = Property.ErrorMessage,
			["Exception Phase"] = Property.ErrorPhase,
			["Exception Status"] = Property.ErrorStatus,
			["Extracted Text Location"] = Property.ExtractedTextLocation,
			["File ID"] = Property.ProcessingFileId,
			["File Name"] = Property.FileName,
			["File Size (KB)"] = Property.FileSize,
			["File Type"] = Property.FileType,
			["Folder Path"] = Property.FolderPath,
			["Import Source"] = Property.ImportSource,
			["Is Container?"] = Property.IsContainer,
			["Is Embedded?"] = Property.IsEmbedded,
			["Is Published?"] = Property.IsPublished,
			["Logical Path"] = Property.LogicalPath,
			["MD5 Hash"] = Property.MD5Hash,
			["Original Path"] = Property.OriginalPath,
			["Parent File ID"] = Property.ParentDocumentId,
			["Processing Deletion?"] = Property.IsDeleted,
			["Sender Domain - Text"] = Property.SenderDomain,
			["SHA-256 Hash"] = Property.SHA256Hash,
			["SHA1 Hash"] = Property.SHA1Hash,
			["Sort Date"] = Property.SortDate,
			["Storage ID"] = Property.StorageId,
			["Text Extraction Method"] = Property.TextExtractionMethod,
			["Unprocessable"] = Property.Unprocessable,
			["Virtual Path - Text"] = Property.VirtualPath
		};

		public Property? GetProperty(string name)
		{
			return PropertyMap.TryGetValue(name, out Property property) ? property : (Property?)null;
		}
	}


	public class ConstraintMapper : IConstraintMapper
	{
		public ProcessingFilterConstraint GetConstraint(CriteriaConditionEnum criteriaConditionOperator)
		{
			return criteriaConditionOperator switch
			{
				CriteriaConditionEnum.AnyOfThese => ProcessingFilterConstraint.IsIn,
				CriteriaConditionEnum.IsLike => ProcessingFilterConstraint.IsLike,
				CriteriaConditionEnum.EndsWith => ProcessingFilterConstraint.EndsWith,
				CriteriaConditionEnum.GreaterThan => ProcessingFilterConstraint.IsGreaterThan,
				CriteriaConditionEnum.GreaterThanOrEqualTo => ProcessingFilterConstraint.IsGreaterThanOrEqualTo,
				CriteriaConditionEnum.In => ProcessingFilterConstraint.Between,
				CriteriaConditionEnum.Is => ProcessingFilterConstraint.Is,
				CriteriaConditionEnum.IsSet => ProcessingFilterConstraint.IsSet,
				CriteriaConditionEnum.LessThan => ProcessingFilterConstraint.IsLessThan,
				CriteriaConditionEnum.LessThanOrEqualTo => ProcessingFilterConstraint.IsLessThanOrEqualTo,
				CriteriaConditionEnum.StartsWith => ProcessingFilterConstraint.BeginsWith,
				_ => throw new ArgumentOutOfRangeException(nameof(criteriaConditionOperator))
			};
		}

		public ProcessingFilterConstraint GetDateConstraint(CriteriaDateConditionEnum dateOperator)
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
				_ => throw new ArgumentOutOfRangeException(nameof(dateOperator))
			};
		}

		public ProcessingFilterConstraint GetNegateConstraint(ProcessingFilterConstraint constraint)
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
				_ => throw new ArgumentOutOfRangeException(nameof(constraint))
			};
		}
	}

	public class DateRangeFactory : IDateRangeFactory
	{
		private readonly IServiceProvider _serviceProvider;

		public DateRangeFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public IDateRangeProvider CreateProvider(string dateRangeType)
		{
			return dateRangeType switch
			{
				"ThisMonth" => new ThisMonthRangeProvider(),
				"LastWeek" => new LastWeekRangeProvider(),
				"ThisWeek" => new CurrentWeekRangeProvider(),
				"NextWeek" => new NextWeekRangeProvider(),
				"Last7Days" => new Last7DaysRangeProvider(),
				"Last30Days" => new Last30DaysRangeProvider(),
				_ => throw new ArgumentException($"Unknown date range type: {dateRangeType}")
			};
		}
	}

	// Processing filter mapping implementation
	public class ProcessingFilterMapper : IProcessingFilterMapper
	{
		private readonly IPropertyMapper _propertyMapper;
		private readonly IConstraintMapper _constraintMapper;
		private readonly IDateRangeFactory _dateRangeFactory;

		public ProcessingFilterMapper(
			IPropertyMapper propertyMapper,
			IConstraintMapper constraintMapper,
			IDateRangeFactory dateRangeFactory)
		{
			_propertyMapper = propertyMapper;
			_constraintMapper = constraintMapper;
			_dateRangeFactory = dateRangeFactory;
		}

		public IProcessingFilterExpressionModel MapToFilter(CriteriaBase criteriaBase)
		{
			return criteriaBase switch
			{
				CriteriaCollection collection => MapCriteriaCollectionToComposite(collection),
				Criteria criteria => MapCriteriaToFilter(criteria),
				_ => throw new ArgumentException("Unknown criteria type", nameof(criteriaBase))
			};
		}

		private IProcessingFilterExpressionModel? MapCriteriaToFilter(Criteria criteria)
		{
			var property = _propertyMapper.GetProperty(criteria.Condition.FieldIdentifier.Name);
			if (property == null) return null;

			return criteria.Condition switch
			{
				CriteriaCondition condition => CreateConditionalExpression(condition, property.Value),
				CriteriaDateCondition dateCondition => CreateDateExpression(dateCondition, property.Value),
				_ => throw new ArgumentException("Unknown criteria condition type", nameof(criteria))
			};
		}

		private ProcessingFilterCompositeExpression MapCriteriaCollectionToComposite(CriteriaCollection collection)
		{
			return new ProcessingFilterCompositeExpression
			{
				Operator = collection.BooleanOperator == BooleanOperatorEnum.And ||
						  collection.BooleanOperator == BooleanOperatorEnum.None
					? ProcessingFilterOperator.And
					: ProcessingFilterOperator.Or,
				Expressions = collection.Conditions
					.Select(MapToFilter)
					.Where(t => t != null)
					.ToArray()
			};
		}

		private const string FullFormatDate = "yyyy-MM-ddTHH:mm:ss.fffZ";

		private ProcessingFilterConditionalExpression CreateConditionalExpression(
			CriteriaCondition condition,
			Property property)
		{
			var filterConstraint = _constraintMapper.GetConstraint(condition.Operator);
			return new ProcessingFilterConditionalExpression
			{
				Property = property,
				Constraint = condition.NotOperator
					? _constraintMapper.GetNegateConstraint(filterConstraint)
					: filterConstraint,
				Value = condition.Value?.ToJSON()
			};
		}

		private ProcessingFilterConditionalExpression CreateDateExpression(
			CriteriaDateCondition dateCondition,
			Property property)
		{
			var filterConstraint = _constraintMapper.GetDateConstraint(dateCondition.Operator);
			return new ProcessingFilterConditionalExpression
			{
				Property = property,
				Constraint = dateCondition.NotOperator
					? _constraintMapper.GetNegateConstraint(filterConstraint)
					: filterConstraint,
				Value = GetDateValue(dateCondition)
			};
		}

		private string GetDateValue(CriteriaDateCondition dateCondition)
		{
			if (dateCondition.Month != Month.NotSet)
			{
				return new[] { dateCondition.Month.ToString() }
					.ToJSON(indented: false, useCamelCase: false);
			}

			var dateValue = dateCondition.Value.ToString();
			if (IsPresetDateRange(dateValue))
			{
				var provider = _dateRangeFactory.CreateProvider(dateValue);
				var (start, end) = provider.GetDateRange();
				return new[] { start.ToString(FullFormatDate), end.ToString(FullFormatDate) }
					.ToJSON(indented: false, useCamelCase: false);
			}

			return new[] { dateValue }.ToJSON(indented: false, useCamelCase: false);
		}

		private static bool IsPresetDateRange(string value)
		{
			return value == "ThisMonth"
				   || value == "ThisWeek"
				   || value == "LastWeek"
				   || value == "NextWeek"
				   || value == "Last7Days"
				   || value == "Last30Days";
		}
	}

	// Refactored TestAgent
	[Name("Test Agent CI Reporting Pivoting")]
	[Guid("4ba10ea2-f92d-453b-a2f3-e52be2d2ca98")]
	[Path(WorkloadEndpoint)]

	public class TestAgent : AgentBase
	{
		private const string WorkloadEndpoint = "Relativity.REST/api/NSerioEmptyProject/workload-agent/4ba10ea2-f92d-453b-a2f3-e52be2d2ca98/get-workload";
		private readonly IProcessingFilterMapper _filterMapper;
		private readonly IServiceManagerFactory _serviceManagerFactory;

		public TestAgent(
			IProcessingFilterMapper filterMapper,
			IServiceManagerFactory serviceManagerFactory)
		{
			_filterMapper = filterMapper;
			_serviceManagerFactory = serviceManagerFactory;
		}

		protected override async Task ExecuteAsync()
		{
			int workspaceId = 3663333;
			using var proxy = _serviceManagerFactory
				.CreateProxy<IProcessingFilterManager>(ExecutionIdentity.CurrentUser);

			await PivotDiscoveredDocumentsAsync(proxy, workspaceId);
		}

		private async Task PivotDiscoveredDocumentsAsync(
			IProcessingFilterManager proxy,
			int workspaceId)
		{
			const int viewId = 1050017;
			using var viewManager = _serviceManagerFactory
				.CreateProxy<IViewManager>(ExecutionIdentity.CurrentUser);

			var view = await viewManager.ReadSingleAsync(workspaceId, viewId);
			if (view == null) return;

			var expression = _filterMapper.MapToFilter(view.SearchCriteria);
			string expressionJson = JsonConvert.SerializeObject(
				expression,
				new StringEnumConverter());

			await LogExpressionAsync(workspaceId, expressionJson);

			var result = await GetPivotResultsAsync(proxy, workspaceId, expressionJson);
			await LogResultAsync(workspaceId, result);
		}

		private async Task<string> GetPivotResultsAsync(
			IProcessingFilterManager proxy,
			int workspaceId,
			string expressionJson)
		{
			var request = new GetDiscoveredDocumentsWithPivotOnRequest
			{
				Expression = expressionJson,
				PivotOnOption = new PivotOnOption
				{
					GroupByProperty = Property.FileExtension,
				}
			};

			var result = await proxy.PivotOnDiscoveredDocumentsAsync(workspaceId, request);
			return result.ToJSON();
		}

		private async Task LogExpressionAsync(int workspaceId, string expressionJson)
		{
			var exception = new Exception(expressionJson);
			await Helper.AddToRelativityErrorTabAsync(
				workspaceId,
				exception,
				nameof(PivotDiscoveredDocumentsAsync));
		}

		private async Task LogResultAsync(int workspaceId, string resultJson)
		{
			var exception = new Exception(resultJson);
			await Helper.AddToRelativityErrorTabAsync(
				workspaceId,
				exception,
				nameof(PivotDiscoveredDocumentsAsync));

			RaiseMessageBase(resultJson);
		}
	}
}
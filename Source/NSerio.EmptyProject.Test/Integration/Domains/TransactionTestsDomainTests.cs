using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSerio.EmptyProject.Core.Domains;
using NSerio.Utils;
using Relativity.API;
using Relativity.Processing.V1.Services.Interfaces.DTOs;
using Relativity.Services.Search;
using Relativity.Services.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NSerio.EmptyProject.Test.Integration.Domains
{
	public class TransactionTestsDomainTests : IntegrationTestBase<IHelper>
	{
		private static readonly int _VIEW_ID = int.Parse(ConfigurationManager.AppSettings["ViewId"] ?? "-1");

		[Fact]
		public async Task ExecuteAutoLinkerJobAsync_Success_TestAsync()
		{
			await ExecuteDomainServiceAsync<ITransactionsTestDomain>(t => t.CreateJobWithTransactionsAsync());
		}

		public TransactionTestsDomainTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public async Task TranslateViewCriteriaToInvariantExpressionAsync()
		{
			using (var viewManager = Helper.GetServicesManager().CreateProxy<IViewManager>(ExecutionIdentity.CurrentUser))
			{
				var view = await viewManager.ReadSingleAsync(WORKSPACE_ID, _VIEW_ID);
				if (view != null)
				{
					var invariantExpression = new ExpressionProcessor().InvariantExpressionFromViewCriteria(view.SearchCriteria);

					if (invariantExpression != null)
					{
						TraceInformation(invariantExpression.ToJSON(true));
						string expressionJson = JsonConvert.SerializeObject(invariantExpression, new StringEnumConverter());

					}
				}
			}
		}
	}

	// Main processing class
	public class ExpressionProcessor
	{
		public IProcessingFilterExpressionModel InvariantExpressionFromViewCriteria(CriteriaBase criteria)
		{
			if (criteria == null) return null;

			if (criteria is CriteriaCollection criteriaCollection)
			{
				if (criteriaCollection.Conditions.Count > 1)
				{

					var expressions = new List<IProcessingFilterExpressionModel>();
					var currentOperator = BooleanOperatorEnum.None;
					List<IProcessingFilterExpressionModel> tempExpressions = new List<IProcessingFilterExpressionModel>();

					foreach (CriteriaBase condition in criteriaCollection.Conditions)
					{
						switch (condition)
						{
							case CriteriaCollection conditionCriteriaCollection when conditionCriteriaCollection.BooleanOperator != currentOperator &&
																					 conditionCriteriaCollection.BooleanOperator != BooleanOperatorEnum.None:
								{
									if (tempExpressions.Any())
									{
										expressions.Add(CreateCompositeExpression(currentOperator, tempExpressions));
									}

									tempExpressions = new List<IProcessingFilterExpressionModel>();
									currentOperator = conditionCriteriaCollection.BooleanOperator;
									break;
								}
							case Criteria conditionCriteria when conditionCriteria.BooleanOperator != currentOperator && conditionCriteria.BooleanOperator != BooleanOperatorEnum.None:
								{
									if (tempExpressions.Any())
									{
										expressions.Add(CreateCompositeExpression(currentOperator, tempExpressions));
									}

									tempExpressions = new List<IProcessingFilterExpressionModel>();
									currentOperator = conditionCriteria.BooleanOperator;
									break;
								}
						}

						var expr = InvariantExpressionFromViewCriteria(condition);
						if (expr != null)
						{
							tempExpressions.Add(expr);
						}
					}

					if (expressions.Any())
					{
						if (tempExpressions.Any())
						{
							expressions.Add(CreateCompositeExpression(currentOperator, tempExpressions));
						}
						return CreateCompositeExpression(BooleanOperatorEnum.Or, expressions);
					}
					else
					{
						return CreateCompositeExpression(currentOperator, tempExpressions);
					}

				}
				else if (criteriaCollection.Conditions.Count == 1)
				{
					return InvariantExpressionFromViewCriteria(criteriaCollection.Conditions[0]);

				}
			}
			else if (criteria is Criteria criteriaCondition)
			{
				return criteriaCondition.Condition switch
				{
					CriteriaCondition criteriaGenericCondition => CreateExpression(criteriaGenericCondition),
					//CriteriaDateCondition criteriaDateCondition => CreateExpression(criteriaDateCondition),
					_ => null
				};
			}

			return null;
		}

		private IProcessingFilterExpressionModel CreateCompositeExpression(BooleanOperatorEnum op, List<IProcessingFilterExpressionModel> expressions)
		{
			// Clean up conditions to remove any null values
			var cleanedExpressions = CleanConditions(expressions);

			// Check if more than one expression is provided
			if (cleanedExpressions.Length > 1)
			{
				var compositeExpression = new ProcessingFilterCompositeExpression
				{
					Expressions = cleanedExpressions,
					Operator = op == BooleanOperatorEnum.And
						? ProcessingFilterOperator.And
						: ProcessingFilterOperator.Or

				};

				return compositeExpression;
			}
			else if (cleanedExpressions.Length == 1)
			{
				// If only one expression remains after cleaning, return it directly
				return cleanedExpressions[0];
			}

			// If there are no expressions, return null or additional expression if provided
			return null;
		}

		private IProcessingFilterExpressionModel CreateExpression(CriteriaCondition criteria)
		{
			ProcessingFilterConstraint filterConstraint = ConstraintMapper.GetConstraint(criteria.Operator);
			var property = ConstraintMapper.GetProperty(criteria.FieldIdentifier.Name);
			if (property == null)
			{
				return null;
			}
			var expr = new ProcessingFilterConditionalExpression
			{
				Property = property.Value,
				Constraint = criteria.NotOperator
				? ConstraintMapper.GetNegateConstraint(filterConstraint)
				: filterConstraint,
				Value = criteria.Value.ToJSON()
			};
			return expr;
		}


		private IProcessingFilterExpressionModel[] CleanConditions(List<IProcessingFilterExpressionModel> expressions)
		{
			return expressions.Where(e => e != null).ToArray();
		}
	}

	public class ConstraintMapper
	{
		public static Property? GetProperty(string name)
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
				"Error Status" => Property.ErrorStatus,
				"Error Message" => Property.ErrorMessage,
				_ => null
			};
		}

		public static ProcessingFilterConstraint GetConstraint(CriteriaConditionEnum criteriaConditionOperator)
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

		public static ProcessingFilterConstraint GetDateConstraint(CriteriaDateConditionEnum dateOperator)
		{
			return dateOperator switch
			{
				CriteriaDateConditionEnum.Between => ProcessingFilterConstraint.Between,
				CriteriaDateConditionEnum.In => ProcessingFilterConstraint.Between,
				CriteriaDateConditionEnum.Is => ProcessingFilterConstraint.Between,
				CriteriaDateConditionEnum.IsAfter => ProcessingFilterConstraint.IsAfter,
				CriteriaDateConditionEnum.IsAfterOrOn => ProcessingFilterConstraint.IsAfterOrOn,
				CriteriaDateConditionEnum.IsBefore => ProcessingFilterConstraint.IsBefore,
				CriteriaDateConditionEnum.IsBeforeOrOn => ProcessingFilterConstraint.IsBeforeOrOn,
				CriteriaDateConditionEnum.IsSet => ProcessingFilterConstraint.IsSet,
				_ => throw new ArgumentOutOfRangeException(nameof(dateOperator))
			};
		}

		public static ProcessingFilterConstraint GetNegateConstraint(ProcessingFilterConstraint constraint)
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

}

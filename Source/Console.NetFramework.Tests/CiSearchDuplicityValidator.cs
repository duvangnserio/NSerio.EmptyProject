using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Console.NetFramework.Tests
{
	public class DtSearchQueryAnalyzer
	{
		public bool HasDuplicateTerms(string query)
		{
			// Normalize the query (remove extra spaces, convert to lowercase)
			query = NormalizeQuery(query);

			// Extract individual terms and phrases
			var terms = ExtractTermsAndPhrases(query);

			// Check for duplicates
			var distinctTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var term in terms)
			{
				if (!distinctTerms.Add(term))
				{
					return true; // Duplicate found
				}
			}

			return false; // No duplicates
		}

		private string NormalizeQuery(string query)
		{
			// Convert to lowercase and trim
			query = query.ToLower().Trim();

			// Replace multiple spaces with a single space
			return Regex.Replace(query, @"\s+", " ");
		}

		private List<string> ExtractTermsAndPhrases(string query)
		{
			var terms = new List<string>();

			// Handle quoted phrases first
			var phraseMatches = Regex.Matches(query, "\"([^\"]*)\"");
			foreach (Match match in phraseMatches)
			{
				terms.Add(match.Value);
				query = query.Replace(match.Value, " ");
			}

			// Handle Boolean operators and other special syntax
			query = PreprocessBooleanOperators(query);

			// Split remaining terms
			var remainingTerms = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			terms.AddRange(remainingTerms);

			return terms;
		}

		private string PreprocessBooleanOperators(string query)
		{
			// Replace AND, OR, NOT with placeholders to prevent them from being counted as terms
			// This is a simplified approach; you might need more complex parsing for actual implementation
			query = Regex.Replace(query, @"\bAND\b", " ");
			query = Regex.Replace(query, @"\bOR\b", " ");
			query = Regex.Replace(query, @"\bNOT\b", " ");

			// Handle proximity operators (w/n)
			query = Regex.Replace(query, @"w\/\d+", " ");

			return query;
		}
	}

	public class ValidateDuplicity
	{
		public static void Test()
		{
			// Create an array of test scenarios
			var testScenarios = new[]
			{
				new TestScenario("Simple duplicate", "contract contract", true),
				new TestScenario("Case-insensitive duplicates", "agreement Agreement AGREEMENT", true),
				new TestScenario("No duplicates", "contract agreement license patent", false),
				new TestScenario("Duplicate phrases", "\"intellectual property\" AND \"intellectual property\"", true),
				new TestScenario("Boolean redundancy (OR)", "patent OR patent", true),
				new TestScenario("Boolean redundancy (AND)", "license AND license", true),
				new TestScenario("Mixed operator redundancy", "contract OR (agreement AND contract)", true),
				new TestScenario("Field search duplicates", "author:smith AND author:smith", true),
				new TestScenario("Field search without duplicates", "author:smith AND title:patent", false),
				new TestScenario("Wildcard redundancy", "employ* employ*", true),
				new TestScenario("Wildcard without duplicates", "employ* work*", false),
				new TestScenario("Proximity search duplicates", "\"agreement w/5 terminate\" OR \"agreement w/5 terminate\"", true),
				new TestScenario("Proximity search variations", "\"agreement w/5 terminate\" OR \"agreement w/3 terminate\"", false),
				new TestScenario("Nested expression duplicates", "(patent OR trademark) AND (patent OR copyright)", true),
				new TestScenario("Complex query redundancy", "(contract w/5 terminate) OR (license AND (contract w/5 terminate))", true),
				new TestScenario("Date search duplicates", "date > 01/01/2023 AND date > 01/01/2023", true),
				new TestScenario("Different date searches", "date > 01/01/2023 AND date < 12/31/2023", false),
				new TestScenario("Fuzzy search duplicates", "%smith %smith", true),
				new TestScenario("Mixed terms with duplicates", "contract license patent contract", true),
				new TestScenario("Complex query without duplicates", "(contract OR agreement) AND (license OR patent) AND copyright", false)
			};

			// Test each scenario
			var analyzer = new DtSearchQueryAnalyzer();

			System.Console.WriteLine("Testing dtSearch Query Duplicate Term Detection");
			System.Console.WriteLine("==============================================");

			foreach (var scenario in testScenarios)
			{
				bool result = analyzer.HasDuplicateTerms(scenario.Query);
				bool passed = result == scenario.ExpectedResult;

				System.Console.WriteLine($"Scenario: {scenario.Name}");
				System.Console.WriteLine($"Query: {scenario.Query}");
				System.Console.WriteLine($"Expected: {scenario.ExpectedResult}, Actual: {result}, Test {(passed ? "PASSED" : "FAILED")}");
				System.Console.WriteLine("----------------------------------------------");
			}

			System.Console.WriteLine("Press any key to exit...");
			System.Console.ReadKey();
		}
	}

	// Class to represent a test scenario
	public class TestScenario
	{
		public string Name { get; }
		public string Query { get; }
		public bool ExpectedResult { get; }

		public TestScenario(string name, string query, bool expectedResult)
		{
			Name = name;
			Query = query;
			ExpectedResult = expectedResult;
		}
	}
}
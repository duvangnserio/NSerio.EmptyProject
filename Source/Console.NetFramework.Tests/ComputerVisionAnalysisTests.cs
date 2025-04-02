using Azure;
using Azure.AI.Vision.ImageAnalysis;
using System;
using System.Threading.Tasks;

namespace Console.NetFramework.Tests
{
	internal class ComputerVisionAnalysisTests
	{
		private const string _DOCUMENT_URL =
			"https://r1fssa794947068.blob.core.windows.net/t005/Files/Cimplifi/EDDS3663333/RV_ceb16810-2223-4542-8a91-65f5de4a65ce/ade67644-6d19-4a84-811b-7674d25809eb?skoid=79034840-1d78-4f2c-98ab-ec45d5b6dd23&sktid=98b658f5-ca7d-43c3-8157-6554f853a01b&skt=2025-01-18T01:56:33Z&ske=2025-01-18T09:56:33Z&sks=b&skv=2024-05-04&sv=2024-05-04&spr=https&st=2025-01-18T01:41:33Z&se=2025-01-18T09:56:33Z&sr=d&sp=rl&sdd=3&sig=F5ZOImMJvDT%2Fi1g5qDapG6WInd1mDtToBo4GgvuAeHE%3D";

		private const string _UNEXPIRED_DOCUMENT_URL =
			"https://r1fssa794947068.blob.core.windows.net/t005/Files/Cimplifi/EDDS3663333/20250117/4876/f31f8629-82df-42e2-b62c-2e87372050db/81089354-9abb-4491-88da-0bd4ab1b3854_1.TIF?skoid=79034840-1d78-4f2c-98ab-ec45d5b6dd23&sktid=98b658f5-ca7d-43c3-8157-6554f853a01b&skt=2025-01-31T14%3A41%3A34Z&ske=2025-01-31T22%3A41%3A34Z&sks=b&skv=2024-05-04&sv=2024-05-04&spr=https&st=2025-01-31T14%3A26%3A34Z&se=2025-01-31T22%3A41%3A34Z&sr=d&sp=rl&sdd=7&sig=PalN%2Fv%2BHahaCjXWsHuUxMVj5KadnnO04dv%2BXF6ZO18A%3D";
		private const string _API_KEY = "c1d3dd0b61ea47fe92a397fc5f3a9dad";
		private const string _API_URL = "https://test-ist-vision.cognitiveservices.azure.com/";
		private const VisualFeatures _VISUAL_FEATURES = VisualFeatures.Tags | VisualFeatures.DenseCaptions;

		internal Task ExecuteAsync()
		{
			try
			{
				var results = GetAnalysisResultForImageAsync(_DOCUMENT_URL, _API_KEY, _API_URL, _VISUAL_FEATURES)
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult();

				return Task.CompletedTask;
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e);
				throw;
			}
		}

		public async Task<Response<ImageAnalysisResult>> GetAnalysisResultForImageAsync(string imageUrl, string apiKey, string apiUrl,
			VisualFeatures visualFeatures)
		{
			var apiUri = new Uri(apiUrl);
			var imageUri = new Uri(imageUrl);
			var azureKeyCredential = new AzureKeyCredential(apiKey);
			var imageAnalysisClient = new ImageAnalysisClient(apiUri, azureKeyCredential);
			var resultCaption = await imageAnalysisClient.AnalyzeAsync(imageUri, visualFeatures);

			return resultCaption;
		}
	}
}

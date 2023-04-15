using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;
using GarbageWatch.Helpers;
using System.Net;

namespace GarbageWatch.Services
{
    public class GarbageDetectionService
    {
        private string imagePath;
        private List<string> tags;
        public GarbageDetectionService(string ImagePath)
        {
            imagePath = ImagePath;
            tags = new List<string>();
        }

        public async Task<List<string>> DetectGarbage()
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(CognitiveServicesHelper.AzureComputerVisionAPIKey))
              { Endpoint = CognitiveServicesHelper.AzureComputerVisionAPIEndpoint };

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags
            };

            using (Stream analyzeImageStream = File.OpenRead(imagePath))
            {

                ImageAnalysis results = await client.AnalyzeImageInStreamAsync(analyzeImageStream, visualFeatures: features);

                foreach (var tag in results.Tags)
                {
                    tags.Add(tag.Name);
                }
            }
            return tags;
        }
    }
}

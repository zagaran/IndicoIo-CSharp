using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace IndicoIo_CSharp
{
    public class Custom : ApiClient
    {
        public Custom() : base()
        {
        }
        public Custom(string apiKey) : base(apiKey)
        {
        }

        public JToken Predict(string inputImage, Dictionary<string, object> config = null)
        {
            return CustomCall("predict", inputImage, config);
        }

        public List<JToken> Predict(List<string> inputImages, Dictionary<string, object> config = null)
        {
            return CustomCall("predict", inputImages, config);
        }

        public Dictionary<object, JToken> Predict(Dictionary<object, string> imageDictionary, Dictionary<string, object> config = null)
        {
            List<object> uuids = new List<object>();
            List<string> imageData = new List<string>();

            foreach(KeyValuePair<object, string> kvp in imageDictionary)
            {
                uuids.Add(kvp.Key);
                imageData.Add(kvp.Value);
            }

            List<JToken> apiResults = CustomCall("predict", imageData, config);
            return uuids.Zip(apiResults, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        }

        private JToken CustomCall(string method, string data, Dictionary<string, dynamic> config = null)
        {
            string processedImage = PreprocessData(data);

            JObject results = BaseCall("custom", processedImage, false, method, config);
            return results["results"];
        }
        private List<JToken> CustomCall(string method, List<string> data, Dictionary<string, dynamic> config = null)
        {
            List<string> processedImages = PreprocessData(data);
            JObject results = BaseCall("custom", processedImages, true, method, config);
            List<JToken> resultsList = results["results"].ToList();
            return resultsList;
        }

        private List<string> PreprocessData(List<string> inputImages)
        {
            List<string> processedImages = new List<string>();
            foreach (string inputImage in inputImages)
            {
                processedImages.Add(PreprocessData(inputImage));
            }

            return processedImages;
        }

        private string PreprocessData(string inputImage)
        {
            if (File.Exists(inputImage))
            {
                byte[] fileContents = File.ReadAllBytes(inputImage);
                return Convert.ToBase64String(fileContents);
            }
            else
            {
                return Regex.Replace(inputImage, "^data:image/.+;base64,", "");
            }
        }
    }
}


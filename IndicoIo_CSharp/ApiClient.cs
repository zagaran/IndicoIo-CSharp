using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace IndicoIo_CSharp
{
    public class ApiClient
    {
        private const String PUBLIC_BASE_URL = "https://apiv2.indico.io";

        public string apiKey;

        public ApiClient()
        {
            string possibleKey = Environment.GetEnvironmentVariable("INDICO_API_KEY");
            if (possibleKey == null)
            {
                Dictionary<string, string> properties = ReadProperties("indicorc");
                if(properties != null)
                {
                    properties.TryGetValue("INDICO_API_KEY", out possibleKey);
                }
            }

            this.apiKey = possibleKey ?? throw new Exception("No api Key was specified in the environment or config file");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
        public ApiClient(string apiKey)
        {
            this.apiKey = apiKey;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public JObject BaseCall(string api, object inputData, bool batch, string method, Dictionary<string, dynamic> extraParams = null)
        {
            JObject content = DoPost(inputData, api, batch, method, extraParams);
            return content;
        }

        private JObject DoPost(object inputData, string api, bool batch, string method, Dictionary<string, dynamic> urlParams = null)
        {
            string req = CreateRequest(api, method, batch);
            RestClient client = new RestClient(PUBLIC_BASE_URL);
            RestRequest request = new RestRequest(req, Method.POST);

            Dictionary<string, dynamic> rawParams = new Dictionary<string, dynamic>();
            if (urlParams != null)
            {
                foreach (var val in urlParams)
                {
                    rawParams[val.Key] = val.Value;
                }
            }

            rawParams["data"] = inputData;
            request.AddJsonBody(rawParams);

            request.AddHeader("content-type", "application/json");
            request.AddHeader("client-lib", "CSharp");
            request.AddHeader("Accept-Charset", "utf-8");
            request.AddHeader("X-ApiKey", apiKey);
            
            var response = client.Execute(request);
            
            if (response.Content != null && response.Content != "")
            {
                JObject jsonData = JObject.Parse(response.Content);

                if (jsonData["results"] == null)
                {
                    throw new Exception(api + " " + method + " failed with error: " + (jsonData["error"] ?? "unexpected error"));
                }

                return jsonData;
            }
            else
            {
                throw new Exception(api + " " + method + " failed with error: " + response.ErrorMessage);
            }
        }

        private string CreateRequest(string api, string method, bool batch)
        {
            string apiUrlSeg = api;
            string batchUrlSeg = batch ? "/batch" : "";
            string methodUrlSeg = method != null ? "/" + method : "";

            return apiUrlSeg + batchUrlSeg + methodUrlSeg;
        }

        private Dictionary<string, string> ReadProperties(string filePath)
        {
            if (File.Exists(filePath))
            {
                var properties = new Dictionary<string, string>();
                foreach (var row in File.ReadAllLines(filePath))
                    properties.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));

                return properties;
            }
            else
            {
                return null;
            }
        }
    }
}

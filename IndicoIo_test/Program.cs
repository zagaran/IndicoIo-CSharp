using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndicoIo_CSharp;
using Newtonsoft.Json.Linq;

namespace IndicoIo_test
{
    class Program
    {


        static void Main(string[] args)
        {
            TestKeys();

            TestPredictMethods();

            TestInputMethods();

            Console.WriteLine("Press Any Key to exit.");
            Console.ReadKey();
        }

        public static void TestKeys()
        {
            Dictionary<string, object> config = new Dictionary<string, object>
            {
                { "collection", "<COLLECTION_ID>" }
            };

            Custom c = new Custom("<API_KEY>");
            var data = c.Predict(@"..\..\fish.jpg", config);
            Console.WriteLine(data);

            File.WriteAllText("indicorc", "INDICO_API_KEY=<API_KEY>");
            Custom c2 = new Custom();
            data = c2.Predict(@"..\..\fish.jpg", config);
            Console.WriteLine(data);

            File.Delete("indicorc");
            Environment.SetEnvironmentVariable("INDICO_API_KEY", "<API_KEY>");
            Custom c3 = new Custom();
            data = c3.Predict(@"..\..\fish.jpg", config);
            Console.WriteLine(data);
        }

        public static void TestPredictMethods()
        {
            Dictionary<string, object> config = new Dictionary<string, object>
            {
                { "collection", "COLLECTION_ID>" }
            };

            Custom c = new Custom("<API_KEY>");
            var data = c.Predict(@"..\..\fish.jpg", config);
            Console.WriteLine(data);

            List<string> dataList = new List<string> { @"..\..\fish.jpg", @"..\..\fish.jpg" };
            var data2 = c.Predict(dataList, config);
            var lines2 = data2.Select(kvp => kvp);
            Console.WriteLine(string.Join(", ", lines2));

            Dictionary<object, string> dataDict = new Dictionary<object, string>()
            {
                { "abc", @"..\..\fish.jpg" },
                { "def", @"..\..\fish.jpg" },
            };
            var data3 = c.Predict(dataDict, config);
            var lines3 = data3.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
            Console.WriteLine(string.Join(Environment.NewLine, lines3));
        }

        public static void TestInputMethods()
        {
            Dictionary<string, object> config = new Dictionary<string, object>
            {
                { "collection", "COLLECTION_ID>" }
            };

            Custom c = new Custom("<API_KEY>");
            var data = c.Predict(@"..\..\fish.jpg", config);
            Console.WriteLine(data);

            data = c.Predict(@"https://upload.wikimedia.org/wikipedia/commons/3/3d/Latimeria_chalumnae01.jpg", config);
            Console.WriteLine(data);

            byte[] fileContents = File.ReadAllBytes(@"..\..\fish.jpg");
            string imageData = Convert.ToBase64String(fileContents);
            data = c.Predict(imageData, config);
            Console.WriteLine(data);
        }
    }
}

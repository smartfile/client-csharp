using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartFile;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using Newtonsoft.Json.Linq;

namespace SmartFile.Tests
{
    [TestClass()]
    public class ClientTests
    {

        [TestMethod]
        public void ConstructorTest()
        {
            var client = new Client("https://app.smartfile.com/api/2/");
            string API_KEY = Environment.GetEnvironmentVariable("API_KEY");
            string API_PASS = Environment.GetEnvironmentVariable("API_PASS");
            client.Authenticator = new HttpBasicAuthenticator(API_KEY, API_PASS);
        }

        [TestMethod]
        public void CanConnectTest()
        {
            var client = new Client("https://app.smartfile.com/api/2/");
            string API_KEY = Environment.GetEnvironmentVariable("API_KEY");
            string API_PASS = Environment.GetEnvironmentVariable("API_PASS");
            client.Authenticator = new HttpBasicAuthenticator(API_KEY, API_PASS);

            var request = new RestRequest("/ping/", Method.GET);

            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string

            // Deserialize response to a C# dict
            JObject o = JObject.Parse(content);
            string ping = (string)o.SelectToken("ping");

            // Assert 'ping':'pong'
            Assert.AreEqual("pong", ping);
        }

        [TestMethod]
        public void UploadTest()
        {
            var client = new Client("http://app.smartfile.com/api/2/");
            string API_KEY = Environment.GetEnvironmentVariable("API_KEY");
            string API_PASS = Environment.GetEnvironmentVariable("API_PASS");
            client.Authenticator = new HttpBasicAuthenticator(API_KEY, API_PASS);

            // Get info from local file
            FileInfo localFileInfo = new FileInfo(@"C:\\Users\\usr\\Documents\\GitHub\\client-csharp\\TextFile1.txt");
            long localFileSize = localFileInfo.Length;

            // Uploads that file
            var request = Client.Upload("C:\\Users\\usr\\Documents\\GitHub\\client-csharp\\TextFile1.txt");
            // Checks info of file we just uploaded
            var requestFileInfo = new RestRequest("/path/info/TextFile1.txt", Method.GET);

            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string

            IRestResponse responseFileInfo = client.Execute(requestFileInfo);
            var fileInfoContent = responseFileInfo.Content; // raw content as string

            // Parses the JSON to get the size of the uploaded file
            JObject o = JObject.Parse(fileInfoContent);
            long uploadedFileSize = (long)o.SelectToken("size");

            Assert.AreEqual(localFileSize, uploadedFileSize);
        }
}
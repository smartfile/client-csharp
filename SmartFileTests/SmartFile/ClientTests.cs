using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;

using client_csharp;

namespace SmartFile.Tests
{
    [TestClass()]
    public class ClientTests
    {
        public void ConstructorTest()
        {
            // Checks constructors
            var client = new Client("https://app.smartfile.com/api/2/");
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentVariables.API_KEY, EnvironmentVariables.API_PASS);
        }

        public void CanConnectTest()
        {

            // Ping test to assure we can connect
            var client = new Client("https://app.smartfile.com/api/2/");
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentVariables.API_KEY, EnvironmentVariables.API_PASS);

            var request = new RestRequest("/ping/", Method.GET);

            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string

            // Deserialize response to a C# dict
            JObject o = JObject.Parse(content);
            string ping = (string)o.SelectToken("ping");

            // Assert 'ping':'pong'
            Assert.AreEqual("pong", ping);
        }

        public void UploadTest()
        {
            var client = new Client("http://app.smartfile.com/api/2/");
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentVariables.API_KEY, EnvironmentVariables.API_PASS);

            // Get info from local file
            FileInfo localFileInfo = new FileInfo(@LocalFile.MyTextFile);
            long localFileSize = localFileInfo.Length;

            // Uploads that file
            var request = Client.Upload(LocalFile.MyTextFile);
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

        public void DownloadTest()
        {
            var client = new Client("http://app.smartfile.com/api/2/");
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentVariables.API_KEY, EnvironmentVariables.API_PASS);

            // Downloads file from SmartFile to a location other than the orginal local copy
            var request = Client.Download("TextFile1.txt");
            client.DownloadData(request).SaveAs(SaveLocation.MyTextFile);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            // Reads text from both files to compare and assure they are the same
            string LocalFileText = File.ReadAllText(@LocalFile.MyTextFile);
            string RemoteFileText = File.ReadAllText(SaveLocation.MyTextFile);

            Assert.AreEqual(LocalFileText, RemoteFileText);
        }

        public void MoveTest()
        {
            var client = new Client("http://app.smartfile.com/api/2/");
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentVariables.API_KEY, EnvironmentVariables.API_PASS);

            // Moves file to a new location
            var request = Client.Move("TextFile1.txt", "/newFolder/");

            IRestResponse response = client.Execute(request);
            var content = response.Content;
        }

        public void DeleteTest()
        {
            var client = new Client("http://app.smartfile.com/api/2/");
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentVariables.API_KEY, EnvironmentVariables.API_PASS);

            // Deletes file from that new location
            var request = Client.Delete("/newFolder/TextFile1.txt");

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            JObject o = JObject.Parse(content);
            string detail = (string)o.SelectToken("type");

            // If the file does not exist this will fail because detail will return null
            Assert.AreEqual("Remove", detail);
        }

        // Runs all the tests in the order wanted
        [TestMethod]
        public void RunAllTests()
        {
            ConstructorTest();
            CanConnectTest();
            UploadTest();
            DownloadTest();
            MoveTest();
            DeleteTest();
        }

    }
}